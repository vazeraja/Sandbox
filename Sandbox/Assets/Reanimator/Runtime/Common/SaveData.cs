using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using Aarthificial.Reanimation.ResolutionGraph.Editor;
using UnityEngine;

namespace Aarthificial.Reanimation.ResolutionGraph {
    
    [Serializable]
    public class NodeLinkData {
        public string BaseNodeGUID;
        public string TargetNodeGUID;
    }
    [Serializable]
    public class ReanimatorNodeData {
        public string NodeGUID;
        public ReanimatorNode ReanimatorNode;
        public Vector2 Position;
    }
    [Serializable]
    public class SaveData {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<ReanimatorNodeData> ReanimatorNodeData = new List<ReanimatorNodeData>();
        public List<GroupBlock> CommentBlockData = new List<GroupBlock>();
    }
}