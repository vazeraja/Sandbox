using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TextAnimationSearchWindowProvider : ScriptableObject, ISearchWindowProvider {
    private EditorWindow window;
    private TextAnimationGraphView graphView;

    private Texture2D indentationIcon;

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) {
        var searchOptions = new List<SearchTreeEntry> {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            new SearchTreeEntry(new GUIContent("Simple Text Animation", indentationIcon)) {
                level = 1,
                userData = "simple text animation"
            },
        };

        return searchOptions;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) {
        var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
            context.screenMousePosition - window.position.position);
        var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

        switch (SearchTreeEntry.userData) {
            case "simple text animation":
                graphView.CreateNode(graphMousePosition);
                return true;
            default:
                break;
        }

        return false;
    }

    public void Initialize(EditorWindow window, TextAnimationGraphView graphView) {
        this.window = window;
        this.graphView = graphView;

        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        indentationIcon.Apply();
    }
}