using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public struct ConnectionGene
    {
        public uint Source { get; set; }
        public uint Target { get; set; }
        public float Weight { get; set; }
        public bool IsMutated { get; set; }

        public ConnectionGene(uint source, uint target, float weight)
            : this()
        {
            Source = source;
            Target = target;
            Weight = weight;
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
        public HashSet<uint> SourceNodes { get; set; }
        public HashSet<uint> TargetNodes { get; set; }

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
        public uint Source { get; set; }
        public uint Target { get; set; }
    }

    public struct AddedNode
    {
        public uint Node { get; private set; }
        public uint InputConn { get; private set; }
        public uint OutputConn { get; private set; }

        public AddedNode(ref uint lastID)
            : this()
        {
            Node = ++lastID;
            InputConn = ++lastID;
            OutputConn = ++lastID;
        }

    }
}
