using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Aarthificial.Reanimation {
    [CreateAssetMenu(fileName = "ResolutionGraph", menuName = "Reanimator/ResolutionGraph", order = 400)]
    public class ResolutionGraph : ScriptableObject {
        public ReanimatorNode root;
        public List<ReanimatorNode> nodes = new List<ReanimatorNode>();
        public List<Group> groups = new List<Group>();
        
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