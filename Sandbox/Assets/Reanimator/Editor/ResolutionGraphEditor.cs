
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation {
    
    [CustomEditor(typeof(ResolutionGraph))]
    public class ResolutionGraphEditor : UnityEditor.Editor {
        private SerializedProperty _root;
        private SerializedProperty _nodes;
        private SerializedProperty _groups;

        private SerializedProperty _reanimatorNodeData;
        private SerializedProperty _nodeLinks;
        private SerializedProperty _commentBlockData;
        
        private ResolutionGraph _resolutionGraph;

        private void OnEnable()
        {
            _root = serializedObject.FindProperty(nameof(ResolutionGraph.root));
            _nodes = serializedObject.FindProperty(nameof(ResolutionGraph.nodes));
            _groups = serializedObject.FindProperty(nameof(ResolutionGraph.groups));

            _resolutionGraph = (ResolutionGraph)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _resolutionGraph = (ResolutionGraph)serializedObject.targetObject;
            
            using (new EditorGUI.DisabledGroupScope(true)) {
                EditorGUILayout.PropertyField(_root);
                EditorGUILayout.PropertyField(_nodes);
                EditorGUILayout.PropertyField(_groups);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}