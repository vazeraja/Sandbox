using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

namespace Aarthificial.Reanimation.Common {
    
    [Serializable]
    public class NodeLinkData {
        public string BaseNodeGUID;
        public string TargetNodeGUID;
    }
    [Serializable]
    public class ReanimatorNodeData {
        public ReanimatorNode ReanimatorNode;
        public string NodeGUID;
        public Vector2 Position;
    }
    [Serializable]
    public class SaveData {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<ReanimatorNodeData> ReanimatorNodeData = new List<ReanimatorNodeData>();
        public List<GroupBlock> CommentBlockData = new List<GroupBlock>();
    }
}