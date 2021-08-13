using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aarthificial.Reanimation {
    [CreateAssetMenu(fileName = "ResolutionGraph", menuName = "Reanimator/ResolutionGraph", order = 400)]
    public class ResolutionGraph : ScriptableObject {
        public ReanimatorNode root;
        public List<ReanimatorNode> nodes = new List<ReanimatorNode>();
        
        public List<Group> groups = new List<Group>();
        [ItemCanBeNull] public List<FloatingElement> floatingElements = new List<FloatingElement>();
        //[NonSerialized] public Dictionary<string, BaseNode> nodesPerGUID = new Dictionary<string, BaseNode>();
        

        
        public void AddGroup(Group block)
        {
            groups.Add(block);
        }
        public void RemoveGroup(Group block)
        {
            groups.Remove(block);
        }
        public void AddFloatingElement(FloatingElement element)
        {
            floatingElements.Add(element);
        }
        public void RemoveFloatingElement(FloatingElement element)
        {
            floatingElements.Remove(element);
        }
    }
}