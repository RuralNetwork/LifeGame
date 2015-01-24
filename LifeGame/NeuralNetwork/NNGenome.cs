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
        NodeType Type { get; private set; }
    }

    public class NNGenome
    {
        ///// Parameters
        static float WeigthRange = 5.0f;
        static float DisjointExcessRecombProb = 0.1f;
        //mutation probalility coefficients
        static float WeightProb = 0.988f;
        static float AddNodeProb = 0.01f;
        static float AddConnectionProb = 0.01f;
        static float AddConnectionProb = 0.01f;


        //SortedList
        public NNGenome(NNGenome parent1, NNGenome parent2)
        {
            //do the cross-over (actually don't know, it's though with the ogranization of genes)
        }

        NeuralNetwork GenerateNeuralNetwork()
        {
            //generate the network from the dominant phenotypes
            return null;
        }

         void Mutate()
        {

        }

        void MutateWeigth()
        {

        }

        void AddNode()
        {

        }

        void AddConnection()
        {

        }

        void RemoveConnection()
        {

        }

        public void BrainWash() // A way to simplify a neural network maintaining similar behaviour. Should not be called in a regular simulation
        {
            //remove dead ends (nodes with no output connections)
            //remove weak connections (weigth too small)
        }
    }
}
