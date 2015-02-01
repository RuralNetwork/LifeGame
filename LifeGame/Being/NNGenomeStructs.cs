using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    public enum NodeType
    {
        Bias,
        Input,
        Output,
        Hidden
    }

    public class NodeGene// switched back to class because it is not immutable and gave problems with references
    {
        public NodeType Type { get; private set; }
        public HashSet<uint> SourceNodes { get; private set; }
        public HashSet<uint> TargetNodes { get; private set; }

        public NodeGene()
        {
            Type = NodeType.Hidden;
            SourceNodes = new HashSet<uint>();
            TargetNodes = new HashSet<uint>();
        }
        public NodeGene(NodeType type)
        {
            Type = type;
            SourceNodes = new HashSet<uint>();
            TargetNodes = new HashSet<uint>();
        }

        public bool IsRedundant
        {
            get
            {
                return Type == NodeType.Hidden && SourceNodes.Count + TargetNodes.Count == 0;
            }

        }
    }
    public class ConnectionGene// switched back to class
    {
        public uint Source { get; private set; }
        public uint Target { get; private set; }
        public float Weight { get; set; }

        public ConnectionGene(uint source, uint target, float weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }

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

    public struct AddedConnection
    {
        public uint Source { get; private set; }
        public uint Target { get; private set; }

        public AddedConnection(uint source, uint target)
            : this()
        {
            Source = source;
            Target = target;
        }
    }
}
