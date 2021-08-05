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
        private SaveData Save()
        {
            Debug.Log("Saving");

            var saveData = new SaveData();
            if (!SaveNodes(saveData)) return default;
            SaveGroupBlocks(saveData);

            return saveData;
        }

        private bool SaveNodes(SaveData saveData)
        {
            if (!Edges.Any()) return false;
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            foreach (var edge in connectedPorts) {
                var outputNode = edge.output.node as ReanimatorGraphNode;
                var inputNode = edge.input.node as ReanimatorGraphNode;
                saveData.NodeLinks.Add(new NodeLinkData {
                    BaseNode = outputNode?.node,
                    BaseNodeGUID = outputNode?.node.guid,
                    TargetNode = inputNode?.node,
                    TargetNodeGUID = inputNode?.node.guid
                });
            }

            foreach (var graphNode in Nodes) {
                saveData.ReanimatorNodeData.Add(new ReanimatorNodeData {
                    ReanimatorNode = graphNode.node,
                    NodeGUID = graphNode.node.guid,
                    Position = GetGraphNodeByGuid(graphNode.node).GetPosition().position
                });
            }

            return true;
        }

        private ReanimatorGraphNode GetGraphNodeByGuid(ReanimatorNode node) =>
            _graphView.GetNodeByGuid(node.guid) as ReanimatorGraphNode;

        private void SaveGroupBlocks(SaveData saveData)
        {
            foreach (var block in CommentBlocks) {
                var childNodes = block.containedElements
                    .Where(x => x is ReanimatorGraphNode)
                    .Cast<ReanimatorGraphNode>()
                    .Select(x => x.node.guid)
                    .ToList();

                saveData.CommentBlockData.Add(new GroupBlock() {
                    ChildNodes = childNodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadFromSubAssets()
        {
            // Create root node if graph is empty
            if (GraphSubAssets.Count == 0) {
                _graphView.graph.root = _graphView.CreateSubAsset(typeof(BaseNode)) as BaseNode;
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

            // Load all comment blocks and contained nodes
            foreach (GroupBlock commentBlockData in GraphSaveData.CommentBlockData) {
                var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.BlockSize),
                    commentBlockData);
                block.AddElements(Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.node.guid)));
            }
        }

        public void LoadFromSaveData()
        {
            GenerateNodes();
            ConnectNodes();
        }

        private bool GenerateNodes()
        {
            try {
                foreach (var tempNode in GraphSaveData.ReanimatorNodeData.Select(perNode =>
                    _graphView.CreateGraphNode(perNode.ReanimatorNode))) {
                    _graphView.AddElement(tempNode);
                }

                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        private bool ConnectNodes()
        {
            try {
                foreach (var connection in GraphSaveData.NodeLinks) {
                    var baseNode = Nodes.Find(x => x.node.guid == connection.BaseNodeGUID);
                    var targetNode = Nodes.Find(x => x.node.guid == connection.TargetNodeGUID);

                    var edge = baseNode?.output.ConnectTo(targetNode?.input);
                    _graphView.Add(edge);
                }

                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge() {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        public static implicit operator SaveData(ReanimatorSaveService saveService) => saveService.Save();

        public ReanimatorSaveService(ReanimatorGraphView graphView)
        {
            _graphView = graphView;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public ReanimatorSaveService()
        { }

        public static ReanimatorSaveService GetInstance(ReanimatorGraphView graphView)
            => new ReanimatorSaveService {_graphView = graphView};

        private ReanimatorGraphView _graphView;

        private List<ReanimatorNode> GraphSubAssets => _graphView.graph.nodes;
        private SaveData GraphSaveData => _graphView.graph.saveData;
        private List<Edge> Edges => _graphView.edges.ToList();

        private List<ReanimatorGraphNode> Nodes =>
            _graphView.nodes.ToList().Cast<ReanimatorGraphNode>().ToList();

        private IEnumerable<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();
    }
}