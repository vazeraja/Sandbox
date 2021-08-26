using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Group = Aarthificial.Reanimation.Common.Group;


namespace Aarthificial.Reanimation {
    public class ReanimatorGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<ReanimatorGraphView, UxmlTraits> { }

        public ResolutionGraph graph;
        public ReanimatorGraphEditorWindow editorWindow;
        private ReanimatorSearchWindowProvider searchWindowProvider;
        public UnityAction<ReanimatorGraphNode> onNodeSelected;

        public List<ReanimatorGraphNode> nodeViews = new List<ReanimatorGraphNode>();

        public Dictionary<ReanimatorNode, ReanimatorGraphNode> nodeViewsPerNode =
            new Dictionary<ReanimatorNode, ReanimatorGraphNode>();

        public List<ReanimatorGroup> groupViews = new List<ReanimatorGroup>();
        public List<ReanimatorEdge> edgeViews = new List<ReanimatorEdge>();

        public FloatingAnimationPreview FloatingAnimationPreview;


        private const string styleName = "ReanimatorGraphEditor";

        public ReanimatorGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));
            
            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;
            graphViewChanged = OnGraphViewChanged;
            
            Debug.Log("Constructor");
            
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new DragAndDropManipulator());
            
            Undo.undoRedoPerformed += () => {
                Initialize(graph, editorWindow);
                AssetDatabase.SaveAssets();
            };
            EditorApplication.update += PlayAnimationPreview;
        }

        private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            var data = new CopyPasteHelper();

            var enumerable = elements.ToList();
            foreach (var graphElement in enumerable.Where(e => e is ReanimatorGraphNode)) {
                var nodeView = (ReanimatorGraphNode) graphElement;
                data.copiedNodes.Add(JsonSerializer.SerializeNode(nodeView.node));
            }

            foreach (var graphElement in enumerable.Where(e => e is ReanimatorGroup)) {
                var groupView = (ReanimatorGroup) graphElement;
                data.copiedGroups.Add(JsonSerializer.Serialize(groupView.@group));
            }
            
            foreach (ReanimatorEdge edgeView in elements.Where(e => e is ReanimatorEdge))
                data.copiedEdges.Add(JsonSerializer.Serialize(edgeView.edgeData));

            ClearSelection();
            
            graph.CopyPasteHelper = data;
            return JsonUtility.ToJson(data, true);;
        }

        private bool CanPasteSerializedDataCallback(string serializedData)
        {
            try {
                return JsonUtility.FromJson(serializedData, typeof(CopyPasteHelper)) != null;
            }
            catch {
                return false;
            }
        }

        private void UnserializeAndPasteCallback(string operationName, string serializedData)
        {
            var data = JsonUtility.FromJson<CopyPasteHelper>(serializedData);

            RegisterCompleteObjectUndo(operationName);

            Dictionary<string, ReanimatorNode> copiedNodesMap = new Dictionary<string, ReanimatorNode>();

            var unserializedGroups = data.copiedGroups.Select(g => JsonSerializer.Deserialize<Group>(g)).ToList();
            var unserializedEdges = data.copiedGroups.Select(g => JsonSerializer.Deserialize<EdgeData>(g)).ToList();

            foreach (var serializedNode in data.copiedNodes) {
                var node = JsonSerializer.DeserializeNode(serializedNode);

                Debug.Log(node.title);

                if (node == null)
                    continue;

                string sourceGUID = node.guid;
                graph.nodesPerGUID.TryGetValue(sourceGUID, out var sourceNode);
                //Call OnNodeCreated on the new fresh copied node
                node.createdFromDuplication = true;
                node.createdWithinGroup = unserializedGroups.Any(g => g.innerNodeGUIDs.Contains(sourceGUID));
                //And move a bit the new node
                node.position += new Vector2(20, 20);

                var newNodeView = CreateGraphNode(node);

                copiedNodesMap[sourceGUID] = node;

                //Select the new node
                AddToSelection(nodeViewsPerNode[node]);
            }

            foreach (var group in unserializedGroups) {
                //Same than for node
                group.OnCreated();

                // try to centre the created node in the screen
                group.position.position += new Vector2(20, 20);

                var oldGUIDList = group.innerNodeGUIDs.ToList();
                group.innerNodeGUIDs.Clear();
                foreach (var guid in oldGUIDList) {
                    graph.nodesPerGUID.TryGetValue(guid, out ReanimatorNode node);

                    // In case group was copied from another graph
                    if (node == null) {
                        copiedNodesMap.TryGetValue(guid, out node);
                        group.innerNodeGUIDs.Add(node.guid);
                    }
                    else {
                        group.innerNodeGUIDs.Add(copiedNodesMap[guid].guid);
                    }
                }

                AddGroup(group);
            }

            foreach (var edge in unserializedEdges) {
                nodeViewsPerNode.TryGetValue(edge.baseNode, out var baseNode);
                nodeViewsPerNode.TryGetValue(edge.targetNode, out var targetNode);
                //AddEdge(baseNode, targetNode);
            }
        }

        void InitializeNodeViews()
        {
            graph.nodes.RemoveAll(n => n == null);

            foreach (var _ in graph.nodes.Select(node => CreateGraphNode(node))) { }
        }

        void InitializeGroups()
        {
            foreach (var group in graph.groups)
                AddGroupView(group);
        }

        void InitializeEdgeViews()
        {
            foreach (var edge in graph.edges)
                AddEdgeView(edge);
        }

        public void Initialize(ResolutionGraph graph, ReanimatorGraphEditorWindow editorWindow)
        {
            this.graph = graph;
            this.editorWindow = editorWindow;

            EditorSceneManager.sceneSaved += _ => SaveGraphToDisk();
            RegisterCallback<KeyDownEvent>(e => {
                if (e.keyCode == KeyCode.S && e.actionKey)
                    SaveGraphToDisk();
            });

            ClearGraphElements();

            InitializeNodeViews();
            InitializeGroups();
            InitializeEdgeViews();
            
            //graphViewChanged -= OnGraphViewChanged;
            //DeleteElements(graphElements.ToList());
            //if(FloatingAnimationPreview != null)
            //    RemoveFloatingGraphElement();
            //graphViewChanged += OnGraphViewChanged;

            //Helpers.SaveService(this).LoadFromSubAssets();
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
        
        public EdgeData AddEdge(ReanimatorGraphNode parentNode, ReanimatorGraphNode childNode)
        {
            var edgeData = new EdgeData {
                owner = graph,
                guid = Guid.NewGuid().ToString(),
                baseNode = parentNode?.node,
                baseNodeGUID = parentNode?.node.guid,
                targetNode = childNode?.node,
                targetNodeGUID = childNode?.node.guid,
            };
            
            AddEdgeView(edgeData);
            return graph.AddEdge(edgeData);
        }

        public void AddEdgeView(EdgeData data)
        {
            nodeViewsPerNode.TryGetValue(data.baseNode, out var baseGraphNode);
            nodeViewsPerNode.TryGetValue(data.targetNode, out var targetGraphNode);
            
            var edgeToCreate = baseGraphNode?.output.ConnectTo(targetGraphNode?.input) as ReanimatorEdge;
            //edgeToCreate.userData = data;
            
            AddElement(edgeToCreate);
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

            nodeViews.Add(graphNode);
            nodeViewsPerNode[node] = graphNode;

            return graphNode;
        }

        void ClearGraphElements()
        {
            RemoveGroups();
            RemoveNodeViews();
            RemoveEdgeViews();
        }

        void RemoveNodeViews()
        {
            foreach (var nodeView in nodeViews)
                RemoveElement(nodeView);
            nodeViews.Clear();
            nodeViewsPerNode.Clear();
        }
        public void RemoveEdgeViews()
        {
            foreach (var edge in edgeViews)
                RemoveElement(edge);
            edgeViews.Clear();
        }

        public void RemoveGroups()
        {
            foreach (var groupView in groupViews)
                RemoveElement(groupView);
            groupViews.Clear();
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
        public void CreateSimpleAnimationNode(Type type, Vector2 nodePosition,
            IEnumerable<SimpleCel> simpleCels, ControlDriver controlDriver, DriverDictionary driverDictionary)
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
                RegisterCompleteObjectUndo("Remove Graph Elements");
                switch (elem) {
                    case ReanimatorGraphNode graphNode:
                        Helpers.Call(() => graphNode.OnRemoved());
                        graph.DeleteSubAsset(graphNode.node);
                        RemoveElement(graphNode);
                        break;
                    case ReanimatorEdge edge:
                        Debug.Log("fhdjkh");
                        edgeViews.Remove(edge);
                        graph.RemoveEdge(edge.edgeData.guid);
                        
                        var parentNode = edge.output.node as ReanimatorGraphNode;
                        var childNode = edge.input.node as ReanimatorGraphNode;

                        graph.RemoveChild(parentNode?.node, childNode?.node);
                        break;
                    case ReanimatorGroup group:
                        graph.RemoveGroup(group.group);
                        RemoveElement(group);
                        break;
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge => {
                RemoveElement(edge);
                RegisterCompleteObjectUndo("Remove Graph Elements");

                var parentNode = edge.output.node as ReanimatorGraphNode;
                var childNode = edge.input.node as ReanimatorGraphNode;

                graph.AddChild(parentNode?.node, childNode?.node);
                AddEdge(parentNode, childNode);
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

        public void SaveGraphToDisk()
        {
            if (graph == null)
                return;

            EditorUtility.SetDirty(graph);
        }

        public void RegisterCompleteObjectUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(graph, name);
        }
    }
}