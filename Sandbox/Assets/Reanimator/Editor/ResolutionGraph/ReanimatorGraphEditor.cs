using System;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
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
        private InspectorCustomControl inspector;
        
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(styleSheet);

            editorGraph = root.Q<ReanimatorGraphView>();
            inspector = root.Q<InspectorCustomControl>();

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
                editorGraph.Initialize(resolutionGraph, this, inspector);
            }
            else {
                editorGraph.Initialize(resolutionGraph, this, inspector);
            }

            EditorApplication.delayCall += () => { editorGraph.FrameAll(); };
        }
    }
}