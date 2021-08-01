using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class DragAndDropManipulator : MouseManipulator {
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragExitedEvent>(CreateDragAndDropNodes);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragExitedEvent>(CreateDragAndDropNodes);
        }

        private void CreateDragAndDropNodes(DragExitedEvent evt) 
        {
            if (!(target is ReanimatorGraphView graphView)) return;
            if (DragAndDrop.objectReferences == null) return;

            var references = DragAndDrop.objectReferences;
            List<ReanimatorNode> draggedNodes = new List<ReanimatorNode>();

            Vector2 nodePosition = graphView.ChangeCoordinatesTo(graphView.contentViewContainer, evt.localMousePosition);
            foreach (var reference in references) {
                if (reference is ReanimatorNode reanimatorNode) {
                    draggedNodes.Add(reanimatorNode);

                    foreach (ReanimatorNode node in draggedNodes) {
                        switch (node) {
                            case SimpleAnimationNode simpleAnimationNode: {
                                var cels = simpleAnimationNode.sprites;
                                var controlDriver = simpleAnimationNode.ControlDriver;
                                var drivers = simpleAnimationNode.Drivers;
                                EditorApplication.delayCall += () => {
                                    graphView.CreateSimpleAnimationNode(node.GetType(), nodePosition, cels,
                                        controlDriver, drivers);
                                };
                                break;
                            }
                            case SwitchNode switchNode: {
                                EditorApplication.delayCall += () => {
                                    var nodes = switchNode.nodes;
                                    graphView.CreateSwitchNode(switchNode.GetType(), nodePosition, nodes);
                                };
                                break;
                            }
                            case OverrideNode overrideNode: {
                                EditorApplication.delayCall += () => {
                                    graphView.CreateNode(overrideNode.GetType(), nodePosition);
                                };
                                break;
                            }
                        }
                    }
                }
                else {
                    EditorUtility.DisplayDialog("Invalid", "Dont be dumb, use a Reanimator Node", "OK");
                    break;
                }
            }
        }
    }
}