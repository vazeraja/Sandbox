using System;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public sealed class ReanimatorGraphNode : Node {

        public UnityAction<ReanimatorGraphNode> onNodeSelected;

        private const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";
        public ReanimatorNode node { get; }
        public ReanimatorPort input { get; }
        public ReanimatorPort output { get; }
        public void OnCreated() {}
        public void OnRemoved() {}
        
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
                    if (string.IsNullOrEmpty(node.title)) {
                        node.title = node.GetType().Name;
                    }

                    input = new ReanimatorPort(Direction.Input, Port.Capacity.Single) {
                        portColor =  new Color(0.12f, 0.44f, 0.81f),
                        portName = "",
                    };
                    input.Initialize(this, "");
                    inputContainer.Add(input);
                    node.needsAnimationPreview = true;
                    AddToClassList("simpleAnimation");
                    break;
                case SwitchNode _:
                    if (string.IsNullOrEmpty(node.title)) {
                        node.title = node.GetType().Name;
                    }

                    input = new ReanimatorPort(Direction.Input, Port.Capacity.Single) {
                        portColor = new Color(0.94f, 0.7f, 0.31f),
                        portName = "",
                    };
                    input.Initialize(this, "");
                    
                    output = new ReanimatorPort(Direction.Output, Port.Capacity.Multi){
                        portColor = new Color(0.94f, 0.7f, 0.31f),
                        portName = "",
                    };
                    output.Initialize(this, "");
                    
                    inputContainer.Add(input);
                    outputContainer.Add(output);
                    node.needsAnimationPreview = false;
                    AddToClassList("switch");
                    break;
                case OverrideNode _:
                    if (string.IsNullOrEmpty(node.title)) {
                        node.title = node.GetType().Name;
                    }
                    
                    input = new ReanimatorPort(Direction.Input, Port.Capacity.Single){
                        portColor = new Color(0.81f, 0.29f, 0.28f),
                        portName = "",
                    };
                    input.Initialize(this, "");
                    
                    output = new ReanimatorPort(Direction.Output, Port.Capacity.Single){
                        portColor = new Color(0.81f, 0.29f, 0.28f),
                        portName = "",
                    };
                    output.Initialize(this, "");
                    
                    inputContainer.Add(input);
                    outputContainer.Add(output);
                    node.needsAnimationPreview = false;
                    AddToClassList("override");
                    break;
                case BaseNode _:
                    if (string.IsNullOrEmpty(node.title)) {
                        node.title = node.GetType().Name;
                    }
                    
                    output = new ReanimatorPort(Direction.Output, Port.Capacity.Single){
                        portColor = new Color(0.98f, 1f, 0.98f),
                        portName = "",
                    };
                    output.Initialize(this, "");
                    
                    outputContainer.Add(output);
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Deletable;
                    node.needsAnimationPreview = false;
                    AddToClassList("base");
                    break;
            }
            
            Label description = this.Q<Label>("title-label");
            description.AddToClassList("custom-title");
            description.bindingPath = "title";
            description.Bind(new SerializedObject(node));

            var textField = new TextField();
            extensionContainer.Add(textField);
        }
        
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
        

    }
}