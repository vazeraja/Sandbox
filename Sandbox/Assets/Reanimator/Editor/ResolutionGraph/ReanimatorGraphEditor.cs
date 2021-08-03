using System;
using Aarthificial.Reanimation.Editor;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {

    public interface IEditorWindow {
        
    }

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
        private InspectorCustomControl inspector;
        private VisualElement animationPreview;
        public Label previewPanelLabel;

        private void OnEnable()
        {
            
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(styleSheet);

            editorGraph = root.Q<ReanimatorGraphView>();
            inspector = root.Q<InspectorCustomControl>();
            animationPreview = root.Q<VisualElement>("animation-preview");
            toolbarMenu = root.Q<ToolbarMenu>();


            var behaviourTrees = Helpers.LoadAssetsOfType<ResolutionGraph>();
            behaviourTrees.ForEach(graph => {
                toolbarMenu.menu.AppendAction($"{graph.name}", (a) => {
                    Selection.activeObject = graph;
                    OnSelectionChange();
                });
            });
            
            Label previewLabel = root.Q<Label>("preview-panel-title");

            if (resolutionGraph == null) {
                OnSelectionChange();
            }
            else {
                SelectTree(resolutionGraph);
            }
        }
        
        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () => {
                ResolutionGraph graph = Selection.activeObject as ResolutionGraph;
                if (!graph) {
                    if (Selection.activeGameObject) {
                        Reanimator reanimator = Selection.activeGameObject.GetComponent<Reanimator>();
                        if (reanimator) {
                            graph = reanimator.graph.Value;
                        }
                    }
                }

                SelectTree(graph);
            };
        }

        private void SelectTree(ResolutionGraph newGraph)
        {
            if (editorGraph == null || !newGraph) {
                return;
            }

            resolutionGraph = newGraph;

            if (Application.isPlaying) {
                editorGraph.Initialize(this, resolutionGraph, inspector, animationPreview);
            }
            else {
                editorGraph.Initialize(this, resolutionGraph, inspector, animationPreview);
            }

            EditorApplication.delayCall += () => { editorGraph.FrameAll(); };
        }
    }
}