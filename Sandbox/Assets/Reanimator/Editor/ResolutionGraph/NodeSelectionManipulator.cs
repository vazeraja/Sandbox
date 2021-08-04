using Aarthificial.Reanimation.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class NodeSelectionManipulator : MouseManipulator {
        
        private IMGUIContainer inspectorContainer;
        private IMGUIContainer animationContainer;

        private UnityEditor.Editor editor;
        private AnimationNodeEditor animationEditor;
        private SwitchNodeEditor switchNodeEditor;
        private OverrideNodeEditor overrideNodeEditor;
        
        public NodeSelectionManipulator()
        {
            // activators.Add(new ManipulatorActivationFilter()
            // {
            //     button = MouseButton.LeftMouse
            // });
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!(target is ReanimatorGraphView graphView)) 
                return;

            if (!CanStopManipulation(evt)) return;

            if (evt.target is ReanimatorGraphNode clickedElement) {
                Debug.Log(clickedElement.node.title);
            }
        }

    }
}