
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
        const float ADD_CONN_MUT_PROB = 0.01f;  // add connection mutation probability
        const float DEL_CONN_MUT_PROB = 0.01f;  // delete connection mutation probability

        //random generators
        static RouletteWheel stdMutationRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_CONN_MUT_PROB, DEL_CONN_MUT_PROB, UNCHANGED_MUT_PROB);
        static RouletteWheel alwaysMutateRW = new RouletteWheel(WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_CONN_MUT_PROB, DEL_CONN_MUT_PROB);
        static FastRandom rand = new FastRandom();
        static RandomBool randBool = new RandomBool();

        //--------Globally store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure:
        //Connections: based on source & target ID, not on weight
        //Nodes: based on connection they replaced

        //ID counter for both connectionBuffer and nodeBuffer
        static uint lastID;

        //The key is the connection ID the AddedNode struct replaced
        //The ID is contained in the AddedNode sruct
        static KVCircularBuffer<uint, AddedNode> nodeBuffer = new KVCircularBuffer<uint, AddedNode>(0x20000);

        //The value is the actual ID
        static KVCircularBuffer<AddedConnection, uint?> connectionBuffer = new KVCircularBuffer<AddedConnection, uint?>(0x20000);


        //Being specific genomes
        public SortedList<uint, NodeGene> NodeGeneList { get; private set; }
        public SortedList<uint, ConnectionGene> ConnectionGeneList { get; private set; }
        // SortedList:       fast to search by key, slow to add, fast to search by index  <-- best for our needs
        // SortedDictionary: fast to search by key, fast to add, slow to search by index

        public FloatCircularBuffer FitnessHistory { get; private set; }

        /// <summary>
        /// Initialize a random genome
        /// </summary>
        public NNGenome()
        {
            NodeGeneList = new SortedList<uint, NodeGene>(INPUTS_COUNT + OUTPUTS_COUNT + 1);
            NodeGeneList.Add(0, new NodeGene(NodeType.Bias));
            for (uint i = 1; i < INPUTS_COUNT + 1; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Input));
            }
            for (uint i = INPUTS_COUNT + 1; i < OUTPUTS_COUNT + INPUTS_COUNT + 1; i++)
            {
                NodeGeneList.Add(i, new NodeGene(NodeType.Output));
            }

            ConnectionGeneList = new SortedList<uint, ConnectionGene>(5);
            for (int i = 0; i < 5; i++)// choose how many connection to add for starting the simulation
            {
                addConnection();
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
            NodeGeneList = new SortedList<uint, NodeGene>(fitPar.NodeGeneList.Count + weakPar.NodeGeneList.Count);
            foreach (var kvp in fitPar.NodeGeneList)
            {
                NodeGeneList.Add(kvp.Key, new NodeGene(kvp.Value));
            }

            // correlate connection genes:
            // if present in both: choose the weight randomly from one of the parent
            // if present in the fittest parent: just use it
            // if present in the weakest parent: add it to the disjoint-excess list
            var connList1 = fitPar.ConnectionGeneList;
            var connList2 = weakPar.ConnectionGeneList;
            var list1Count = connList1.Count;
            var list2Count = connList2.Count;
            ConnectionGeneList = new SortedList<uint, ConnectionGene>(list1Count + list2Count);
            var disjExcList = new List<KeyValuePair<uint, ConnectionGene>>(list2Count); // genes in weakPar that don't match in fitPar
            var recombDisjExc = rand.Next() < DISJ_EXC_RECOMB_PROB;

            var idx1 = 0;
            var idx2 = 0;
            uint ID1, ID2;
            ConnectionGene conn1, conn2;
            while (idx1 == list1Count && idx2 == list2Count)
            {
                ID1 = connList1.Keys[idx1];
                ID2 = connList2.Keys[idx2];
                conn1 = connList1.Values[idx1];
                conn2 = connList2.Values[idx2];

                if (ID1 == ID2)
                {
                    ConnectionGeneList.Add(ID1, new ConnectionGene(randBool.Next() ? conn1 : conn2));
                    idx1++;
                    idx2++;
                }
                else if (ID1 < ID2) // then beacause in fitPar there's a gene that don't match in weakPar
                {
                    ConnectionGeneList.Add(ID1, new ConnectionGene(conn1));
                    idx1++;
                }
                else // ID1 > ID2
                {
                    if (recombDisjExc)
                    {
                        disjExcList.Add(new KeyValuePair<uint, ConnectionGene>(ID2, conn2));
                    }
                    idx2++;
                }

                if (idx2 == list2Count)
                {
                    while (idx1 < list1Count)
                    {
                        idx1++;
                        ConnectionGeneList.Add(connList1.Keys[idx1], new ConnectionGene(connList1.Values[idx1]));
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
                            disjExcList.Add(new KeyValuePair<uint, ConnectionGene>(connList2.Keys[idx2], new ConnectionGene(connList2.Values[idx2])));
                        }
                    }
                    break;
                }
            }

            //TODO: this works (and allow evolution) but is missing code for adding disjoint-excess
            // (need an additional SortedDictionary<AddedConnection, uint?> filled with ConnectionGeneList genes)


            //mutate
            mutate();//consider calling this multiple times


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
                    addConnection();
                    break;
                case 3:
                    removeConnection();
                    break;
                case 4:
                    //do nothing
                    break;
            }
        }

        void mutateWeight()//sharpneat's code for weight mutation was extremely long and redundant with lot of wasted r.n.g.
        {
            var n = (int)Math.Ceiling(WEIGHT_MUT_PROP * ConnectionGeneList.Count);
            for (int i = 0; i < n; i++)
            {
                var m = rand.Next(n);

                var weight = ConnectionGeneList.Values[m].Weight + 2 * (float)rand.NextDouble() * MAX_WEIGHT_PERT_PROP - MAX_WEIGHT_PERT_PROP;
                ConnectionGeneList.Values[m].Weight = (weight < WEIGHT_RANGE ? (weight > -WEIGHT_RANGE ? weight : -WEIGHT_RANGE) : WEIGHT_RANGE);
            }
        }


        #region addNode
        /// <summary>
        /// Replace a connection gene with a node gene and two connection genes, in a way to achieve a similiar behaviour
        /// </summary>
        void addNode()
        {
            var idx = rand.Next(ConnectionGeneList.Count);// when picking items in a sortedList, the index mustn't be confused with the key (that is the ID)
            var oldConnID = ConnectionGeneList.Keys[idx];//   blabla[n] -> pick by key/ID       blabla.Values[n] -> pick by index
            var oldConn = ConnectionGeneList.Values[idx];
            ConnectionGeneList.Remove(oldConnID);

            var addedNode = getAddedNode(oldConnID);

            var newNode = new NodeGene();// NodeType set to hidden
            NodeGeneList.Add(addedNode.NodeID, newNode);
            //TODO: recheck the following approach
            //input connection with oldConn weight
            ConnectionGeneList.Add(addedNode.InpConnID, new ConnectionGene(oldConn.SourceID, addedNode.NodeID, oldConn.Weight));
            //output connection with max weight
            ConnectionGeneList.Add(addedNode.OutpConnID, new ConnectionGene(addedNode.NodeID, oldConn.TargetID, WEIGHT_RANGE));

            var srcNode = NodeGeneList[oldConn.SourceID];//search by key -> binary search
            srcNode.TgtNodeIDs.Remove(oldConn.SourceID);
            srcNode.TgtNodeIDs.Add(addedNode.NodeID);
            newNode.SrcNodeIDs.Add(oldConn.SourceID);

            var tgtNode = NodeGeneList[oldConn.TargetID];//search by key -> binary search
            tgtNode.SrcNodeIDs.Remove(oldConn.SourceID);
            tgtNode.SrcNodeIDs.Add(addedNode.NodeID);
            newNode.TgtNodeIDs.Add(oldConn.TargetID);

        }

        /// <summary>
        /// Returns an AddedNode struct with the right IDs and, if necessary, save the new struct in the global buffer
        /// </summary>
        AddedNode getAddedNode(uint oldConnID)//tried to shrink sharpneat code but didn't succeed
        {
            AddedNode addedNode;
            var isNewAddedNode = false;
            if (nodeBuffer.TryGetValue(oldConnID, out addedNode))
            {
                if (!(NodeGeneList.ContainsKey(addedNode.NodeID) || ConnectionGeneList.ContainsKey(addedNode.InpConnID) || ConnectionGeneList.ContainsKey(addedNode.OutpConnID)))
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
                nodeBuffer.Enqueue(oldConnID, addedNode);
            }
            return addedNode;
        }
        #endregion

        /// <summary>
        /// Allowed recursive connection and two way connections. After 5 search attempts, mutate weight.
        /// </summary>
        void addConnection()
        {
            var nodeCount = NodeGeneList.Count;

            for (int i = 0; i < 5; i++)
            {
                var srcIdx = rand.Next(nodeCount);
                var srcID = NodeGeneList.Keys[srcIdx];
                var srcNode = NodeGeneList.Values[srcIdx];//use index instead if ID for faster search

                var tgtIdx = INPUTS_AND_BIAS_COUNT + rand.Next(nodeCount - INPUTS_AND_BIAS_COUNT);// input and bias nodes can't be targets
                var tgtID = NodeGeneList.Keys[tgtIdx];
                var tgtNode = NodeGeneList.Values[tgtIdx];
                //if srcID == tgtID then the connection is recursive

                if (!srcNode.TgtNodeIDs.Contains(tgtID))
                {
                    var addedConn = new AddedConnection(srcID, tgtID);
                    uint? existingConnID;// must be nullable, to handle the case there isn't an existing ID
                    var newConn = new ConnectionGene(srcID, tgtID, ((float)rand.NextDouble() * 2f - 1f) * WEIGHT_RANGE);

                    if (connectionBuffer.TryGetValue(addedConn, out existingConnID))
                    {
                        ConnectionGeneList.Add(existingConnID.Value, newConn);
                    }
                    else
                    {
                        ConnectionGeneList.Add(++lastID, newConn);
                        connectionBuffer.Enqueue(addedConn, lastID);//same ID
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
        /// Remove a connection. If there's only one connection, then mutate weight.
        /// </summary>
        void removeConnection()
        {
            if (ConnectionGeneList.Count == 1)
            {
                mutateWeight();
                return;
            }

            var idx = rand.Next(ConnectionGeneList.Count);
            var oldConn = ConnectionGeneList.Values[idx];
            ConnectionGeneList.RemoveAt(idx);

            //-------------Remove unnecessary nodes------------
            //source node
            idx = NodeGeneList.IndexOfKey(oldConn.SourceID);
            var srcNode = NodeGeneList.Values[idx];
            srcNode.TgtNodeIDs.Remove(oldConn.TargetID);

            if (srcNode.IsRedundant) NodeGeneList.RemoveAt(idx);

            //target node
            idx = NodeGeneList.IndexOfKey(oldConn.TargetID);
            var tgtNode = NodeGeneList.Values[idx];
            tgtNode.SrcNodeIDs.Remove(oldConn.TargetID);

            if (tgtNode.IsRedundant) NodeGeneList.RemoveAt(idx);

        }
    }
}
