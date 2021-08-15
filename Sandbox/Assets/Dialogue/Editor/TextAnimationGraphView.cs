using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TextAnimationGraphView : GraphView {
    private readonly string styleSheetPath = "Assets/Dialogue/Editor/TextAnimationEditorWindow.uss";
    private TextAnimationSearchWindowProvider searchWindowProvider;
    private TextAnimationEditorWindow editorWindow;

    public new class UxmlFactory : UxmlFactory<TextAnimationGraphView, UxmlTraits> {
    }

    public void Initialize(TextAnimationEditorWindow editorWindow) {
        this.editorWindow = editorWindow;
        // graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        // graphViewChanged += OnGraphViewChanged;

        CreateSearchWindow(editorWindow);
    }

    public TextAnimationGraphView() {
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath));

        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public class TextAnimationNode : Node {
        public Vector2 Position;
        public string Guid;
    }


    public void CreateNode(Vector2 graphMousePosition) {
        var node = new TextAnimationNode() { Position = graphMousePosition, Guid = Guid.NewGuid().ToString() };
        AddElement(node);
    }

    public void CreateSearchWindow(EditorWindow window) {
        searchWindowProvider = ScriptableObject.CreateInstance<TextAnimationSearchWindowProvider>();
        searchWindowProvider.Initialize(window, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
    }
}