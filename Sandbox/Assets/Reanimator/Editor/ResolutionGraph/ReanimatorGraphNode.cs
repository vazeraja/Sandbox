using System;
using System.Linq;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public sealed class ReanimatorGraphNode : Node {
        public readonly ReanimatorNode node;
        private InspectorCustomControl inspector;
        private ReanimatorGraphView graphView;

        public const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";

        public Port input;
        public Port output;

        public ReanimatorGraphNode(ReanimatorNode node, ReanimatorGraphView graphView, InspectorCustomControl inspector)
            : base(nodeStyleSheetPath) {
            // UseDefaultStyling();
            this.node = node;
            this.inspector = inspector;
            this.graphView = graphView;

            this.node.name = node.title == string.Empty ? node.GetType().Name : node.title;
            title = node.GetType().Name;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            this.AddManipulator(new InspectorManipulator(graphView, inspector));

            CreateInputPorts();
            CreateOutputPorts();
            CreateTitleEditField();
            SetupClasses();
        }

        private void SetupClasses() {
            switch (node) {
                case SimpleAnimationNode _:
                    AddToClassList("simpleAnimation");
                    break;
                case SwitchNode _:
                    AddToClassList("switch");
                    break;
                case OverrideNode _:
                    AddToClassList("override");
                    break;
                case BaseNode _:
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Deletable;
                    AddToClassList("base");
                    break;
            }
        }

        private void CreateTitleEditField() {
            Label description = this.Q<Label>("title-label");
            description.bindingPath = "title";
            description.Bind(new SerializedObject(node));

            var textField = new TextField();
            extensionContainer.Add(textField);
        }

        private void CreateInputPorts() {
            switch (node) {
                case SimpleAnimationNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(SimpleAnimationNode));
                    break;
                case SwitchNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(SwitchNode));
                    break;
                case OverrideNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(OverrideNode));
                    break;
                case BaseNode _:
                    break;
            }

            if (input == null) return;
            input.portName = "";
            inputContainer.Add(input);
        }

        private void CreateOutputPorts() {
            switch (node) {
                case SimpleAnimationNode _:
                    break;
                case SwitchNode _:
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                        typeof(SwitchNode));
                    break;
                case OverrideNode _:
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                        typeof(OverrideNode));
                    break;
                case BaseNode _:
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                        typeof(BaseNode));
                    break;
            }

            if (output == null) return;
            output.portName = "";
            outputContainer.Add(output);
        }

        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "ResolutionGraph (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public void PlayAnimationPreview() {
            RemoveFromClassList("selected ---");
            RemoveFromClassList("not-selected");

            if (!Application.isPlaying) {
                switch (selected) {
                    case true:
                        AddToClassList("selected ---");
                        break;
                    case false:
                        AddToClassList("not-selected");
                        break;
                }
            }
        }
    }
}