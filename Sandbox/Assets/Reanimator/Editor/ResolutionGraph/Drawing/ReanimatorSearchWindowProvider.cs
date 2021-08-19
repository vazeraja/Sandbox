using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Group = Aarthificial.Reanimation.Common.Group;

namespace Aarthificial.Reanimation {
    public class ReanimatorSearchWindowProvider : ScriptableObject, ISearchWindowProvider {
        
        private EditorWindow window;
        private ReanimatorGraphView graphView;

        private Texture2D indentationIcon;
        
        public void Initialize(EditorWindow window, ReanimatorGraphView graphView){
            this.window = window;
            this.graphView = graphView;
            
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchOptions = new List<SearchTreeEntry> {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                
                new SearchTreeGroupEntry(new GUIContent("Reanimator"), 1),
                new SearchTreeEntry(new GUIContent("Simple Animation", indentationIcon)) {
                    level = 2,
                    userData = CreateInstance<SimpleAnimationNode>()
                },
                new SearchTreeEntry(new GUIContent("Switch Node", indentationIcon)) {
                    level = 2,
                    userData = CreateInstance<SwitchNode>()
                },
                new SearchTreeEntry(new GUIContent("Override Node", indentationIcon)) {
                    level = 2,
                    userData = CreateInstance<OverrideNode>()
                },
                
                new SearchTreeGroupEntry(new GUIContent("Create Group"), 1),
                new SearchTreeEntry(new GUIContent("Group", indentationIcon)) {
                    level = 2,
                    userData = new Group()
                },
            };

            return searchOptions;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (graphView.graph == null) {
                EditorUtility.DisplayDialog("Invalid", "Resolution Graph has not been initialized yet", "OK");
                return false;
            }
            
            var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, 
                context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            
            switch (SearchTreeEntry.userData) {
                case SimpleAnimationNode simpleAnimationNode:
                    graphView.CreateNode(typeof(SimpleAnimationNode), graphMousePosition);
                    return true;
                case SwitchNode switchNode:
                    graphView.CreateNode(typeof(SwitchNode), graphMousePosition);
                    return true;
                case OverrideNode overrideNode:
                    graphView.CreateNode(typeof(OverrideNode), graphMousePosition);
                    return true;
                case Group group:
                    graphView.AddGroup(new Group("Group Block", graphMousePosition));
                    return true;
            }
            return false;
        }
    }
}