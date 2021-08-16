using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<FloatingElement> floatingElements = new List<FloatingElement>();
        //[NonSerialized] public Dictionary<string, BaseNode> nodesPerGUID = new Dictionary<string, BaseNode>();
        
        
        /// <summary>
        /// Creates a scriptable object sub asset for the current resolution graph and adds it to the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public ReanimatorNode CreateSubAsset(Type type, string assetName = null)
        {
            ReanimatorNode node = ScriptableObject.CreateInstance(type) as ReanimatorNode;

            // ReSharper disable once PossibleNullReferenceException
            node.name = string.IsNullOrEmpty(assetName) ? type.Name : assetName;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Resolution Tree");
            nodes.Add(node);
            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Resolution Tree");

            EditorUtility.SetDirty(this);
            return node;
        }

        /// <summary>
        /// Removes the scriptable object sub asset from the resolution graph and removes it from the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="node"></param>
        public void DeleteSubAsset(ReanimatorNode node)
        {
            Undo.RecordObject(this, "Resolution Tree");
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Adds appropriate child node(s) when an edge or node is created
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void AddChild(ReanimatorNode parent, ReanimatorNode child)
        {
            switch (parent) {
                case BaseNode rootNode:
                    Undo.RecordObject(rootNode, "Resolution Tree");
                    root = child;
                    rootNode.root = child;
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = child;
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Add(child);
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(switchNode);
                    break;
            }
        }

        /// <summary>
        /// Removes appropriate child node(s) when an edge or node is deleted
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChild(ReanimatorNode parent, ReanimatorNode child)
        {
            switch (parent) {
                case BaseNode rootNode:
                    Undo.RecordObject(rootNode, "Resolution Tree");
                    root = null;
                    rootNode.root = null;
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = null;
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Remove(child);
                    EditorUtility.SetDirty(this);
                    EditorUtility.SetDirty(switchNode);
                    break;
            }
        }

        
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