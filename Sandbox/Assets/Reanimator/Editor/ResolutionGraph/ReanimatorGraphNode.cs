using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public sealed class ReanimatorGraphNode : Node {

        public UnityAction<ReanimatorNode> onNodeSelected;

        private const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";
        
        public NodeDTO nodeData;
        public readonly ReanimatorPort input;
        public readonly ReanimatorPort output;
        public void OnCreated() {}
        public void OnRemoved() {}
        
        public ReanimatorGraphNode(NodeDTO nodeData) : base(nodeStyleSheetPath)
        {
            // UseDefaultStyling();
            this.nodeData = nodeData;
            this.nodeData.node.name = nodeData.title == string.Empty ? nodeData.GetType().Name: nodeData.node.title;
            this.nodeData.title = nodeData.title == string.Empty ? nodeData.GetType().Name : nodeData.node.title;
            title = nodeData.GetType().Name;
            viewDataKey = nodeData.guid;

            style.left = nodeData.position.x;
            style.top = nodeData.position.y;
            
            switch (nodeData.node) {
                case SimpleAnimationNode _:
                    if (string.IsNullOrEmpty(nodeData.title)) {
                        nodeData.title = nodeData.GetType().Name;
                    }

                    input = new ReanimatorPort(Direction.Input, Port.Capacity.Single) {
                        portColor =  new Color(0.12f, 0.44f, 0.81f),
                        portName = "",
                    };
                    input.Initialize(this, "");
                    inputContainer.Add(input);
                    nodeData.needsAnimationPreview = true;
                    AddToClassList("simpleAnimation");
                    break;
                case SwitchNode _:
                    if (string.IsNullOrEmpty(nodeData.title)) {
                        nodeData.title = nodeData.GetType().Name;
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
                    nodeData.needsAnimationPreview = false;
                    AddToClassList("switch");
                    break;
                case OverrideNode _:
                    if (string.IsNullOrEmpty(nodeData.title)) {
                        nodeData.title = nodeData.GetType().Name;
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
                    nodeData.needsAnimationPreview = false;
                    AddToClassList("override");
                    break;
                case BaseNode _:
                    if (string.IsNullOrEmpty(nodeData.title)) {
                        nodeData.title = nodeData.GetType().Name;
                    }
                    
                    output = new ReanimatorPort(Direction.Output, Port.Capacity.Single){
                        portColor = new Color(0.98f, 1f, 0.98f),
                        portName = "",
                    };
                    output.Initialize(this, "");
                    
                    outputContainer.Add(output);
                    capabilities &= ~Capabilities.Movable;
                    capabilities &= ~Capabilities.Deletable;
                    nodeData.needsAnimationPreview = false;
                    AddToClassList("base");
                    break;
            }
            
            Label description = this.Q<Label>("title-label");
            description.AddToClassList("custom-title");
            description.bindingPath = "title";
            description.Bind(new SerializedObject(nodeData.node));

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

            //Undo.RecordObject(nodeData, "ResolutionGraph");
            nodeData.position.x = newPos.xMin;
            nodeData.position.y = newPos.yMin;
            //EditorUtility.SetDirty(nodeData);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(nodeData.node);
        }
        

    }
}