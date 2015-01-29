
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

        ///// Parameters
        public static float WeightRange = 5.0f;
        public static float DisjointExcessRecombProb = 0.1f;
        //mutation probalility coefficients
        public static float UnchangedProb = 10f;
        public static float WeightProb = 0.988f;
        public static float AddNodeProb = 0.01f;
        public static float AddConnectionProb = 0.01f;
        public static float DeleteConnectionProb = 0.01f;
        RouletteWheel mutationRW = new RouletteWheel(UnchangedProb, WeightProb, AddNodeProb, AddConnectionProb, DeleteConnectionProb);

        static Random rand = new Random();

        //Being specific genomes
        public SortedList<uint, ConnectionGene> ConnectionGeneList { get; set; }
        public SortedList<uint, NodeGene> NodeGeneList { get; set; }


        //--------Globally store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure:
        //Connections: based on source & target ID, not on weight
        //Nodes: based on connection they replaced

        //ID counter for both connectionBuffer and nodeBuffer
        static uint lastID;

        //The value is the actual ID
        static SortedList<AddedConnection, uint?> connectionBuffer;

        //The key is the connection ID the AddedNode struct replaced
        //The ID is contained in the AddedNode sruct
        static SortedList<uint, AddedNode> nodeBuffer;


        public NNGenome()
        {

        }

        public NNGenome(NNGenome parent1, NNGenome parent2)
        {
            //do the cross-over (actually don't know, it's though with the ogranization of genes)

        }

        void mutate()
        {
            var success = false;
            while (!success)
            {
                switch (mutationRW.Spin())
                {
                    case 0:
                        break;
                    case 1:

                        mutateWeight();
                        break;

                }
            }

        }

        void mutateWeight()
        {

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

            var newNode = new NodeGene() { Type = NodeType.Hidden };
            NodeGeneList.Add(addedNode.Node, newNode);
            //TODO: recheck the following approach
            //input connection with oldConn weight
            ConnectionGeneList.Add(addedNode.InputConn, new ConnectionGene(oldConn.Source, addedNode.Node, oldConn.Weight));
            //output connection with max weight
            ConnectionGeneList.Add(addedNode.OutputConn, new ConnectionGene(addedNode.Node, oldConn.Target, WeightRange));

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
                var srcNode = NodeGeneList.Values[srcIdx];

                var tgtIdx = INPUTS_AND_BIAS_COUNT + rand.Next(nodeCount - INPUTS_AND_BIAS_COUNT);
                var tgtID = NodeGeneList.Keys[tgtIdx];
                var tgtNode = NodeGeneList.Values[tgtIdx];
                //if srcID == tgt ID then the connection is recursive

                if (!srcNode.TargetNodes.Contains(tgtID))
                {
                    var addedConn = new AddedConnection() { Source = srcID, Target = tgtID };
                    uint? existingConnID;// must be nullable, to handle the case there isn't an existing ID
                    var newConn = new ConnectionGene(srcID, tgtID, ((float)rand.NextDouble() * 2f - 1f) * WeightRange);

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
        }

        bool removeConnection()
        {
            if (ConnectionGeneList.Count > 1)
            {

                return true;
            }
            return false;
        }

        void defragIDs()
        {

        }

        //what i called "brainwash" was already present in sharpneat in another way
    }
}
