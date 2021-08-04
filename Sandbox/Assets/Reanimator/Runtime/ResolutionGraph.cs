using System.Collections.Generic;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aarthificial.Reanimation.ResolutionGraph {
    [CreateAssetMenu(fileName = "ResolutionGraph", menuName = "Reanimator/ResolutionGraph", order = 400)]
    public class ResolutionGraph : ScriptableObject {
        public ReanimatorNode root;

        public List<ReanimatorNode> nodes = new List<ReanimatorNode>();
        public List<ReanimatorNode> currentTrace = new List<ReanimatorNode>();

        public SaveData SaveData = new SaveData();
        
    }
}