using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class GraphSaveService {
        private ReanimatorGraphView _graphView;
        
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<ReanimatorGraphNode> Nodes => _graphView.nodes.ToList().Cast<ReanimatorGraphNode>().ToList();
        private IEnumerable<Group> CommentBlocks => _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        public static GraphSaveService GetInstance(ReanimatorGraphView graphView){
            return new GraphSaveService {
                _graphView = graphView
            };
        }
        
        public void Save()
        {
            var saveData = new SaveData();

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

            if (_graphView.graph.SaveData == null) {
                _graphView.graph.SaveData = new SaveData();
                EditorUtility.SetDirty(_graphView.graph);
            }
            else {
                _graphView.graph.SaveData.CommentBlockData = saveData.CommentBlockData;
                EditorUtility.SetDirty(_graphView.graph);
            }
        }

    }

    internal static class Save {
        public static GraphSaveService GraphSaveService => new GraphSaveService();
    }
}