using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public class ConnectionGene
    {
        public int InnovationID { get; set; }
        public int Source { get; set; }
        public int Target { get; set; }
        public float Weight { get; set; }
        public bool IsMutated { get; set; }

    }

    public enum NodeType
    {
        Bias,
        Input,
        Output,
        Hidden
    }

    public class NodeGene
    {
        public NodeType Type { get; private set; }
    }

    public class NNGenome
    {
        ///// Parameters
        public static float WeigthRange = 5.0f;
        public static float DisjointExcessRecombProb = 0.1f;
        //mutation probalility coefficients
        public static float WeightProb = 0.988f;
        public static float AddNodeProb = 0.01f;
        public static float AddConnectionProb = 0.01f;
        public static float DeleteConnectionProb = 0.01f;


        //SortedList
        public NNGenome(NNGenome parent1, NNGenome parent2)
        {
            //do the cross-over (actually don't know, it's though with the ogranization of genes)
        }

        void mutate()
        {

        }

        void mutateWeigth()
        {

        }

        void addNode()
        {

        }

        void addConnection()
        {

        }

        void removeConnection()
        {

        }

        public void BrainWash() // A way to simplify a neural network maintaining similar behaviour. Should not be called in a regular simulation
        {
            //remove dead ends (nodes with no output connections)
            //remove weak connections (weigth too small)
        }
    }
}
