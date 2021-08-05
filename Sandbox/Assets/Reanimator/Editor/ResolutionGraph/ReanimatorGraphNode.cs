using System;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public sealed class ReanimatorGraphNode : Node {

        public Action<ReanimatorGraphNode> onNodeSelected;

        private const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";
        
        public ReanimatorNode node { get; }
        public Port input { get; }
        public Port output { get; }
        
        public void PlayAnimationPreview()
        {
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
        
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Undo.RecordObject(node, "ResolutionGraph");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
        }
        
        public ReanimatorGraphNode(ReanimatorNode node) : base(nodeStyleSheetPath)
        {
            // UseDefaultStyling();
            this.node = node;
            this.node.name = node.title == string.Empty ? node.GetType().Name : node.title;
            title = node.GetType().Name;
            viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;
            
            
            switch (node) {
                case SimpleAnimationNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(SimpleAnimationNode));
                    input.portName = "";
                    inputContainer.Add(input);
                    AddToClassList("simpleAnimation");
                    break;
                case SwitchNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(SwitchNode));
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                        typeof(SwitchNode));
                    input.portName = "";
                    inputContainer.Add(input);
                    output.portName = "";
                    outputContainer.Add(output);
                    AddToClassList("switch");
                    break;
                case OverrideNode _:
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        typeof(OverrideNode));
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                        typeof(OverrideNode));
                    input.portName = "";
                    inputContainer.Add(input);
                    output.portName = "";
                    outputContainer.Add(output);
                    AddToClassList("override");
                    break;
                case BaseNode _:
                    output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                        typeof(BaseNode));
                    output.portName = "";
                    outputContainer.Add(output);
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Deletable;
                    AddToClassList("base");
                    break;
            }
            
            Label description = this.Q<Label>("title-label");
            description.bindingPath = "title";
            description.Bind(new SerializedObject(node));

            var textField = new TextField();
            extensionContainer.Add(textField);
        }

    }
}