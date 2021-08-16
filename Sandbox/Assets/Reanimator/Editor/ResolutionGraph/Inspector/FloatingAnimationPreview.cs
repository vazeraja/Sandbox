using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class FloatingAnimationPreview : FloatingGraphElement {
        private ReanimatorGraphView reanimatorGraphView;
        
        private AnimationNodeEditor animationEditor;
        private IMGUIContainer animationContainer;

        protected override void Initialize(GraphView graphView)
        {
            reanimatorGraphView = graphView as ReanimatorGraphView;
        }
        
        public FloatingAnimationPreview()
        {
            title = "Animation Preview";
            scrollable = false;
            resizable = false;
        }
        
        public void DrawNodeProperties(ReanimatorGraphNode node)
        {
            content.Clear();
            Object.DestroyImmediate(animationEditor);
            
            animationEditor = UnityEditor.Editor.CreateEditor(node.node) as AnimationNodeEditor;
            animationContainer = node.node switch {
                SimpleAnimationNode _ => new IMGUIContainer(() => {
                    if (!animationEditor || !animationEditor.target) return;

                    animationEditor.UpdateSprites();
                    animationEditor.RequiresConstantRepaint();
                    animationEditor.HasPreviewGUI();
                    animationEditor.OnPreviewGUI(GUILayoutUtility.GetRect(125, 125), new GUIStyle());
                }),
                _ => animationContainer
            };

            content.Add(animationContainer);
        }

    }
}