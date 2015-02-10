
//    __/_    \/    __/     \/                       _           _ ,,                  ,-
//     /      /\     /      /      _ _              //           \\||                  \ \
//                                 \V/         ____//____         (@)              _____\ \_____
//       ><         X  Y            @         '----@,----'        ||\\            (______(@)____)
//           -<                     U             //              || \\                   \ \  
//                                                "               ""  "                    \_\

// Opera: "Studio di cromosomi"
// Tecnica: "Monospace su canvas"


//                                 ok ci ho provato

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    public class NNGenome
    {
        const int INPUTS_COUNT = 135;
        const int INPUTS_AND_BIAS_COUNT = INPUTS_COUNT + 1;
        const int OUTPUTS_COUNT = 20;

        // parameters
        const float WEIGHT_RANGE = 5.0f;
        const float DISJ_EXC_RECOMB_PROB = 0.1f;// disjoint-excess recombine probability
        const float MUT_PROP = 0.02f;           // mutate genome proportion
        const float WEIGHT_MUT_PROP = 0.02f;    // weight mutation proportion    0.02 -> 2% of connectivity
        const float MAX_WEIGHT_PERT_PROP = 0.4f;// maximum weight perturbation proportion

        // mutation probalility coefficients
        const float UNCHANGED_MUT_PROB = 10f;   // do nothing probability
        const float WEIGHT_MUT_PROB = 0.988f;   // weight mutation probability
        const float ADD_NODE_MUT_PROB = 0.01f;  // add node mutation probability
        const float ADD_LINK_MUT_PROB = 0.01f;  // add link mutation probability
        const float DEL_LINK_MUT_PROB = 0.01f;  // delete link mutation probability

        //random generators
        static RouletteWheel stdMutationRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_LINK_MUT_PROB, DEL_LINK_MUT_PROB, UNCHANGED_MUT_PROB);
        static RouletteWheel alwaysMutateRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_LINK_MUT_PROB, DEL_LINK_MUT_PROB);
        static FastRandom rand = new FastRandom();
        static RandomBool randBool = new RandomBool();

        //--------Globally store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure:
        //Links: based on source & target ID, not on weight
        //Nodes: based on link they replaced

        //ID counter for both linkBuffer and nodeBuffer
        static uint lastID;

        //The key is the link ID the AddedNode struct replaced
        //The ID is contained in the AddedNode sruct
        static KVCircularBuffer<uint, AddedNode> nodeBuffer = new KVCircularBuffer<uint, AddedNode>(0x20000);

        //The value is the actual ID
        static KVCircularBuffer<AddedLink, uint?> linkBuffer = new KVCircularBuffer<AddedLink, uint?>(0x20000);


        //Being specific genomes
        public Dictionary<uint, NodeGene> NodeGeneList { get; private set; }
        public SortedList<uint, LinkGene> LinkGeneList { get; private set; }// need sorted, for the recombination algorithm
        // Complexity comparison:
        // Dictionary: unsorted, search by index: O(1) (in practice is slower), search by key: O(1),     add: O(1)
        // SortedList: sorted,   search by index: O(log n),                     search by key: O(log n), add: O(n) (O(1) if sorted)
        // TODO: test if it's better to switch to SortedDictionary (consider faster manipulation and slower access)

        public FloatCircularBuffer FitnessHistory { get; private set; }

        /// <summary>
        /// Initialize a random genome
        /// </summary>
        public NNGenome()
        {
            NodeGeneList = new Dictionary<uint, NodeGene>(INPUTS_AND_BIAS_COUNT + OUTPUTS_COUNT);
            NodeGeneList.Add(0, new NodeGene(NodeType.Bias));
            for (uint i = 1; i < INPUTS_AND_BIAS_COUNT; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Input));
            }
            for (uint i = INPUTS_AND_BIAS_COUNT; i < OUTPUTS_COUNT + INPUTS_AND_BIAS_COUNT; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Output));
            }

            LinkGeneList = new SortedList<uint, LinkGene>(5);
            for (int i = 0; i < 5; i++)// choose how many links to add for starting the simulation
            {
                addLink();
            }

            FitnessHistory = new FloatCircularBuffer(10);

        }

        /// <summary>
        /// Create a new genome recombining two genomes.
        /// </summary>
        // for now this is unfinisced: the recombination applies only over weights and not over morphology of the net
        // the structure is copied from the fittest genome
        public NNGenome(NNGenome parent1, NNGenome parent2)
        {
            //determine fittest parent
            NNGenome fitPar, weakPar;
            if (parent1.FitnessHistory.Mean > parent2.FitnessHistory.Mean)
            {
                fitPar = parent1;
                weakPar = parent2;
            }
            else
            {
                fitPar = parent2;
                weakPar = parent1;
            }

            // add nodes to NodeGeneList
            var nodeGeneList = new Dictionary<uint, NodeGene>(fitPar.NodeGeneList.Count + weakPar.NodeGeneList.Count);
            foreach (var kvp in fitPar.NodeGeneList)
            {
                nodeGeneList.Add(kvp.Key, new NodeGene(kvp.Value));
            }

            // correlate link genes:
            // if present in both: choose the weight randomly from one of the parent
            // if present in the fittest parent: just use it
            // if present in the weakest parent: add it to the disjoint-excess list

            var list1Count = fitPar.LinkGeneList.Count; // precalculate values & references
            var list2Count = weakPar.LinkGeneList.Count;
            var idList1 = fitPar.LinkGeneList.Keys;
            var linkList1 = fitPar.LinkGeneList.Values;
            var idList2 = weakPar.LinkGeneList.Keys;
            var linkList2 = weakPar.LinkGeneList.Values;

            var linkGeneList = new SortedList<uint, LinkGene>(list1Count + list2Count);
            var disjExcList = new List<KeyValuePair<uint, LinkGene>>(list2Count); // genes in weakPar that don't match in fitPar
            var recombDisjExc = rand.Next() < DISJ_EXC_RECOMB_PROB;

            var idx1 = 0;
            var idx2 = 0;
            uint ID1, ID2;
            LinkGene link1, link2;
            while (idx1 == list1Count && idx2 == list2Count)
            {
                ID1 = idList1[idx1];
                ID2 = idList2[idx2];
                link1 = linkList1[idx1];
                link2 = linkList2[idx2];

                if (ID1 == ID2)
                {
                    linkGeneList.Add(ID1, new LinkGene(randBool.Next() ? link1 : link2));
                    idx1++;
                    idx2++;
                }
                else if (ID1 < ID2) // then beacause in fitPar there's a gene that don't match in weakPar
                {
                    linkGeneList.Add(ID1, new LinkGene(link1));
                    idx1++;
                }
                else // ID1 > ID2
                {
                    if (recombDisjExc)
                    {
                        disjExcList.Add(new KeyValuePair<uint, LinkGene>(ID2, link2));
                    }
                    idx2++;
                }

                if (idx2 == list2Count)
                {
                    while (idx1 < list1Count)
                    {
                        idx1++;
                        linkGeneList.Add(idList1[idx1], new LinkGene(linkList1[idx1]));
                    }
                    break;
                }

                if (idx1 == list1Count)
                {
                    if (recombDisjExc)
                    {

                        while (idx2 < list2Count)
                        {
                            idx2++;
                            disjExcList.Add(new KeyValuePair<uint, LinkGene>(idList2[idx2], new LinkGene(linkList2[idx2])));
                        }
                    }
                    break;
                }
            }

            //TODO: this works (and allow evolution) but is missing code for adding disjoint-excess
            // (need an additional SortedDictionary<AddedLink, uint?> filled with LinkGeneList genes)


            //mutate
            mutate();//consider calling this multiple times

            LinkGeneList = linkGeneList;
            NodeGeneList = nodeGeneList;
            FitnessHistory = new FloatCircularBuffer(10);
        }

        void mutate()
        {
            switch ((true/*here the SimulationType flag*/? stdMutationRW : alwaysMutateRW).Spin())
            {
                case 0:
                    mutateWeight();
                    break;
                case 1:
                    addNode();
                    break;
                case 2:
                    addLink();
                    break;
                case 3:
                    removeLink();
                    break;
                default:
                    //do nothing
                    break;
            }
        }

        void mutateWeight()//sharpneat's code for weight mutation was extremely long and redundant with lot of wasted r.n.g.
        {
            var n = (int)Math.Ceiling(WEIGHT_MUT_PROP * LinkGeneList.Count);
            var valueList = LinkGeneList.Values;
            for (int i = 0; i < n; i++)
            {
                var m = rand.Next(n);// it doesn't matter mutating a connection twice

                var weight = valueList[m].Weight + 2 * (float)rand.NextDouble() * MAX_WEIGHT_PERT_PROP - MAX_WEIGHT_PERT_PROP;
                valueList[m].Weight = (weight < WEIGHT_RANGE ? (weight > -WEIGHT_RANGE ? weight : -WEIGHT_RANGE) : WEIGHT_RANGE);
            }
        }


        #region addNode
        /// <summary>
        /// Replace a link gene with a node gene and two link genes, in a way to achieve a similiar behaviour
        /// </summary>
        void addNode()
        {
            var idx = rand.Next(LinkGeneList.Count);// when picking items in a sortedList, the index mustn't be confused with the key (that is the ID)
            var oldLinkID = LinkGeneList.Keys[idx];//   blabla[n] -> pick by key/ID       blabla.Values[n] -> pick by index
            var oldLink = LinkGeneList.Values[idx];
            LinkGeneList.Remove(oldLinkID);

            var addedNode = getAddedNode(oldLinkID);

            var newNode = new NodeGene();// NodeType set to hidden
            NodeGeneList.Add(addedNode.NodeID, newNode);
            //TODO: recheck the following approach
            //input link with oldLink weight
            LinkGeneList.Add(addedNode.InpLinkID, new LinkGene(oldLink.SourceID, addedNode.NodeID, oldLink.Weight));
            //output link with max weight
            LinkGeneList.Add(addedNode.OutpLinkID, new LinkGene(addedNode.NodeID, oldLink.TargetID, WEIGHT_RANGE));

            var srcNode = NodeGeneList[oldLink.SourceID];//search by key -> binary search
            srcNode.TgtNodeIDs.Remove(oldLink.SourceID);
            srcNode.TgtNodeIDs.Add(addedNode.NodeID);
            newNode.SrcNodeIDs.Add(oldLink.SourceID);

            var tgtNode = NodeGeneList[oldLink.TargetID];//search by key -> binary search
            tgtNode.SrcNodeIDs.Remove(oldLink.SourceID);
            tgtNode.SrcNodeIDs.Add(addedNode.NodeID);
            newNode.TgtNodeIDs.Add(oldLink.TargetID);

        }

        /// <summary>
        /// Returns an AddedNode struct with the right IDs and, if necessary, save the new struct in the global buffer
        /// </summary>
        AddedNode getAddedNode(uint oldLinkID)//tried to shrink sharpneat code but didn't succeed
        {
            AddedNode addedNode;
            var isNewAddedNode = false;
            if (nodeBuffer.TryGetValue(oldLinkID, out addedNode))
            {
                if (!(NodeGeneList.ContainsKey(addedNode.NodeID) || LinkGeneList.ContainsKey(addedNode.InpLinkID) || LinkGeneList.ContainsKey(addedNode.OutpLinkID)))
                {
                    return addedNode;
                }
            }
            else
            {
                isNewAddedNode = true;
            }

            addedNode = new AddedNode(ref lastID);

            if (isNewAddedNode)
            {
                nodeBuffer.Enqueue(oldLinkID, addedNode);
            }
            return addedNode;
        }
        #endregion

        /// <summary>
        /// Allowed recursive links and two way links. After 5 search attempts, mutate weight.
        /// </summary>
        void addLink()
        {
            var nodeCount = NodeGeneList.Count;

            for (int i = 0; i < 5; i++)
            {
                var srcID = NodeGeneList.Keys.ElementAt(rand.Next(nodeCount));
                var srcNode = NodeGeneList[srcID];//ind Dictionary, key search is fast

                var tgtID = NodeGeneList.Keys.ElementAt(INPUTS_AND_BIAS_COUNT + rand.Next(nodeCount - INPUTS_AND_BIAS_COUNT));// input and bias nodes can't be targets
                var tgtNode = NodeGeneList[tgtID];
                //if srcID == tgtID then the link is recursive

                if (!srcNode.TgtNodeIDs.Contains(tgtID))
                {
                    var addedLink = new AddedLink(srcID, tgtID);
                    uint? existingLinkID;// must be nullable, to handle the case there isn't an existing ID
                    var newLink = new LinkGene(srcID, tgtID, ((float)rand.NextDouble() * 2f - 1f) * WEIGHT_RANGE);

                    if (linkBuffer.TryGetValue(addedLink, out existingLinkID))
                    {
                        LinkGeneList.Add(existingLinkID.Value, newLink);
                    }
                    else
                    {
                        LinkGeneList.Add(++lastID, newLink);
                        linkBuffer.Enqueue(addedLink, lastID);//same ID
                    }
                    srcNode.TgtNodeIDs.Add(tgtID);
                    tgtNode.SrcNodeIDs.Add(srcID);

                    return;
                }
            }
            //else
            mutateWeight();
        }

        /// <summary>
        /// Remove a link. If there's only one link, then mutate weight.
        /// </summary>
        void removeLink()
        {
            if (LinkGeneList.Count == 1)
            {
                mutateWeight();
                return;
            }

            var idx = rand.Next(LinkGeneList.Count);
            var oldLink = LinkGeneList.Values[idx];
            LinkGeneList.RemoveAt(idx);

            //-------------Remove unnecessary nodes------------
            var srcID = oldLink.SourceID;
            var tgtID = oldLink.TargetID;

            //source node
            var srcNode = NodeGeneList[srcID];
            srcNode.TgtNodeIDs.Remove(tgtID);
            if (srcNode.IsRedundant) NodeGeneList.Remove(srcID);

            //target node
            var tgtNode = NodeGeneList[tgtID];
            tgtNode.SrcNodeIDs.Remove(srcID);
            if (tgtNode.IsRedundant) NodeGeneList.Remove(tgtID);


        }
    }
}
