using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Object;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class InspectorManipulator : MouseManipulator {
        private readonly InspectorCustomControl inspector;
        private readonly ReanimatorGraphView reanimatorGraphView;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(ShowInInspector);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(ShowInInspector);
        }

        public InspectorManipulator(ReanimatorGraphView reanimatorGraphView, InspectorCustomControl inspector)
        {
            this.inspector = inspector;
            this.reanimatorGraphView = reanimatorGraphView;
        }

        private void ShowInInspector(MouseDownEvent evt)
        {
            if (!(target is ReanimatorGraphNode graphNode)) return;
            if (!CanStopManipulation(evt)) return;

            inspector.Clear();
            DestroyImmediate(editor);
            DestroyImmediate(switchNodeEditor);
            DestroyImmediate(animationEditor);
            DestroyImmediate(overrideNodeEditor);
            
            overrideNodeEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as OverrideNodeEditor;
            editor = UnityEditor.Editor.CreateEditor(graphNode.node);
            animationEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as AnimationNodeEditor;
            switchNodeEditor = UnityEditor.Editor.CreateEditor(graphNode.node) as SwitchNodeEditor;

            IMGUIContainer container = new IMGUIContainer(() => {
                switch (graphNode.node) {
                    case OverrideNode _ when graphNode.IsSelected(reanimatorGraphView):
                        if (overrideNodeEditor && editor.target) {
                            overrideNodeEditor.OnInspectorGUI();
                        }
                        break;
                    case BaseNode _ when graphNode.IsSelected(reanimatorGraphView):
                        if (switchNodeEditor && editor.target) {
                            editor.OnInspectorGUI();
                        }
                        break;
                    case SimpleAnimationNode _ when graphNode.IsSelected(reanimatorGraphView):
                        if (animationEditor && editor.target) {
                            animationEditor.OnInspectorGUI();
                            animationEditor.RequiresConstantRepaint();
                            animationEditor.HasPreviewGUI();
                            animationEditor.OnPreviewGUI(GUILayoutUtility.GetRect(200, 200), new GUIStyle());
                        }
                        break;
                    case SwitchNode _ when graphNode.IsSelected(reanimatorGraphView):
                        if (switchNodeEditor && editor.target) {
                            switchNodeEditor.OnInspectorGUI();
                        }
                        break;
                }
            });
            inspector.Add(container);
        }
    }
}