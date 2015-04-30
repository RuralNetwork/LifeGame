
//    __/_    \/    __/     \/                       _           _ ,,                  ,-
//     /      /\     /      /      _ _              //           \\||                  \ \
//                                 \V/         ____//____         (@)              _____\ \_____
//       ><         X  Y            @         '----@,----'        ||\\            (______(@)____)
//           -<                     U             //              || \\                   \ \  
//                                                "               ""  "                    \_\

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    [Serializable]
    public class NNGenome
    {
        // parameters
        const float WEIGHT_RANGE = 5.0f;
        const float DISJ_EXC_RECOMB_PROB = 0.1f;// disjoint-excess recombine probability
        const float MUT_PROP = 0.02f;           // mutate genome proportion
        const float WEIGHT_MUT_PROP = 0.02f;    // weight mutation proportion    0.02 -> 2% of connectivity
        const float MAX_WEIGHT_PERT_PROP = 0.4f;// maximum weight perturbation proportion

        // mutation probalility coefficients
        const float UNCHANGED_MUT_PROB = 10f;   // do nothing probability
        const float WEIGHT_MUT_PROB = 0.7f;   // weight mutation probability
        const float ADD_NODE_MUT_PROB = 0.1f;  // add node mutation probability
        const float ADD_LINK_MUT_PROB = 0.1f;  // add link mutation probability
        const float DEL_LINK_MUT_PROB = 0.1f;  // delete link mutation probability

        //random generators
        static RouletteWheel stdMutationRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_LINK_MUT_PROB, DEL_LINK_MUT_PROB, UNCHANGED_MUT_PROB);
        static RouletteWheel alwaysMutateRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_LINK_MUT_PROB, DEL_LINK_MUT_PROB);
        static FastRandom rand = new FastRandom();
        static ZigguratGaussianSampler gaussRand = new ZigguratGaussianSampler();

        //Being specific genomes
        public Dictionary<ulong, NodeGene> NodeGeneList { get; private set; }
        public SortedList<ulong, LinkGene> LinkGeneList { get; private set; }// need sorted, for the recombination algorithm
        // Complexity comparison:
        // Dictionary: unsorted, search by index: O(1) (in practice is slower), search by key: O(1),     add: O(1)
        // SortedList: sorted,   search by index: O(log n),                     search by key: O(log n), add: O(n) (O(1) if sorted)
        // TODO: test if it's better to switch to SortedDictionary (consider faster manipulation but slower access)

        /// <summary>
        /// Initialize a random genome
        /// </summary>
        public NNGenome()
        {
            NodeGeneList = new Dictionary<ulong, NodeGene>(Constants.INPUTS_AND_BIAS_COUNT + Constants.OUTPUTS_COUNT);
            NodeGeneList.Add(0, new NodeGene(NodeType.Bias));
            for (ulong i = 1; i < Constants.INPUTS_AND_BIAS_COUNT; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Input));
            }
            for (ulong i = Constants.INPUTS_AND_BIAS_COUNT; i < Constants.OUTPUTS_COUNT + Constants.INPUTS_AND_BIAS_COUNT; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Output));
            }

            LinkGeneList = new SortedList<ulong, LinkGene>(5);
            for (int i = 0; i < 5; i++)// choose how many links to add for starting the simulation
            {
                addLink();
            }
        }
        /// <summary>
        /// Create a new genome mutating a genome
        /// </summary>
        public NNGenome(NNGenome genome)
        {
            initModdedGen(genome);
            mutate();
        }

        void initModdedGen(NNGenome gen)
        {
            NodeGeneList = new Dictionary<ulong, NodeGene>(gen.NodeGeneList.Count);
            foreach (var kvp in gen.NodeGeneList)
            {
                NodeGeneList.Add(kvp.Key, new NodeGene(kvp.Value));
            }
            LinkGeneList = new SortedList<ulong, LinkGene>(gen.LinkGeneList.Count);
            foreach (var kvp in gen.LinkGeneList)
            {
                LinkGeneList.Add(kvp.Key, new LinkGene(kvp.Value));
            }
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
            if (RandomBool.Next())
            {
                fitPar = parent1;
                weakPar = parent2;
            }
            else
            {
                fitPar = parent2;
                weakPar = parent1;
            }

            if (rand.NextDouble() > DISJ_EXC_RECOMB_PROB)
            {
                initModdedGen(fitPar);
            }
            else
            {
                // add nodes to NodeGeneList
                var nodeGeneList = new Dictionary<ulong, NodeGene>(fitPar.NodeGeneList.Count + weakPar.NodeGeneList.Count);
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

                var linkGeneList = new SortedList<ulong, LinkGene>(list1Count + list2Count);
                var disjExcList = new List<KeyValuePair<ulong, LinkGene>>(list2Count); // genes in weakPar that don't match in fitPar

                var idx1 = 0;
                var idx2 = 0;
                ulong ID1, ID2;
                LinkGene link1, link2;
                while (true)
                {
                    ID1 = idList1[idx1];
                    ID2 = idList2[idx2];
                    link1 = linkList1[idx1];
                    link2 = linkList2[idx2];

                    if (ID1 == ID2)
                    {
                        linkGeneList.Add(ID1, new LinkGene(RandomBool.Next() ? link1 : link2));
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
                        disjExcList.Add(new KeyValuePair<ulong, LinkGene>(ID2, link2));
                        idx2++;
                    }

                    if (idx2 == list2Count)
                    {
                        while (idx1 < list1Count)
                        {
                            linkGeneList.Add(idList1[idx1], new LinkGene(linkList1[idx1]));
                            idx1++;
                        }
                        break;
                    }

                    if (idx1 == list1Count)
                    {
                        while (idx2 < list2Count)
                        {
                            disjExcList.Add(new KeyValuePair<ulong, LinkGene>(idList2[idx2], new LinkGene(linkList2[idx2])));
                            idx2++;
                        }
                        break;
                    }
                }

                //TODO: this works (and allow evolution) but is missing code for adding disjoint-excess
                // (need an additional SortedDictionary<AddedLink, ulong?> filled with LinkGeneList genes)


                LinkGeneList = linkGeneList;
                NodeGeneList = nodeGeneList;

            }


            //mutate
            mutate();//consider calling this multiple times
        }

        void mutate()
        {
            switch (alwaysMutateRW.Spin())
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
                var m = rand.Next(LinkGeneList.Count);// it doesn't matter mutating a connection twice

                //var weight = valueList[m].Weight + 2 * (float)rand.NextDouble() * MAX_WEIGHT_PERT_PROP - MAX_WEIGHT_PERT_PROP;
                var weight = (float)gaussRand.NextSample(valueList[m].Weight, MAX_WEIGHT_PERT_PROP);
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

            var srcID = oldLink.SourceID;
            var tgtID = oldLink.TargetID;

            var srcNode = NodeGeneList[srcID];//search by key -> binary search
            srcNode.TgtNodeIDs.Remove(tgtID);
            srcNode.TgtNodeIDs.Add(addedNode.NodeID);
            newNode.SrcNodeIDs.Add(srcID);

            var tgtNode = NodeGeneList[tgtID];//search by key -> binary search
            tgtNode.SrcNodeIDs.Remove(srcID);
            tgtNode.SrcNodeIDs.Add(addedNode.NodeID);
            newNode.TgtNodeIDs.Add(tgtID);
        }

        /// <summary>
        /// Returns an AddedNode struct with the right IDs and, if necessary, save the new struct in the global buffer
        /// </summary>
        AddedNode getAddedNode(ulong oldLinkID)//tried to shrink sharpneat code but didn't succeed
        {
            AddedNode addedNode;
            var isNewAddedNode = false;
            if (Simulation.Instance.NNLists.nodeBuffer.TryGetValue(oldLinkID, out addedNode))
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

            addedNode = new AddedNode(ref Simulation.Instance.NNLists.lastID);

            if (isNewAddedNode)
            {
                Simulation.Instance.NNLists.nodeBuffer.Enqueue(oldLinkID, addedNode);
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

                var tgtID = NodeGeneList.Keys.ElementAt(Constants.INPUTS_AND_BIAS_COUNT + rand.Next(nodeCount - Constants.INPUTS_AND_BIAS_COUNT));// input and bias nodes can't be targets
                var tgtNode = NodeGeneList[tgtID];
                //if srcID == tgtID then the link is recursive

                if (!srcNode.TgtNodeIDs.Contains(tgtID))
                {
                    var addedLink = new AddedLink(srcID, tgtID);
                    ulong? existingLinkID;// must be nullable, to handle the case there isn't an existing ID
                    var newLink = new LinkGene(srcID, tgtID, ((float)rand.NextDouble() * 2f - 1f) * WEIGHT_RANGE);

                    if (Simulation.Instance.NNLists.linkBuffer.TryGetValue(addedLink, out existingLinkID))
                    {
                        LinkGeneList.Add(existingLinkID.Value, newLink);
                    }
                    else
                    {
                        Simulation.Instance.NNLists.linkBuffer.Enqueue(addedLink, ++Simulation.Instance.NNLists.lastID);
                        LinkGeneList.Add(Simulation.Instance.NNLists.lastID, newLink);//same ID
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

            var srcID = oldLink.SourceID;
            var tgtID = oldLink.TargetID;

            //source node
            var srcNode = NodeGeneList[srcID];
            srcNode.TgtNodeIDs.Remove(tgtID);

            //target node
            var tgtNode = NodeGeneList[tgtID];
            tgtNode.SrcNodeIDs.Remove(srcID);

            //-------------Remove unnecessary nodes------------
            if (srcNode.IsRedundant) NodeGeneList.Remove(srcID);
            if (tgtNode.IsRedundant) NodeGeneList.Remove(tgtID);


        }


        public bool integrityCheck()
        {
            foreach (var node in NodeGeneList)
            {
                foreach (var id in node.Value.SrcNodeIDs)
                {
                    if (!NodeGeneList.ContainsKey(id))
                    {
                        return false;
                    }
                }
                foreach (var id in node.Value.TgtNodeIDs)
                {
                    if (!NodeGeneList.ContainsKey(id))
                    {
                        return false;
                    }
                }

            }
            foreach (var link in LinkGeneList)
            {
                if (!NodeGeneList.ContainsKey(link.Value.SourceID) || !NodeGeneList.ContainsKey(link.Value.TargetID))
                {
                    return false;
                }

            }
            return true;
        }
    }

    [Serializable]
    public class NNGlobalLists
    {
        //--------Globally store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure:
        //Links: based on source & target ID, not on weight
        //Nodes: based on link they replaced

        //ID counter for both linkBuffer and nodeBuffer
        public ulong lastID = Constants.INPUTS_AND_BIAS_COUNT + Constants.OUTPUTS_COUNT;

        //The key is the link ID the AddedNode struct replaced
        //The ID is contained in the AddedNode struct
        public KVCircularBuffer<ulong, AddedNode> nodeBuffer = new KVCircularBuffer<ulong, AddedNode>(0x20000);

        //The value is the actual ID
        public KVCircularBuffer<AddedLink, ulong?> linkBuffer = new KVCircularBuffer<AddedLink, ulong?>(0x20000);
    }
}
