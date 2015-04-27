using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//I renamed this file because I struct can be confused and structures is too long, I don't know, useless change, but whatever
namespace LifeGame
{
    [Serializable]
    public enum NodeType
    {
        Bias,
        Input,
        Output,
        Hidden
    }
    [Serializable]
    public class NodeGene// switched back to class because it is not immutable and gave problems with references
    {
        public NodeType Type { get; private set; }
        //Source node
        public HashSet<ulong> SrcNodeIDs { get; private set; }
        //Target node
        public HashSet<ulong> TgtNodeIDs { get; private set; }

        public NodeGene()
        {
            Type = NodeType.Hidden;
            SrcNodeIDs = new HashSet<ulong>();
            TgtNodeIDs = new HashSet<ulong>();
        }

        public NodeGene(NodeGene nodeGene)
        {
            Type = nodeGene.Type;
            SrcNodeIDs = new HashSet<ulong>();
            foreach (var id in nodeGene.SrcNodeIDs)
            {
                SrcNodeIDs.Add(id);
            }
            TgtNodeIDs = new HashSet<ulong>();
            foreach (var id in nodeGene.TgtNodeIDs)
            {
                TgtNodeIDs.Add(id);
            }
        }

        public NodeGene(NodeType type)
        {
            Type = type;
            SrcNodeIDs = new HashSet<ulong>();
            TgtNodeIDs = new HashSet<ulong>();
        }

        public bool IsRedundant
        {
            get
            {
                return Type == NodeType.Hidden && SrcNodeIDs.Count + TgtNodeIDs.Count == 0;
            }

        }
    }
    //Class that identifies one single link
    [Serializable]
    public class LinkGene
    {
        public ulong SourceID { get; private set; }
        public ulong TargetID { get; private set; }
        public float Weight { get; set; }

        public LinkGene(ulong sourceId, ulong targetId, float weight)
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

    [Serializable]
    public struct AddedNode
    {
        public ulong NodeID { get; private set; }
        public ulong InpLinkID { get; private set; }
        public ulong OutpLinkID { get; private set; }

        public AddedNode(ref ulong lastID)
            : this()
        {
            NodeID = ++lastID;
            InpLinkID = ++lastID;
            OutpLinkID = ++lastID;
        }
    }

    [Serializable]
    public struct AddedLink
    {
        public ulong SourceID { get; private set; }
        public ulong TargetID { get; private set; }

        public AddedLink(ulong sourceId, ulong targetId)
            : this()
        {
            SourceID = sourceId;
            TargetID = targetId;
        }
    }
}
