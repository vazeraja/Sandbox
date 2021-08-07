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
    public class GraphChanges {
        // public SerializableEdge removedEdge;
        // public SerializableEdge addedEdge;
        
        public ReanimatorNode removedNode;
        public ReanimatorNode addedNode;
        public ReanimatorNode nodeChanged;
        
        public Group addedGroups;
        public Group removedGroups;
        
        // public BaseStackNode	addedStackNode;
        // public BaseStackNode	removedStackNode;
        // public StickyNote addedStickyNotes;
        // public StickyNote removedStickyNotes;
    }

    [CreateAssetMenu(fileName = "ResolutionGraph", menuName = "Reanimator/ResolutionGraph", order = 400)]
    public class ResolutionGraph : ScriptableObject {
        public ReanimatorNode root;
        public List<ReanimatorNode> nodes = new List<ReanimatorNode>();
        [SerializeField] public List<Group> groups = new List<Group>();

        /// <summary>
        /// Triggered when the graph is changed
        /// </summary>
        public event Action<GraphChanges> onGraphChanges;

        /// <summary>
        /// Add a group
        /// </summary>
        /// <param name="block"></param>
        public void AddGroup(Group block)
        {
            groups.Add(block);
            onGraphChanges?.Invoke(new GraphChanges {addedGroups = block});
        }
        /// <summary>
        /// Removes a group
        /// </summary>
        /// <param name="block"></param>
        public void RemoveGroup(Group block)
        {
            groups.Remove(block);
            onGraphChanges?.Invoke(new GraphChanges{ removedGroups = block });
        }
    }
}