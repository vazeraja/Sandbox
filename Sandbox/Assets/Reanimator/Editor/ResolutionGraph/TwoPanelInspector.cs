using System.Collections;
using System.Collections.Generic;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class TwoPanelInspector : TwoPaneSplitView {
        public new class UxmlFactory : UxmlFactory<TwoPanelInspector, UxmlTraits> { }

        public void Initialize(InspectorCustomControl inspectorPanel, VisualElement animationPreviewPanel)
        {
            this.inspectorPanel = inspectorPanel;
            this.animationPreviewPanel = animationPreviewPanel;
        }

        public void DrawNodeProperties(ReanimatorNode node)
        {
            ClearAll();

            editor = UnityEditor.Editor.CreateEditor(node);
            animationEditor = UnityEditor.Editor.CreateEditor(node) as AnimationNodeEditor;
            switchNodeEditor = UnityEditor.Editor.CreateEditor(node) as SwitchNodeEditor;
            overrideNodeEditor = UnityEditor.Editor.CreateEditor(node) as OverrideNodeEditor;

            switch (node) {
                case BaseNode _:

                    animationPreviewPanel.style.display = DisplayStyle.None;
                    inspectorContainer = new IMGUIContainer(() => {
                        if (editor && editor.target) {
                            editor.OnInspectorGUI();
                        }
                    });

                    break;
                case SimpleAnimationNode _:

                    animationPreviewPanel.style.display = DisplayStyle.Flex;
                    inspectorContainer = new IMGUIContainer(() => {
                        if (!animationEditor || !animationEditor.target) return;
                        animationEditor.OnInspectorGUI();
                    });

                    animationContainer = new IMGUIContainer(() => {
                        if (!animationEditor || !animationEditor.target) return;
                        animationEditor.RequiresConstantRepaint();
                        animationEditor.HasPreviewGUI();
                        animationEditor.OnPreviewGUI(GUILayoutUtility.GetRect(150, 150), new GUIStyle());
                    });

                    break;
                case SwitchNode _:

                    animationPreviewPanel.style.display = DisplayStyle.None;
                    inspectorContainer = new IMGUIContainer(() => {
                        if (!switchNodeEditor || !switchNodeEditor.target) return;
                        switchNodeEditor.OnInspectorGUI();
                    });

                    break;
                case OverrideNode _:
                    
                    animationPreviewPanel.style.display = DisplayStyle.None;
                    inspectorContainer = new IMGUIContainer(() => {
                        if (!overrideNodeEditor || !overrideNodeEditor.target) return;
                        overrideNodeEditor.OnInspectorGUI();
                    });
                    
                    break;
            }

            inspectorPanel.Add(inspectorContainer);
            animationPreviewPanel.Add(animationContainer);
        }

        private void ClearAll()
        {
            inspectorPanel.Clear();
            animationPreviewPanel.Clear();

            Object.DestroyImmediate(editor);
            Object.DestroyImmediate(switchNodeEditor);
            Object.DestroyImmediate(animationEditor);
            Object.DestroyImmediate(overrideNodeEditor);
        }

        private IMGUIContainer inspectorContainer;
        private IMGUIContainer animationContainer;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;

        private InspectorCustomControl inspectorPanel;
        private VisualElement animationPreviewPanel;
    }
}