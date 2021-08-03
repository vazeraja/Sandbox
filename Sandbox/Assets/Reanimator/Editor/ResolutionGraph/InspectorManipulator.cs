using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Object;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class InspectorManipulator : MouseManipulator {
        private readonly ReanimatorGraphView reanimatorGraphView;
        private readonly InspectorCustomControl VE_Inspector;
        private readonly VisualElement VE_AnimationPreview;

        private IMGUIContainer inspectorContainer;
        private IMGUIContainer animationContainer;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;
        
        private GUIContent m_InfoText = new GUIContent("Animation Preview:");

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(ShowInInspector);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(ShowInInspector);
        }

        public InspectorManipulator(ReanimatorGraphView reanimatorGraphView, InspectorCustomControl veInspector,
            VisualElement veAnimationPreview)
        {
            this.VE_Inspector = veInspector;
            this.reanimatorGraphView = reanimatorGraphView;
            this.VE_AnimationPreview = veAnimationPreview;
        }

        private void ShowInInspector(MouseDownEvent evt)
        {
            if (!(target is ReanimatorGraphNode graphNode)) return;
            if (!CanStopManipulation(evt)) return;

            ClearAll();

            overrideNodeEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as OverrideNodeEditor;
            editor = UnityEditor.Editor.CreateEditor(graphNode.node);
            animationEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as AnimationNodeEditor;
            switchNodeEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as SwitchNodeEditor;
            
            switch (graphNode.node) {
                case OverrideNode _ when graphNode.IsSelected(reanimatorGraphView):
                    if (overrideNodeEditor && overrideNodeEditor.target) {
                        VE_AnimationPreview.style.visibility = Visibility.Hidden;
                        inspectorContainer = new IMGUIContainer(() => {
                            overrideNodeEditor.OnInspectorGUI();
                        });
                    }
                    break;
                case BaseNode _ when graphNode.IsSelected(reanimatorGraphView):
                    if (editor && editor.target) {
                        VE_AnimationPreview.style.visibility = Visibility.Hidden;
                        inspectorContainer = new IMGUIContainer(() => {
                            editor.OnInspectorGUI();
                        });
                    }
                    break;
                case SimpleAnimationNode _ when graphNode.IsSelected(reanimatorGraphView):
                    if (animationEditor && animationEditor.target) {
                        VE_AnimationPreview.style.visibility = Visibility.Visible;
                        
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
                case SwitchNode _ when graphNode.IsSelected(reanimatorGraphView):
                    if (switchNodeEditor && switchNodeEditor.target) {
                        VE_AnimationPreview.style.visibility = Visibility.Hidden;
                        inspectorContainer = new IMGUIContainer(() => {
                            switchNodeEditor.OnInspectorGUI();
                        });
                    }
                    break;
            }
            
            VE_Inspector.Add(inspectorContainer);
            VE_AnimationPreview.Add(animationContainer);
        }

        private void ClearAll()
        {
            VE_Inspector.Clear();
            VE_AnimationPreview.Clear();

            DestroyImmediate(editor);
            DestroyImmediate(switchNodeEditor);
            DestroyImmediate(animationEditor);
            DestroyImmediate(overrideNodeEditor);
        }
    }
}