using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aarthificial.Reanimation {
    
    [Serializable]
    public struct NodeDTO {
        public ReanimatorNode node;
        public string guid;
        public string title;
        public Vector2 position;
        public bool needsAnimationPreview;
    }
    
    [CreateAssetMenu(fileName = "ResolutionGraph", menuName = "Reanimator/ResolutionGraph", order = 400)]
    public class ResolutionGraph : ScriptableObject {
        [SerializeField] public ReanimatorNode root;
        [SerializeField] public List<NodeDTO> nodes = new List<NodeDTO>();
        [SerializeField] public List<Group> groups = new List<Group>();
        
        
        //[NonSerialized] public Dictionary<string, BaseNode> nodesPerGUID = new Dictionary<string, BaseNode>();

        /// <summary>
        /// Add a group
        /// </summary>
        /// <param name="block"></param>
        public void AddGroup(Group block)
        {
            groups.Add(block);
        }

        /// <summary>
        /// Removes a group
        /// </summary>
        /// <param name="block"></param>
        public void RemoveGroup(Group block)
        {
            groups.Remove(block);
        }
    }
}