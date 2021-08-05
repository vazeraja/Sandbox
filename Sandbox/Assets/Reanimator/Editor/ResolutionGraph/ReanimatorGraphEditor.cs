using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {

    public class ReanimatorGraphEditor : EditorWindow {
        [MenuItem("Reanimator/Resolution Graph")]
        public static void ShowWindow()
        {
            ReanimatorGraphEditor wnd = GetWindow<ReanimatorGraphEditor>();
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
        
        private ReanimatorGraphView editorGraph;
        private ToolbarMenu toolbarMenu;
        private InspectorCustomControl inspectorPanel;
        private VisualElement animationPreview;
        private TwoPanelInspector twoPanelInspector;
        private ToolbarButton saveButton;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(styleSheet);

            editorGraph = root.Q<ReanimatorGraphView>();
            inspectorPanel = root.Q<InspectorCustomControl>();
            animationPreview = root.Q<VisualElement>("animation-preview");
            twoPanelInspector = root.Q<TwoPanelInspector>("TwoPanelInspector");
            toolbarMenu = root.Q<ToolbarMenu>();
            saveButton = root.Q<ToolbarButton>("save-button");

            editorGraph.onNodeSelected = DrawNodeProperties;
            
            var resolutionGraphs = Helpers.LoadAssetsOfType<ResolutionGraph>();
            resolutionGraphs.ForEach(graph => {
                toolbarMenu.menu.AppendAction($"{graph.name}", (a) => {
                    Selection.activeObject = graph;
                    OnSelectionChange();
                });
            });

            saveButton.clicked += () => {
                resolutionGraph.saveData = Helpers.SaveService(editorGraph);
            };

            if (resolutionGraph == null) {
                OnSelectionChange();
            }
            else {
                SelectTree(resolutionGraph);
            }
        }

        private void DrawNodeProperties(ReanimatorGraphNode graphNode)
        {
            twoPanelInspector.Initialize(inspectorPanel, animationPreview);
            twoPanelInspector.DrawNodeProperties(graphNode);
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () => {
                ResolutionGraph graph = Selection.activeObject as ResolutionGraph;
                SelectTree(graph);
            };
        }

        private void OnDisable()
        {
            resolutionGraph.saveData = Helpers.SaveService(editorGraph);
        }

        private void SelectTree(ResolutionGraph newGraph)
        {
            if (editorGraph == null || !newGraph) {
                return;
            }

            resolutionGraph = newGraph;

            if (Application.isPlaying) {
                editorGraph.Initialize(this, resolutionGraph);
            }
            else {
                editorGraph.Initialize(this, resolutionGraph);
            }

            EditorApplication.delayCall += () => { editorGraph.FrameAll(); };
        }
    }
}