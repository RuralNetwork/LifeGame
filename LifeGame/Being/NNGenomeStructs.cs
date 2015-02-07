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
        public HashSet<uint> SrcNodeIDs { get; private set; }
        public HashSet<uint> TgtNodeIDs { get; private set; }

        public NodeGene()
        {
            Type = NodeType.Hidden;
            SrcNodeIDs = new HashSet<uint>();
            TgtNodeIDs = new HashSet<uint>();
        }

        public NodeGene(NodeGene nodeGene)
        {
            Type = nodeGene.Type;
            SrcNodeIDs = new HashSet<uint>();
            foreach (var id in nodeGene.SrcNodeIDs)
            {
                SrcNodeIDs.Add(id);
            }
            TgtNodeIDs = new HashSet<uint>();
            foreach (var id in nodeGene.TgtNodeIDs)
            {
                TgtNodeIDs.Add(id);
            }
        }

        public NodeGene(NodeType type)
        {
            Type = type;
            SrcNodeIDs = new HashSet<uint>();
            TgtNodeIDs = new HashSet<uint>();
        }

        public bool IsRedundant
        {
            get
            {
                return Type == NodeType.Hidden && SrcNodeIDs.Count + TgtNodeIDs.Count == 0;
            }

        }
    }

    public class LinkGene// switched back to class
    {
        public uint SourceID { get; private set; }
        public uint TargetID { get; private set; }
        public float Weight { get; set; }

        public LinkGene(uint sourceId, uint targetId, float weight)
        {
            SourceID = sourceId;
            TargetID = targetId;
            Weight = weight;
        }

        public LinkGene(LinkGene linkGene)
        {
            SourceID = linkGene.SourceID;
            TargetID = linkGene.TargetID;
            Weight = linkGene.Weight;
        }
    }

    public struct AddedNode
    {
        public uint NodeID { get; private set; }
        public uint InpLinkID { get; private set; }
        public uint OutpLinkID { get; private set; }

        public AddedNode(ref uint lastID)
            : this()
        {
            NodeID = ++lastID;
            InpLinkID = ++lastID;
            OutpLinkID = ++lastID;
        }
    }

    public struct AddedLink
    {
        public uint SourceID { get; private set; }
        public uint TargetID { get; private set; }

        public AddedLink(uint sourceId, uint targetId)
            : this()
        {
            SourceID = sourceId;
            TargetID = targetId;
        }
    }
}
