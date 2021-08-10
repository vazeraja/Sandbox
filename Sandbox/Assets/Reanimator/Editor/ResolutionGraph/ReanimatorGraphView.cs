using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class ReanimatorGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<ReanimatorGraphView, UxmlTraits> { }

        public ResolutionGraph graph;
        public ReanimatorGraphEditorWindow editorWindow;
        private ReanimatorSearchWindowProvider searchWindowProvider;
        public UnityAction<ReanimatorNode> onNodeSelected;

        public List<ReanimatorGraphNode> GraphNodes => nodes.ToList().Cast<ReanimatorGraphNode>().ToList();

        // public Dictionary<ReanimatorNode, ReanimatorGraphNode> GraphNodesPerNode =
        //     new Dictionary<ReanimatorNode, ReanimatorGraphNode>();

        private IEnumerable<MiniMap> MiniMaps =>
            graphElements.ToList().Where(x => x is MiniMap).Cast<MiniMap>().ToList();

        public List<ReanimatorGroup> groupViews = new List<ReanimatorGroup>();

        private const string styleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uss";

        public void Initialize(ReanimatorGraphEditorWindow editorWindow, ResolutionGraph graph)
        {
            this.graph = graph;
            this.editorWindow = editorWindow;

            //GraphNodesPerNode.Clear();
            groupViews.Clear();

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            DeleteElements(MiniMaps);
            graphViewChanged += OnGraphViewChanged;

            CreateSearchWindow(editorWindow);
        }

        public void SaveGraphToDisk()
        {
            if (graph == null)
                return;

            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
        }

        public void SaveToDisk(ScriptableObject obj)
        {
            if (obj == null)
                return;

            EditorUtility.SetDirty(obj);
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
        public void CreateMiniMap()
        {
            var miniMap = new MiniMap {
                anchored = true
            };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            Add(miniMap);
        }

        /// <summary>
        /// Create group and add it into stored list on the graph
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public ReanimatorGroup AddGroup(Group block)
        {
            Undo.RecordObject(graph, "Resolution Tree");
            graph.AddGroup(block);
            block.OnCreated();
            return AddGroupView(block);
        }

        /// <summary>
        /// Create group graph element and add it into the graph
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public ReanimatorGroup AddGroupView(Group block)
        {
            var c = new ReanimatorGroup(this, block);
            AddElement(c);

            groupViews.Add(c);
            return c;
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
        /// Create a sub asset of node when a node is requested to be created from the search window
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodePosition"></param>
        /// <returns></returns>
        public NodeDTO CreateNode(Type type, Vector2 nodePosition)
        {
            NodeDTO nodeDto = CreateSubAsset(type);
            nodeDto.position = nodePosition;
            var graphNode = CreateGraphNode(nodeDto);
            Helpers.Call(() => graphNode.OnCreated());
            return nodeDto;
        }

        /// <summary>
        /// Create a ReanimatorNode graph element and add it to the graph view 
        /// </summary>
        /// <param name="nodeDto"></param>
        /// <param name="assetName"></param>
        public ReanimatorGraphNode CreateGraphNode(NodeDTO nodeDto, string assetName = null)
        {
            nodeDto.node.name = string.IsNullOrEmpty(assetName) ? nodeDto.GetType().Name : assetName;

            var graphNode = new ReanimatorGraphNode(nodeDto) {
                onNodeSelected = onNodeSelected
            };
            AddElement(graphNode);

            //GraphNodesPerNode[nodeData] = graphNode;

            return graphNode;
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
            if (!(CreateNode(type, nodePosition).node is SimpleAnimationNode simpleAnimationNode)) return;
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
            if (!(CreateNode(type, nodePosition).node is SwitchNode switchNode)) return;
            switchNode.nodes = reanimatorNodes;
        }

        /// <summary>
        /// Creates a scriptable object sub asset for the current resolution graph and adds it to the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public NodeDTO CreateSubAsset(Type type, string assetName = null)
        {
            ReanimatorNode node = ScriptableObject.CreateInstance(type) as ReanimatorNode;

            // ReSharper disable once PossibleNullReferenceException
            node.name = string.IsNullOrEmpty(assetName) ? type.Name : assetName;

            var nodeData = new NodeDTO {
                node = node,
                guid = GUID.Generate().ToString()
            };
            
            Undo.RecordObject(graph, "Resolution Tree");
            graph.nodes.Add(nodeData);

            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, graph);
            }

            Undo.RegisterCreatedObjectUndo(node, "Resolution Tree");

            SaveGraphToDisk();
            return nodeData;
        }

        /// <summary>
        /// Removes the scriptable object sub asset from the resolution graph and removes it from the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="nodeDto"></param>
        private void DeleteSubAsset(NodeDTO nodeDto)
        {
            Undo.RecordObject(graph, "Resolution Tree");
            graph.nodes.Remove(nodeDto);
            Undo.DestroyObjectImmediate(nodeDto.node);

            SaveGraphToDisk();
        }

        /// <summary>
        /// Adds appropriate child node(s) when an edge or node is created
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        private void AddChild(ReanimatorNode parent, ReanimatorNode child)
        {
            switch (parent) {
                case BaseNode rootNode:
                    Undo.RecordObject(rootNode, "Resolution Tree");
                    graph.root = child;
                    rootNode.root = child;
                    SaveGraphToDisk();
                    SaveToDisk(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = child;
                    SaveGraphToDisk();
                    SaveToDisk(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Add(child);
                    SaveGraphToDisk();
                    SaveToDisk(switchNode);
                    break;
            }
        }

        /// <summary>
        /// Removes appropriate child node(s) when an edge or node is deleted
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        private void RemoveChild(ReanimatorNode parent, ReanimatorNode child)
        {
            switch (parent) {
                case BaseNode rootNode:
                    Undo.RecordObject(rootNode, "Resolution Tree");
                    graph.root = null;
                    rootNode.root = null;
                    SaveGraphToDisk();
                    SaveToDisk(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = null;
                    SaveGraphToDisk();
                    SaveToDisk(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Remove(child);
                    SaveGraphToDisk();
                    SaveToDisk(switchNode);
                    break;
            }
        }


        /// <summary>
        /// Event listener to intercept the GraphView graphViewChanged delegate.
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(elem => {
                switch (elem) {
                    case ReanimatorGraphNode graphNode:
                        Helpers.Call(() => graphNode.OnRemoved());
                        // GraphNodesPerNode.Remove(graphNode.NodeData);
                        DeleteSubAsset(graphNode.nodeData);
                        break;
                    case Edge edge:
                        var parentNode = edge.output.node as ReanimatorGraphNode;
                        var childNode = edge.input.node as ReanimatorGraphNode;
                        RemoveChild(parentNode?.nodeData.node, childNode?.nodeData.node);
                        break;
                    case ReanimatorGroup group:
                        graph.RemoveGroup(group.group);
                        groupViews.Remove(group);
                        RemoveElement(group);
                        break;
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge => {
                var parentNode = edge.output.node as ReanimatorGraphNode;
                var childNode = edge.input.node as ReanimatorGraphNode;

                AddChild(parentNode?.nodeData.node, childNode?.nodeData.node);
            });

            return graphViewChange;
        }

        private void PlayAnimationPreview()
        {
            nodes
                .ToList()
                .Cast<ReanimatorGraphNode>()
                .ToList()
                .ForEach(graphNode => {
                    if (graphNode.nodeData.node is SimpleAnimationNode) {
                        graphNode.PlayAnimationPreview();
                    }
                });
        }

        public ReanimatorGraphView()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath));

            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new DragAndDropManipulator());

            Undo.undoRedoPerformed += () => {
                Initialize(editorWindow, graph);
                AssetDatabase.SaveAssets();
            };
            EditorApplication.update += PlayAnimationPreview;
        }
    }
}