using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class ReanimatorSaveService {
        private ReanimatorGraphView _graphView;

        private List<Edge> Edges => _graphView.edges.ToList();
        private List<ReanimatorGraphNode> Nodes => _graphView.nodes.ToList().Cast<ReanimatorGraphNode>().ToList();

        private IEnumerable<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        public static implicit operator SaveData(ReanimatorSaveService saveService) => saveService.Save();

        public ReanimatorSaveService(ReanimatorGraphView graphView)
        {
            _graphView = graphView;
        }

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
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++) {
                var outputNode = connectedSockets[i].output.node as ReanimatorGraphNode;
                var inputNode = connectedSockets[i].input.node as ReanimatorGraphNode;
                saveData.NodeLinks.Add(new NodeLinkData {
                    BaseNodeGUID = outputNode?.node.guid,
                    TargetNodeGUID = inputNode?.node.guid
                });
            }

            foreach (var node in _graphView.graph.nodes.Where(node => !(node is BaseNode))) {
                saveData.ReanimatorNodeData.Add(new ReanimatorNodeData {
                    ReanimatorNode = node,
                    NodeGUID = node.guid,
                    Position = GetGraphNodeByGuid(node).GetPosition().position
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
    }
}