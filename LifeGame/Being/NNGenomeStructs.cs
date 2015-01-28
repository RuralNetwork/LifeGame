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

        public ConnectionGene(int source, int target, float weight)
        {

        }

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
        public NodeType Type { get; set; }
        public HashSet<int> SourceNodes { get; set; }
        public HashSet<int> TargetNodes { get; set; }

        public bool IsRedundant
        {
            get
            {
                return Type == NodeType.Hidden && SourceNodes.Count + TargetNodes.Count == 0;
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
        public int InputConn { get; private set; }
        public int OutputConn { get; private set; }

        public AddedNode(ref int lastID)
            : this()
        {
            ID = ++lastID;
            InputConn = ++lastID;
            OutputConn = ++lastID;
        }

    }
}
