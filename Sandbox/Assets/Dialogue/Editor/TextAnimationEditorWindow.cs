using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TextAnimationEditorWindow : EditorWindow {
    private TextAnimationGraphView graphView;

    [MenuItem("Window/TextAnimationEditorWindow")]
    public static void ShowExample() {
        TextAnimationEditorWindow wnd = GetWindow<TextAnimationEditorWindow>();
        wnd.titleContent = new GUIContent("TextAnimationEditorWindow");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Dialogue/Editor/TextAnimationEditorWindow.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dialogue/Editor/TextAnimationEditorWindow.uss");
        root.styleSheets.Add(styleSheet);

        graphView = root.Q<TextAnimationGraphView>();
        graphView.CreateSearchWindow(this);
    }
}