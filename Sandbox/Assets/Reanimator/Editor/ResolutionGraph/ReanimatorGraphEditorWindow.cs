using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Aarthificial.Reanimation {

    public class ReanimatorGraphEditorWindow : EditorWindow {
        [MenuItem("Reanimator/Resolution Graph")]
        public static void ShowWindow()
        {
            ReanimatorGraphEditorWindow wnd = GetWindow<ReanimatorGraphEditorWindow>();
            wnd.titleContent = new GUIContent("ReanimatorGraph");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (!(Selection.activeObject is ResolutionGraph)) return false;
            ShowWindow();
            return true;
        }
        private const string visualTreePath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uxml";
        private const string styleSheetPath = "Assets/Reanimator/Editor/ResolutionGraph/ReanimatorGraphEditor.uss";

        private ResolutionGraph resolutionGraph;
        
        private ReanimatorGraphView graphView;
        private ToolbarMenu toolbarMenu;
        private InspectorCustomControl inspectorPanel;
        private VisualElement animationPreviewPanel;
        private TwoPanelInspector twoPanelInspector;
        private ToolbarButton saveButton;
        private ToolbarButton loadButton;

        private void Update()
        {
            // if (graphView != null) {
            //     Debug.Log("Count of GraphNodes: " + graphView.GraphNodes.Count);
            //     Debug.Log("Count of GraphNodesByNode Dict: " + graphView.GraphNodesPerNode.Count);
            //     Debug.Log("Count of GroupViews: " + graphView.groupViews.Count);
            // }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(styleSheet);

            graphView = root.Q<ReanimatorGraphView>();
            
            inspectorPanel = root.Q<InspectorCustomControl>();
            animationPreviewPanel = root.Q<VisualElement>("animation-preview");
            twoPanelInspector = root.Q<TwoPanelInspector>("TwoPanelInspector");
            
            toolbarMenu = root.Q<ToolbarMenu>();
            saveButton = root.Q<ToolbarButton>("save-button");
            loadButton = root.Q<ToolbarButton>("load-button");
            
            var resolutionGraphs = Helpers.LoadAssetsOfType<ResolutionGraph>();
            Select(resolutionGraphs.First());
            resolutionGraphs.ForEach(graph => {
                toolbarMenu.menu.AppendAction($"{graph.name}", (a) => {
                    Select(graph);
                });
            });
            
            loadButton.clicked += () => {
                Helpers.SaveService(graphView).LoadFromSubAssets();
            };
            
            graphView.onNodeSelected = DrawNodeProperties;
        }

        private void DrawNodeProperties(ReanimatorGraphNode graphNode)
        {
            twoPanelInspector.Initialize(inspectorPanel, animationPreviewPanel);
            twoPanelInspector.DrawNodeProperties(graphNode);
        }

        private void Select(Object graph)
        {
            Selection.activeObject = graph;
            resolutionGraph = Selection.activeObject as ResolutionGraph;
            graphView.Initialize(this, resolutionGraph);
            EditorGUIUtility.PingObject(graph);
            EditorApplication.delayCall += () => { graphView.FrameAll(); };
        }
        
        
    }
}