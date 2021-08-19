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
using Group = Aarthificial.Reanimation.Common.Group;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;

namespace Aarthificial.Reanimation {
    public class ReanimatorGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<ReanimatorGraphView, UxmlTraits> { }

        public ResolutionGraph graph;
        public ReanimatorGraphEditorWindow editorWindow;
        private ReanimatorSearchWindowProvider searchWindowProvider;
        public UnityAction<ReanimatorGraphNode> onNodeSelected;

        public List<ReanimatorGraphNode> GraphNodes => nodes.ToList().Cast<ReanimatorGraphNode>().ToList();
        private IEnumerable<MiniMap> miniMaps => graphElements.ToList().Where(x => x is MiniMap).Cast<MiniMap>().ToList();
        public FloatingAnimationPreview FloatingAnimationPreview;
        
        
        
        public Dictionary<ReanimatorNode, ReanimatorGraphNode> GraphNodesPerNode = new Dictionary<ReanimatorNode, ReanimatorGraphNode>();
        public List<ReanimatorGroup> groupViews = new List<ReanimatorGroup>();
        
        // Dictionary<Type, FloatingGraphElement> pinnedElements = new Dictionary<Type, FloatingGraphElement>();

        private const string styleName = "ReanimatorGraphEditor";
        
        public ReanimatorGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));

            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new DragAndDropManipulator());

            Undo.undoRedoPerformed += () => {
                ClearGraphElements();
                Helpers.SaveService(this).LoadFromSubAssets();
                AssetDatabase.SaveAssets();
            };
            EditorApplication.update += PlayAnimationPreview;
        }

        public void Init(ReanimatorGraphEditorWindow editorWindow, ResolutionGraph graph)
        {
            this.graph = graph;
            this.editorWindow = editorWindow;
        }
        public void ClearGraphElements()
        {
            GraphNodesPerNode.Clear();
            groupViews.Clear();

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            DeleteElements(miniMaps);
            if(FloatingAnimationPreview != null) Remove(FloatingAnimationPreview);
            graphViewChanged += OnGraphViewChanged;
        }

        /// <summary>
        /// Creates a Search Window as seen in Unity graph tools such as Shader Graph
        /// </summary>
        /// <param name="window"></param>
        public void CreateSearchWindow(EditorWindow window)
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
        public FloatingAnimationPreview AddFloatingElement(FloatingElement floatingElement)
        {
            Undo.RecordObject(graph, "Resolution Tree");
            graph.AddFloatingElement(floatingElement);
            return AddFloatingGraphElement(floatingElement);
        }

        public FloatingAnimationPreview AddFloatingGraphElement(FloatingElement floatingElement)
        {
            var f = new FloatingAnimationPreview();
            f.InitializeGraphView(floatingElement, this);
            FloatingAnimationPreview = f;
            Add(f);

            return f;
        }

        public void RemoveFloatingGraphElement()
        {
            graph.RemoveFloatingElement(FloatingAnimationPreview.FloatingElement);
            Remove(FloatingAnimationPreview);
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
        public ReanimatorNode CreateNode(Type type, Vector2 nodePosition)
        {
            ReanimatorNode node = graph.CreateSubAsset(type);
            node.position = nodePosition;
            var graphNode = CreateGraphNode(node);
            Helpers.Call(() => graphNode.OnCreated());
            return node;
        }

        /// <summary>
        /// Create a ReanimatorNode graph element and add it to the graph view 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="assetName"></param>
        public ReanimatorGraphNode CreateGraphNode(ReanimatorNode node, string assetName = null)
        {
            node.name = string.IsNullOrEmpty(assetName) ? node.GetType().Name : assetName;

            var graphNode = new ReanimatorGraphNode(this, node) {
                onNodeSelected = onNodeSelected
            };
            AddElement(graphNode);

            GraphNodesPerNode[node] = graphNode;

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
                        GraphNodesPerNode.Remove(graphNode.node);
                        graph.DeleteSubAsset(graphNode.node);
                        break;
                    case Edge edge:
                        var parentNode = edge.output.node as ReanimatorGraphNode;
                        var childNode = edge.input.node as ReanimatorGraphNode;
                        graph.RemoveChild(parentNode?.node, childNode?.node);
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

                graph.AddChild(parentNode?.node, childNode?.node);
            });

            return graphViewChange;
        }

        private void PlayAnimationPreview()
        {
            nodes
                .ToList()
                .Cast<ReanimatorGraphNode>()
                .ToList()
                .ForEach(node => {
                    if (node.node is SimpleAnimationNode) {
                        node.PlayAnimationPreview();
                    }
                });
        }
    }
}