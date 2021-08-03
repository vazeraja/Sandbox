using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Editor;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class ReanimatorGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<ReanimatorGraphView, UxmlTraits> { }
        
        
        public ReanimatorGraphView()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            styleSheets.Add(styleSheet);

            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new DragAndDropManipulator());

            Undo.undoRedoPerformed += UndoRedo;
            EditorApplication.update += PlayAnimationPreview;
        }
        
        public void Initialize(
            ReanimatorGraphEditor editorWindow,
            ResolutionGraph graph,
            InspectorCustomControl inspector,
            VisualElement animationPreview)
        {
            this.graph = graph;
            this.editorWindow = editorWindow;
            inspectorPanel = inspector;
            animationPreviewPanel = animationPreview;
            
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            CreateSearchWindow(editorWindow);
            CreateMiniMap();
            Load();
        }

        private void UndoRedo()
        {
            Initialize(editorWindow, graph, inspectorPanel, animationPreviewPanel);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a Search Window as seen in Unity graph tools such as Shader Graph
        /// </summary>
        /// <param name="window"></param>
        private void CreateSearchWindow(EditorWindow window)
        {
            searchWindowProvider = ScriptableObject.CreateInstance<ReanimatorSearchWindowProvider>();
            searchWindowProvider.Initialize(window, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
        }
        
        /// <summary>
        /// Creates a minimap on top left corner of the graphview
        /// </summary>
        private void CreateMiniMap()
        {
            var miniMap = new MiniMap {
                anchored = true
            };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            Add(miniMap);
        }

        /// <summary>
        /// Creates a group block to contain and organize sections of related nodes
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="groupBlock"></param>
        /// <returns></returns>
        public ReanimatorGroup CreateCommentBlock(Rect rect, GroupBlock groupBlock = null)
        {
            groupBlock ??= new GroupBlock();
            var group = new ReanimatorGroup(this, groupBlock);
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        /// <summary>
        /// Make sure inputs cant be connected to inputs and outputs to outputs
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
            => ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();

        /// <summary>
        /// Event listener to intercept the GraphView graphViewChanged delegate.
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(elem => {
                switch (elem) {
                    case ReanimatorGraphNode nodeDisplay:
                        graph.DeleteSubAsset(nodeDisplay.node);
                        break;
                    case Edge edge:
                        var parent = edge.output.node as ReanimatorGraphNode;
                        var child = edge.input.node as ReanimatorGraphNode;
                        graph.RemoveChild(parent?.node, child?.node);
                        break;
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge => {
                var parent = edge.output.node as ReanimatorGraphNode;
                var child = edge.input.node as ReanimatorGraphNode;

                graph.AddChild(parent?.node, child?.node);
            });

            return graphViewChange;
        }

        public ReanimatorNode CreateNode(Type type, Vector2 nodePosition)
        {
            ReanimatorNode node = graph.CreateSubAsset(type);
            node.position = nodePosition;
            CreateGraphNode(node);
            return node;
        }
        
        private void CreateGraphNode(ReanimatorNode node)
        {
            var graphNode = new ReanimatorGraphNode(this, node);
            AddElement(graphNode);
        }

        
        /// <summary>
        /// Creates a simple animation node on the graph
        /// -- Used for drag and drop nodes --
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodePosition"></param>
        /// <param name="simpleCels"></param>
        /// <param name="controlDriver"></param>
        /// <param name="driverDictionary"></param>
        public void CreateSimpleAnimationNode(
            Type type,
            Vector2 nodePosition,
            IEnumerable<SimpleCel> simpleCels,
            ControlDriver controlDriver,
            DriverDictionary driverDictionary)
        {
            if (!(CreateNode(type, nodePosition) is SimpleAnimationNode simpleAnimationNode)) return;
            var nodeSprites = simpleCels as SimpleCel[] ?? simpleCels.ToArray();
            simpleAnimationNode.sprites = nodeSprites;
            simpleAnimationNode.ControlDriver = controlDriver;
            simpleAnimationNode.Drivers = driverDictionary;
        }

        /// <summary>
        /// Creates a switch node on the graph
        /// -- Used for drag and drop nodes --
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodePosition"></param>
        /// <param name="reanimatorNodes"></param>
        public void CreateSwitchNode(Type type, Vector2 nodePosition, List<ReanimatorNode> reanimatorNodes)
        {
            if (!(CreateNode(type, nodePosition) is SwitchNode switchNode)) return;
            switchNode.nodes = reanimatorNodes;
        }
        
        private void PlayAnimationPreview() => GraphNodes.ForEach(node => {
            if (node.node is SimpleAnimationNode) node.PlayAnimationPreview();
        });

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

            if (graph.SaveData == null) {
                graph.SaveData = new SaveData();
                EditorUtility.SetDirty(graph);
            }
            else {
                graph.SaveData.CommentBlockData = saveData.CommentBlockData;
                EditorUtility.SetDirty(graph);
            }
        }

        private void Load()
        {
            // Create root node if graph is empty
            if (graph.nodes.Count == 0) {
                graph.root = graph.CreateSubAsset(typeof(BaseNode)) as BaseNode;
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }

            // Create every graph node from the nodes in the graph
            graph.nodes.ForEach(CreateGraphNode);

            // Create all connections based on the children of the nodes in the graph
            graph.nodes.ForEach(p => {
                var children = Helpers.GetChildren(p);
                foreach (var c in children) {
                    // Returns node by its guid and cast it back to a ReanimatorGraphNode
                    var parent = GetNodeByGuid(p.guid) as ReanimatorGraphNode;
                    var child = GetNodeByGuid(c.guid) as ReanimatorGraphNode;

                    // If it is a new graph, check if the root has a child or not
                    if (parent?.node is BaseNode && child?.node == null)
                        continue;

                    // Connect each parents output to the saved children
                    var edge = parent?.output.ConnectTo(child?.input);
                    AddElement(edge);
                }
            });

            // Load all comment blocks and contained nodes
            foreach (GroupBlock commentBlockData in graph.SaveData.CommentBlockData) {
                var block = CreateCommentBlock(new Rect(commentBlockData.Position, BlockSize),
                    commentBlockData);
                block.AddElements(GraphNodes.Where(x => commentBlockData.ChildNodes.Contains(x.node.guid)));
            }
        }

        private ResolutionGraph graph;
        private ReanimatorGraphEditor editorWindow;
        public InspectorCustomControl inspectorPanel;
        public VisualElement animationPreviewPanel;
        private ReanimatorSearchWindowProvider searchWindowProvider;

        private IEnumerable<ReanimatorGroup> CommentBlocks => graphElements
            .ToList()
            .Where(x => x is ReanimatorGroup)
            .Cast<ReanimatorGroup>().ToList();

        private List<ReanimatorGraphNode> GraphNodes => nodes.ToList().Cast<ReanimatorGraphNode>().ToList();

        private const string styleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uss";
        private readonly Vector2 BlockSize = new Vector2(300, 200);
    }
}