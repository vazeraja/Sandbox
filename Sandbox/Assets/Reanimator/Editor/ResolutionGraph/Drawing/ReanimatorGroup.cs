using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Group = Aarthificial.Reanimation.Common.Group;

namespace Aarthificial.Reanimation {
    public sealed class ReanimatorGroup : UnityEditor.Experimental.GraphView.Group {
        public Group group;
        private ReanimatorGraphView graphView;

        private Label titleLabel;
        private ColorField colorField;

        private readonly string groupStyle = "ReanimatorGroup";

        public ReanimatorGroup(ReanimatorGraphView graphView, Group group)
        {
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{groupStyle}"));

            this.group = group;
            this.graphView = graphView;
            title = group.title;
            autoUpdateGeometry = true;
            
            SetPosition(group.position);
            UpdateGroupColor(group.color);

            headerContainer.Q<TextField>().RegisterCallback<ChangeEvent<string>>(TitleChangedCallback);
            headerContainer.Q<Label>().style.color = new Color(1f, 1f, 1f, 0.61f);

            colorField = new ColorField {
                value = group.color, 
                name = "headerColorPicker"
            };
            colorField.RegisterValueChangedCallback(e => { UpdateGroupColor(e.newValue); });
            headerContainer.Add(colorField);

            InitializeInnerNodes();
        }

        private static void BuildContextualMenu(ContextualMenuPopulateEvent evt) { }

        private void InitializeInnerNodes()
        {
            foreach (string nodeGUID in group.innerNodeGUIDs.ToList()) {
                if (!graphView.graph.nodesPerGUID.ContainsKey(nodeGUID)) {
                    Debug.LogWarning("Node GUID not found: " + nodeGUID);
                    group.innerNodeGUIDs.Remove(nodeGUID);
                    continue;
                }
            
                var node = graphView.graph.nodesPerGUID[nodeGUID];
                var nodeView = graphView.nodeViewsPerNode[node];
            
                AddElement(nodeView);
            }
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            var graphElements = elements as GraphElement[] ?? elements.ToArray();
            foreach (var element in graphElements) {
                var node = element as ReanimatorGraphNode;

                // Adding an element that is not a node currently supported
                if (node == null)
                    continue;

                if (!group.innerNodeGUIDs.Contains(node.node.guid))
                    group.innerNodeGUIDs.Add(node.node.guid);
            }

            base.OnElementsAdded(graphElements);
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            // Only remove the nodes when the group exists in the hierarchy
            var graphElements = elements as GraphElement[] ?? elements.ToArray();
            if (parent != null) {
                foreach (var elem in graphElements) {
                    if (elem is ReanimatorGraphNode nodeView) {
                        group.innerNodeGUIDs.Remove(nodeView.node.guid);
                    }
                }
            }

            base.OnElementsRemoved(graphElements);
        }

        private void UpdateGroupColor(Color newColor)
        {
            group.color = newColor;
            style.backgroundColor = newColor;
        }

        private void TitleChangedCallback(ChangeEvent<string> e)
        {
            group.title = e.newValue;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            group.position = newPos;
        }
    }
}