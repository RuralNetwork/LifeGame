using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public struct ConnectionGene
    {
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

    public struct NodeGene
    {
        public NodeType Type { get; private set; }
        public HashSet<int> Sources { get; set; }
        public HashSet<int> Targets { get; set; }

        public bool IsRedundant
        {
            get
            {
                return Type == NodeType.Hidden && Sources.Count + Targets.Count == 0;
            }

        }
    }

    public struct AddedConnection
    {
        public int Source { get; private set; }
        public int Target { get; private set; }
    }

    public struct AddedNode
    {
        public int ID { get; private set; }
        public int InputConnID { get; private set; }
        public int OutputConnID { get; private set; }

    }
}
