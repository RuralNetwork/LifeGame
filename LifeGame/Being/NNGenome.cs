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
        RouletteWeel mutationRW = new RouletteWeel(UnchangedProb, WeightProb, AddNodeProb, AddConnectionProb, DeleteConnectionProb);

        static Random rand = new Random();

        public SortedList<int, ConnectionGene> ConnectionGeneList { get; set; }
        public SortedList<int, NodeGene> NodeGeneList { get; set; }

        //Store the identifiers needed to match genes during recombination
        static SortedList<AddedConnection, int?> connectionBuffer { get; set; }
        static SortedList<AddedNode, int?> nodeBuffer { get; set; }


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

        void addNode()
        {
            var n = rand.Next(ConnectionGeneList.Count);
            var oldConn = ConnectionGeneList[n];

        }

        void addConnection()
        {

        }

        bool removeConnection()
        {
            if (ConnectionGeneList.Count > 2)
            {

                return true;
            }
            return false;
        }

        //what i called "brainwash" was already present in sharpneat in another way
    }
}
