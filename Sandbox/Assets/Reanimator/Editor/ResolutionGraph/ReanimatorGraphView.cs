using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class ReanimatorGraphView : GraphView {
        public new class UxmlFactory : UxmlFactory<ReanimatorGraphView, UxmlTraits> { }

        public void Initialize(ReanimatorGraphEditor editorWindow, ResolutionGraph graph, bool useSaveData = true)
        {
            this.graph = graph;
            this.editorWindow = editorWindow;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged; 

            CreateSearchWindow(editorWindow);
            CreateMiniMap();

            //ReanimatorSaveService.GetInstance(this).LoadFromSaveData();
            ReanimatorSaveService.GetInstance(this).LoadFromSubAssets();
        }

        private void UndoRedo()
        {
            Initialize(editorWindow, graph);
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
        /// Create a sub asset of node when a node is requested to be created from the search window
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodePosition"></param>
        /// <returns></returns>
        public ReanimatorNode CreateNode(Type type, Vector2 nodePosition)
        {
            ReanimatorNode node = CreateSubAsset(type);
            node.position = nodePosition;
            CreateGraphNode(node);
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
            
            var graphNode = new ReanimatorGraphNode(node) {
                onNodeSelected = onNodeSelected
            };
            
            AddElement(graphNode);
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
        /// Creates a scriptable object sub asset for the current resolution graph and adds it to the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public ReanimatorNode CreateSubAsset(Type type, string assetName = null)
        {
            ReanimatorNode node = ScriptableObject.CreateInstance(type) as ReanimatorNode;

            // ReSharper disable once PossibleNullReferenceException
            node.name = string.IsNullOrEmpty(assetName) ? type.Name : assetName;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(graph, "Resolution Tree");
            graph.nodes.Add(node);
            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, graph);
            }

            Undo.RegisterCreatedObjectUndo(node, "Resolution Tree");
            
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            return node;
        }
        
        /// <summary>
        /// Removes the scriptable object sub asset from the resolution graph and removes it from the list
        /// of nodes saved in the resolution graph
        /// </summary>
        /// <param name="node"></param>
        private void DeleteSubAsset(ReanimatorNode node)
        {
            Undo.RecordObject(graph, "Resolution Tree");
            graph.nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
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
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = child;
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Add(child);
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(switchNode);
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
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(rootNode);
                    break;
                case OverrideNode overrideNode:
                    Undo.RecordObject(overrideNode, "Resolution Tree");
                    overrideNode.next = null;
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(overrideNode);
                    break;
                case SwitchNode switchNode:
                    Undo.RecordObject(switchNode, "Resolution Tree");
                    switchNode.nodes.Remove(child);
                    EditorUtility.SetDirty(graph);
                    EditorUtility.SetDirty(switchNode);
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
                    case ReanimatorGraphNode nodeDisplay:
                        DeleteSubAsset(nodeDisplay.node);
                        break;
                    case Edge edge:
                        var parent = edge.output.node as ReanimatorGraphNode;
                        var child = edge.input.node as ReanimatorGraphNode;
                        RemoveChild(parent?.node, child?.node);
                        break;
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge => {
                var parent = edge.output.node as ReanimatorGraphNode;
                var child = edge.input.node as ReanimatorGraphNode;

                AddChild(parent?.node, child?.node);
            });

            return graphViewChange;
        }
        
        private void PlayAnimationPreview() => GraphNodes.ForEach(node => {
            if (node.node is SimpleAnimationNode) node.PlayAnimationPreview();
        });
        
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

        public ResolutionGraph graph;
        private ReanimatorGraphEditor editorWindow;
        private ReanimatorSearchWindowProvider searchWindowProvider;
        public Action<ReanimatorGraphNode> onNodeSelected;
        
        private List<ReanimatorGraphNode> GraphNodes => nodes.ToList().Cast<ReanimatorGraphNode>().ToList();

        private const string styleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uss";
        public readonly Vector2 BlockSize = new Vector2(300, 200);
    }
}