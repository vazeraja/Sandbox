using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public sealed class ReanimatorGraphNode : Node {
        
        private void ShowNodeInInspector()
        {
            ClearAll(reanimatorGraphView);
            
            switch (node) {
                case BaseNode _:
                    editor = UnityEditor.Editor.CreateEditor(node);
                    if (editor && editor.target) {
                        reanimatorGraphView.animationPreviewPanel.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
                    }

                    break;
                case SimpleAnimationNode _:
                    animationEditor = UnityEditor.Editor.CreateEditor(node) as AnimationNodeEditor;
                    if (animationEditor && animationEditor.target) {
                        reanimatorGraphView.animationPreviewPanel.style.display = DisplayStyle.Flex;
                        inspectorContainer = new IMGUIContainer(() => { animationEditor.OnInspectorGUI(); });
                        animationContainer = new IMGUIContainer(() => {
                            animationEditor.RequiresConstantRepaint();
                            animationEditor.HasPreviewGUI();
                            animationEditor.OnPreviewGUI(GUILayoutUtility.GetRect(150, 150), new GUIStyle());
                        });
                    }

                    break;
                case SwitchNode _:
                    switchNodeEditor = UnityEditor.Editor.CreateEditor(node) as SwitchNodeEditor;
                    if (switchNodeEditor && switchNodeEditor.target) {
                        reanimatorGraphView.animationPreviewPanel.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => { switchNodeEditor.OnInspectorGUI(); });
                    }

                    break;
                case OverrideNode _:
                    overrideNodeEditor = UnityEditor.Editor.CreateEditor(node) as OverrideNodeEditor;
                    if (overrideNodeEditor && overrideNodeEditor.target) {
                        reanimatorGraphView.animationPreviewPanel.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => { overrideNodeEditor.OnInspectorGUI(); });
                    }
                    break;
            }
            reanimatorGraphView.inspectorPanel.Add(inspectorContainer);
            reanimatorGraphView.animationPreviewPanel.Add(animationContainer);
        }
        private void ClearAll(ReanimatorGraphView graphView)
        {
            graphView.inspectorPanel.Clear();
            graphView.animationPreviewPanel.Clear();

            Object.DestroyImmediate(editor);
            Object.DestroyImmediate(switchNodeEditor);
            Object.DestroyImmediate(animationEditor);
            Object.DestroyImmediate(overrideNodeEditor);
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
            ShowNodeInInspector();
        }
        
        public ReanimatorGraphNode(ReanimatorGraphView reanimatorGraphView, ReanimatorNode node) : base(nodeStyleSheetPath)
        {
            // UseDefaultStyling();
            this.node = node;
            this.reanimatorGraphView = reanimatorGraphView;
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

        private const string nodeStyleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphNode.uxml";
        
        public ReanimatorNode node { get; }
        private readonly ReanimatorGraphView reanimatorGraphView;
        
        private IMGUIContainer inspectorContainer;
        private IMGUIContainer animationContainer;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;
        
        public Port input { get; }
        public Port output { get; }

        
    }
}