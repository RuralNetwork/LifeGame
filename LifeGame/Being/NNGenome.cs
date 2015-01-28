
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    public class NNGenome
    {
        ///// Parameters
        public static float WeigthRange = 5.0f;
        public static float DisjointExcessRecombProb = 0.1f;
        //mutation probalility coefficients
        public static float UnchangedProb = 10f;
        public static float WeightProb = 0.988f;
        public static float AddNodeProb = 0.01f;
        public static float AddConnectionProb = 0.01f;
        public static float DeleteConnectionProb = 0.01f;
        RouletteWheel mutationRW = new RouletteWheel(UnchangedProb, WeightProb, AddNodeProb, AddConnectionProb, DeleteConnectionProb);

        static Random rand = new Random();

        public SortedList<int, ConnectionGene> ConnectionGeneList { get; set; }
        public SortedList<int, NodeGene> NodeGeneList { get; set; }


        //--------Store the identifiers needed to match genes during recombination
        //The innovation IDs are assigned based on the structure, not on internal properties:
        //Connections: based on source & target ID, not on weigth
        //Nodes: based on connection they replaced, not on bias

        //The key is the actual innovation ID
        static SortedList<int, AddedConnection> connectionBuffer;

        //The key is the connection ID the AddedNode struct replaced
        //The ID is contained in the AddedNode sruct
        static SortedList<int, AddedNode> nodeBuffer;


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

                        mutateWeigth();
                        break;

                }
            }

        }

        void mutateWeigth()
        {

        }

        /// <summary>
        /// Replace a connection gene with a node gene and two connection genes, in a way to achieve a similiar behaviour
        /// </summary>
        void addNode()
        {
            var oldConnID = rand.Next(ConnectionGeneList.Count);
            //var oldConn = ConnectionGeneList[connID];
            ConnectionGeneList.Remove(oldConnID);

            AddedNode newStruct;
            if (nodeBuffer.TryGetValue(oldConnID, out newStruct) )
            {
                if (!(NodeGeneList.ContainsKey(newStruct.ID) || ConnectionGeneList.ContainsKey(newStruct.InputConnID) || ConnectionGeneList.ContainsKey(newStruct.OutputConnID)))
                {
                }
            }


        }


        void addConnection()
        {

        }

        bool removeConnection()
        {
            if (ConnectionGeneList.Count > 1)
            {

                return true;
            }
            return false;
        }

        //what i called "brainwash" was already present in sharpneat in another way
    }
}
