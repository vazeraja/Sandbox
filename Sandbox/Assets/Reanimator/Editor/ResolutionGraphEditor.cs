
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation {
    
    [CustomEditor(typeof(ResolutionGraph))]
    public class ResolutionGraphEditor : UnityEditor.Editor {
        private SerializedProperty _root;
        private SerializedProperty _nodes;
        private SerializedProperty _currentTrace;
        private SerializedProperty _SaveData;
        
        private SerializedProperty _reanimatorNodeData;
        private SerializedProperty _nodeLinks;
        private SerializedProperty _commentBlockData;
        
        private ResolutionGraph _resolutionGraph;

        private void OnEnable()
        {
            _root = serializedObject.FindProperty(nameof(ResolutionGraph.root));
            _nodes = serializedObject.FindProperty(nameof(ResolutionGraph.nodes));
            _currentTrace = serializedObject.FindProperty(nameof(ResolutionGraph.currentTrace));
            _SaveData = serializedObject.FindProperty(nameof(ResolutionGraph.saveData));

            _reanimatorNodeData = _SaveData.FindPropertyRelative("ReanimatorNodeData");
            _nodeLinks = _SaveData.FindPropertyRelative("NodeLinks");
            _commentBlockData = _SaveData.FindPropertyRelative("CommentBlockData");
            
            _resolutionGraph = (ResolutionGraph)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _resolutionGraph = (ResolutionGraph)serializedObject.targetObject;
            
            using (new EditorGUI.DisabledGroupScope(true)) {
                EditorGUILayout.PropertyField(_root);
                EditorGUILayout.PropertyField(_nodes);
                EditorGUILayout.PropertyField(_currentTrace);
            }
            
            EditorGUILayout.LabelField("Save Data");
            using (new EditorGUI.IndentLevelScope())
            {
                using (new EditorGUI.DisabledGroupScope(true)) {
                    EditorGUILayout.PropertyField(_reanimatorNodeData);
                    EditorGUILayout.PropertyField(_nodeLinks);
                    EditorGUILayout.PropertyField(_commentBlockData);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}