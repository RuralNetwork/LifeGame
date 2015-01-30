
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
        const int INPUTS_AND_BIAS_COUNT = 135 + 1;
        const int OUTPUTS_COUNT = 19;

        // parameters
        public static float WEIGHT_RANGE = 5.0f;
        public static float DISJ_EXC_RECOMB_PROP = 0.1f;// disjoint-excess recombination proportion
        public static float WEIGHT_MUT_PROP = 0.02f;    // weight mutation proportion
        public static float MAX_WEIGHT_PERT_PROP = 0.4f;// maximum weight perturbation proportion

        // mutation probalility coefficients
        public static float UNCHANGED_MUT_PROB = 10f;   // do nothing probability
        public static float WEIGHT_MUT_PROB = 0.988f;   // weight mutation probability
        public static float ADD_NODE_MUT_PROB = 0.01f;  // add node mutation probability
        public static float ADD_CONN_MUT_PROB = 0.01f;  // add connection mutation probability
        public static float DEL_CONN_MUT_PROB = 0.01f;  // delete connection mutation probability

        RouletteWheel mutationRW = new RouletteWheel(UNCHANGED_MUT_PROB, WEIGHT_MUT_PROB, ADD_NODE_MUT_PROB, ADD_CONN_MUT_PROB, DEL_CONN_MUT_PROB);

        static Random rand = new Random();

        //Being specific genomes
        public SortedList<uint, NodeGene> NodeGeneList { get; set; }
        public SortedList<uint, ConnectionGene> ConnectionGeneList { get; set; }


        //--------Globally store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure:
        //Connections: based on source & target ID, not on weight
        //Nodes: based on connection they replaced

        //ID counter for both connectionBuffer and nodeBuffer
        static uint lastID;

        //The key is the connection ID the AddedNode struct replaced
        //The ID is contained in the AddedNode sruct
        static SortedList<uint, AddedNode> nodeBuffer = new SortedList<uint, AddedNode>();

        //The value is the actual ID
        static SortedList<AddedConnection, uint?> connectionBuffer = new SortedList<AddedConnection, uint?>();

        public NNGenome()
        {

        }

        public NNGenome(NNGenome parent1, NNGenome parent2)
        {
            //do the cross-over (actually don't know, it's though with the ogranization of genes)

        }

        void mutate()
        {
            switch (mutationRW.Spin())
            {
                case 0:
                    //do nothing
                    break;
                case 1:
                    mutateWeight();
                    break;
                case 2:
                    addNode();
                    break;
                case 3:
                    addConnection();
                    break;
                case 4:
                    removeConnection();
                    break;
            }
        }

        void mutateWeight()//sharpneat's code for weight mutation was extremely long and redundant with lot of wasted r.n.g.
        {
            var n = (int)Math.Ceiling(WEIGHT_MUT_PROB * ConnectionGeneList.Count);
            for (int i = 0; i < n; i++)
            {
                var m = rand.Next(n);
                var weight = ConnectionGeneList.Values[m].Weight + 2 * (float)rand.NextDouble() * MAX_WEIGHT_PERT_PROP - MAX_WEIGHT_PERT_PROP;
                var conn = ConnectionGeneList.Values[m];
                conn.Weight = (weight < WEIGHT_RANGE ? (weight > -WEIGHT_RANGE ? weight : -WEIGHT_RANGE) : WEIGHT_RANGE);
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
            NodeGeneList.Add(addedNode.Node, newNode);
            //TODO: recheck the following approach
            //input connection with oldConn weight
            ConnectionGeneList.Add(addedNode.InputConn, new ConnectionGene(oldConn.Source, addedNode.Node, oldConn.Weight));
            //output connection with max weight
            ConnectionGeneList.Add(addedNode.OutputConn, new ConnectionGene(addedNode.Node, oldConn.Target, WEIGHT_RANGE));

            var srcNode = NodeGeneList[oldConn.Source];//search by key -> binary search
            srcNode.TargetNodes.Remove(oldConn.Source);
            srcNode.TargetNodes.Add(addedNode.Node);
            newNode.SourceNodes.Add(oldConn.Source);

            var tgtNode = NodeGeneList[oldConn.Target];//search by key -> binary search
            tgtNode.SourceNodes.Remove(oldConn.Source);
            tgtNode.SourceNodes.Add(addedNode.Node);
            newNode.TargetNodes.Add(oldConn.Target);

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
                if (!(NodeGeneList.ContainsKey(addedNode.Node) || ConnectionGeneList.ContainsKey(addedNode.InputConn) || ConnectionGeneList.ContainsKey(addedNode.OutputConn)))
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
                nodeBuffer.Add(oldConnID, addedNode);
            }
            return addedNode;
        }
        #endregion

        /// <summary>
        /// Allowed recursive connection and two way connections. After 5 finding attempts, mutate weight.
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

                if (!srcNode.TargetNodes.Contains(tgtID))
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
                        connectionBuffer.Add(addedConn, lastID);//same ID
                    }
                    srcNode.TargetNodes.Add(tgtID);
                    tgtNode.SourceNodes.Add(srcID);

                    return;
                }
            }
            //else
            mutateWeight();
        }

        /// <summary>
        /// Remove a connection. If there's only one connection, mutate weight of it.
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
            idx = NodeGeneList.IndexOfKey(oldConn.Source);
            var srcNode = NodeGeneList.Values[idx];
            srcNode.TargetNodes.Remove(oldConn.Target);

            if (srcNode.IsRedundant) NodeGeneList.RemoveAt(idx);

            //target node
            idx = NodeGeneList.IndexOfKey(oldConn.Target);
            var tgtNode = NodeGeneList.Values[idx];
            tgtNode.SourceNodes.Remove(oldConn.Target);

            if (tgtNode.IsRedundant) NodeGeneList.RemoveAt(idx);

        }

        void defragIDs()
        {

        }
    }
}
