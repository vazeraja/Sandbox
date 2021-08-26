
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Aarthificial.Reanimation {
    
    [CustomEditor(typeof(ResolutionGraph))]
    public class ResolutionGraphEditor : UnityEditor.Editor {
        private SerializedProperty _root;
        private SerializedProperty _nodes;
        private SerializedProperty _groups;
        private SerializedProperty _floatingElements;
        private SerializedProperty _edges;
        private SerializedProperty _copyPasteHelper;
        
        private ResolutionGraph _resolutionGraph;

        private void OnEnable()
        {
            _root = serializedObject.FindProperty(nameof(ResolutionGraph.root));
            _nodes = serializedObject.FindProperty(nameof(ResolutionGraph.nodes));
            _groups = serializedObject.FindProperty(nameof(ResolutionGraph.groups));
            _edges = serializedObject.FindProperty(nameof(ResolutionGraph.edges));
            _copyPasteHelper = serializedObject.FindProperty(nameof(ResolutionGraph.CopyPasteHelper));
            _floatingElements = serializedObject.FindProperty(nameof(ResolutionGraph.floatingElements));

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
            EditorGUILayout.PropertyField(_edges);
            EditorGUILayout.PropertyField(_copyPasteHelper);
            EditorGUILayout.PropertyField(_floatingElements);

            serializedObject.ApplyModifiedProperties();
        }
    }
}