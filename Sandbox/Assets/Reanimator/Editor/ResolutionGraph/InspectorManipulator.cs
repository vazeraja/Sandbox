using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Object;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class InspectorManipulator : MouseManipulator {
        
        private IMGUIContainer inspectorContainer;
        private IMGUIContainer animationContainer;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;
        
        public InspectorManipulator() { }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(ShowInInspector);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(ShowInInspector);
        }


        private void ShowInInspector(MouseDownEvent evt)
        {
            if (!(target is ReanimatorGraphView graphView)) 
                return;

            if (!CanStopManipulation(evt)) return;

            if (!(evt.target is ReanimatorGraphNode clickedElement)) {
                var ve = evt.target as VisualElement;
                clickedElement = ve?.GetFirstAncestorOfType<ReanimatorGraphNode>();
                if (clickedElement == null)
                    return;
            }

            ClearAll(graphView);
            
            switch (clickedElement.node) {
                case BaseNode _ when clickedElement.IsSelected(graphView):
                    editor = UnityEditor.Editor.CreateEditor(clickedElement.node);

                    if (editor && editor.target) {
                        graphView.animationPreview.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => {
                            editor.OnInspectorGUI();
                        });
                    }
                    break;
                case SimpleAnimationNode _ when clickedElement.IsSelected(graphView):
                    animationEditor = UnityEditor.Editor.CreateEditor(clickedElement.node) as AnimationNodeEditor;

                    if (animationEditor && animationEditor.target) {
                        graphView.animationPreview.style.display = DisplayStyle.Flex;
                        
                        inspectorContainer = new IMGUIContainer(() => {
                            animationEditor.OnInspectorGUI();
                        });
                        
                        
                        animationContainer = new IMGUIContainer(() => {
                            EditorGUILayout.Space();
                            animationEditor.RequiresConstantRepaint();
                            animationEditor.HasPreviewGUI();
                            animationEditor.OnPreviewGUI(GUILayoutUtility.GetRect(150, 150), new GUIStyle());
                        });
                    }
                    break;
                case SwitchNode _ when clickedElement.IsSelected(graphView):
                    switchNodeEditor = UnityEditor.Editor.CreateEditor(clickedElement.node) as SwitchNodeEditor;

                    if (switchNodeEditor && switchNodeEditor.target) {
                        graphView.animationPreview.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => {
                            switchNodeEditor.OnInspectorGUI();
                        });
                    }
                    break;
                case OverrideNode _ when clickedElement.IsSelected(graphView):
                    overrideNodeEditor = UnityEditor.Editor.CreateEditor(clickedElement.node) as OverrideNodeEditor;

                    if (overrideNodeEditor && overrideNodeEditor.target) {
                        graphView.animationPreview.style.display = DisplayStyle.None;
                        inspectorContainer = new IMGUIContainer(() => {
                            overrideNodeEditor.OnInspectorGUI();
                        });
                    }
                    break;
            }
            
            graphView.inspector.Add(inspectorContainer);
            graphView.animationPreview.Add(animationContainer);
        }

        private void ClearAll(ReanimatorGraphView graphView)
        {
            graphView.inspector.Clear();
            graphView.animationPreview.Clear();

            DestroyImmediate(editor);
            DestroyImmediate(switchNodeEditor);
            DestroyImmediate(animationEditor);
            DestroyImmediate(overrideNodeEditor);
        }
    }
}