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


        public const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";

        public Port input;
        public Port output;

        public ReanimatorGraphNode(ReanimatorNode node) : base(nodeStyleSheetPath) {
            // UseDefaultStyling();
            this.node = node;
            this.node.name = node.title == string.Empty ? node.GetType().Name : node.title;
            title = node.GetType().Name;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;
            
            CreateTitleEditField();
            SetupClasses();
        }

        private void SetupClasses() {
            switch (node) {
                case SimpleAnimationNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(SimpleAnimationNode));
                    AddToClassList("simpleAnimation");
                    break;
                case SwitchNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(SwitchNode));
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(SwitchNode));
                    AddToClassList("switch");
                    break;
                case OverrideNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(OverrideNode));
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(OverrideNode));
                    AddToClassList("override");
                    break;
                case BaseNode _:
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BaseNode));
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Deletable;
                    AddToClassList("base");
                    break;
            }
            
            if (input == null) return;
            input.portName = "";
            inputContainer.Add(input);
            
            if (output == null) return;
            output.portName = "";
            outputContainer.Add(output);
        }

        private void CreateTitleEditField() {
            Label description = this.Q<Label>("title-label");
            description.bindingPath = "title";
            description.Bind(new SerializedObject(node));

            var textField = new TextField();
            extensionContainer.Add(textField);
        }
        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "ResolutionGraph (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public void PlayAnimationPreview() {
            if (Application.isPlaying) return;
            
            RemoveFromClassList("--- selected ---");
            RemoveFromClassList("not-selected");

            switch (selected) {
                case true:
                    AddToClassList("--- selected ---");
                    break;
                case false:
                    AddToClassList("not-selected");
                    break;
            }
        }
    }
}