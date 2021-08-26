using System;
using Aarthificial.Reanimation;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

namespace Aarthificial.Reanimation.Common {
    [Serializable]
    public class EdgeData {
        public string guid;

        public ResolutionGraph owner;

        public ReanimatorNode baseNode;
        public string baseNodeGUID;
        public ReanimatorNode targetNode;
        public string targetNodeGUID;
        
    }
}