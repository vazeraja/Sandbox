﻿using System;
using Aarthificial.Reanimation.Common;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public abstract class FloatingGraphElement : GraphElement {
        public FloatingElement FloatingElement;
        
        private VisualElement main;
        protected readonly VisualElement root;
        protected readonly VisualElement content;
        protected readonly VisualElement header;


        protected readonly ScrollView scrollView;
        private readonly Label titleLabel;
        
        private bool _scrollable;
        private bool _resizable;

        protected event Action onResized;
        
        private static readonly string pinnedElementStyle =
            "Assets/Reanimator/Editor/ResolutionGraph/Inspector/FloatingElement.uss";

        private static readonly string UXMLName = "FloatingElement";

        public sealed override string title {
            get => titleLabel.text;
            set => titleLabel.text = value;
        }

        private readonly Resizer resizer;
        protected bool resizable {
            get => _resizable;
            set {
                if (_resizable == value)
                    return;

                _resizable = value;

                if (_resizable) {
                    this.hierarchy.Add(resizer);
                }
                else {
                    this.hierarchy.Remove(resizer);
                }
            }
        }

        protected bool scrollable {
            get => _scrollable;
            set {
                if (_scrollable == value)
                    return;

                _scrollable = value;

                style.position = Position.Absolute;
                if (_scrollable) {
                    content.RemoveFromHierarchy();
                    root.Add(scrollView);
                    scrollView.Add(content);
                    AddToClassList("scrollable");
                }
                else {
                    scrollView.RemoveFromHierarchy();
                    content.RemoveFromHierarchy();
                    root.Add(content);
                    RemoveFromClassList("scrollable");
                }
            }
        }

        protected FloatingGraphElement()
        {
            var tpl = Resources.Load<VisualTreeAsset>($"UXML/{UXMLName}");
            //styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(pinnedElementStyle));

            main = tpl.CloneTree();
            main.AddToClassList("mainContainer");

            root = main.Q("content");
            header = main.Q("header");
            titleLabel = main.Q<Label>(name: "titleLabel");
            content = main.Q<VisualElement>(name: "contentContainer");

            hierarchy.Add(main);

            capabilities |= Capabilities.Movable | Capabilities.Resizable;
            style.overflow = Overflow.Hidden;

            ClearClassList();
            AddToClassList("pinnedElement");

            this.AddManipulator(new Dragger {clampToParentEdges = true});

            scrollable = false;
            scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            resizer = new Resizer(() => onResized?.Invoke());
            resizable = true;

            RegisterCallback<DragUpdatedEvent>(e => { e.StopPropagation(); });

            title = "PinnedElementView";
        }

        public void InitializeGraphView(FloatingElement floatingElement, GraphView graphView)
        {
            this.FloatingElement = floatingElement;
            SetPosition(floatingElement.position);

            onResized += () => { floatingElement.position.size = layout.size; };

            RegisterCallback<MouseUpEvent>(e => { floatingElement.position.position = layout.position; });

            Initialize(graphView);
        }

        public void ResetPosition()
        {
            // FloatingElement.position = new Rect(Vector2.zero, new Vector2(150, 200));
            SetPosition(FloatingElement.position);
        }

        protected abstract void Initialize(GraphView graphView);

        ~FloatingGraphElement()
        {
            Destroy();
        }

        protected virtual void Destroy() { }
    }
}