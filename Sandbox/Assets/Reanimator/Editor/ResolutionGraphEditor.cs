
using UnityEditor;

namespace Aarthificial.Reanimation {
    
    [CustomEditor(typeof(ResolutionGraph))]
    public class ResolutionGraphEditor : UnityEditor.Editor {
        private SerializedProperty _root;
        private SerializedProperty _nodes;
        private SerializedProperty _currentTrace;
        private SerializedProperty _SaveData;

        private void OnEnable()
        {
            _root = serializedObject.FindProperty(nameof(ResolutionGraph.root));
            _nodes = serializedObject.FindProperty(nameof(ResolutionGraph.nodes));
            _currentTrace = serializedObject.FindProperty(nameof(ResolutionGraph.currentTrace));
            _SaveData = serializedObject.FindProperty(nameof(ResolutionGraph.saveData));
        }
    }
}