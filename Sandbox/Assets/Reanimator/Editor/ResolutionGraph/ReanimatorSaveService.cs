using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Editor;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class ReanimatorSaveService {
        public void LoadFromSubAssets()
        {
            // Create root node if graph is empty
            if (GraphSubAssets.Count == 0) {
                _graphView.graph.root = _graphView.graph.CreateSubAsset(typeof(BaseNode)) as BaseNode;
                EditorUtility.SetDirty(_graphView.graph);
                AssetDatabase.SaveAssets();
            }

            // Create every graph node from the nodes in the graph
            GraphSubAssets.ForEach(node => { _graphView.CreateGraphNode(node, node.title); });

            // Create all connections based on the children of the nodes in the graph
            GraphSubAssets.ForEach(p => {
                var children = Helpers.GetChildren(p);
                foreach (var c in children) {
                    // Returns node by its guid and cast it back to a ReanimatorGraphNode
                    var parent = _graphView.GetNodeByGuid(p.guid) as ReanimatorGraphNode;
                    var child = _graphView.GetNodeByGuid(c.guid) as ReanimatorGraphNode;

                    // If it is a new graph, check if the root has a child or not
                    if (parent?.node is BaseNode && child?.node == null)
                        continue;

                    // Connect each parents output to the saved children
                    var edge = parent?.output.ConnectTo(child?.input);
                    _graphView.AddElement(edge);
                }
            });

            _graphView.graph.groups.ForEach(group => {
                var block = _graphView.AddGroupView(group);
                block.AddElements(Nodes.Where(x => group.innerNodeGUIDs.Contains(x.node.guid)));
            });
            
            _graphView.graph.floatingElements.ForEach(elem => {
                var floatingGraphElement = _graphView.AddFloatingGraphElement(elem);
                floatingGraphElement.ResetPosition();
            });
        }

        public ReanimatorSaveService(ReanimatorGraphView graphView)
        {
            _graphView = graphView;
        }

        private ReanimatorSaveService() { }

        public static ReanimatorSaveService GetInstance(ReanimatorGraphView graphView)
            => new ReanimatorSaveService {_graphView = graphView};

        private ReanimatorGraphView _graphView;

        private List<ReanimatorNode> GraphSubAssets => _graphView.graph.nodes;
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<ReanimatorGraphNode> Nodes => _graphView.nodes.ToList().Cast<ReanimatorGraphNode>().ToList();
        private IEnumerable<UnityEditor.Experimental.GraphView.Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is UnityEditor.Experimental.GraphView.Group)
                .Cast<UnityEditor.Experimental.GraphView.Group>().ToList();
    }
}