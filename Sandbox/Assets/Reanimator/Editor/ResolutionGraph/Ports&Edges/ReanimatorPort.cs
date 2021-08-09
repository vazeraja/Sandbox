using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Aarthificial.Reanimation {
    public class ReanimatorPort : Port {
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();

                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position) { }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);

                // We can't just add these edges to delete to the m_GraphViewChange
                // because we want the proper deletion code in GraphView to also
                // be called. Of course, that code (in DeleteElements) also
                // sends a GraphViewChange.
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.input.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (edge.output.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.output.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                if (m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements(m_EdgesToDelete);

                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null) {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge e in edgesToCreate) {
                    graphView.AddElement(e);
                    edge.input.Connect(e);
                    edge.output.Connect(e);
                }
            }
        }

        private class PortData : IEquatable<PortData> {
            /// <summary>
            /// Unique identifier for the port
            /// </summary>
            public string identifier;

            /// <summary>
            /// Display name on the node
            /// </summary>
            public string displayName;

            /// <summary>
            /// The type that will be used for coloring with the type stylesheet
            /// </summary>
            public Type displayType;

            /// <summary>
            /// If the port accept multiple connection
            /// </summary>
            public bool acceptMultipleEdges;

            /// <summary>
            /// Port size, will also affect the size of the connected edge
            /// </summary>
            public int sizeInPixel;

            /// <summary>
            /// Tooltip of the port
            /// </summary>
            public string tooltip;

            /// <summary>
            /// Is the port vertical
            /// </summary>
            public bool vertical;

            public bool Equals(PortData other)
            {
                return identifier == other.identifier
                       && displayName == other.displayName
                       && displayType == other.displayType
                       && acceptMultipleEdges == other.acceptMultipleEdges
                       && sizeInPixel == other.sizeInPixel
                       && tooltip == other.tooltip
                       && vertical == other.vertical;
            }

            public void CopyFrom(PortData other)
            {
                identifier = other.identifier;
                displayName = other.displayName;
                displayType = other.displayType;
                acceptMultipleEdges = other.acceptMultipleEdges;
                sizeInPixel = other.sizeInPixel;
                tooltip = other.tooltip;
                vertical = other.vertical;
            }
        }

        private const string portStyle = "Assets/Reanimator/Editor/ResolutionGraph/Styles/PortView.uss";

        private ReanimatorGraphNode owner;
        public event UnityAction<ReanimatorPort, Edge> OnConnected;
        public event UnityAction<ReanimatorPort, Edge> OnDisconnected;

        private List<Edge> edges = new List<Edge>();

        protected FieldInfo fieldInfo;

        public ReanimatorPort(Direction direction, Capacity capacity) : base(Orientation.Horizontal, direction,
            capacity, typeof(bool))
        {
            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(portStyle));
        }
        public virtual void Initialize(ReanimatorGraphNode graphNode, string name)
        {
            this.owner = graphNode;
        }
        public override void Connect(Edge edge)
        {
            OnConnected?.Invoke(this, edge);
            base.Connect(edge);

            var inputNode = (edge.input as ReanimatorPort)?.owner;
            var outputNode = (edge.output as ReanimatorPort)?.owner;

            edges.Add(edge);
        }

        public override void Disconnect(Edge edge)
        {
            OnDisconnected?.Invoke(this, edge);
            base.Disconnect(edge);

            var inputNode = (edge.input as ReanimatorPort)?.owner;
            var outputNode = (edge.output as ReanimatorPort)?.owner;

            edges.Remove(edge);
        }

        // public override bool ContainsPoint(Vector2 localPoint) {
        //     Rect rect = new Rect(0, 0, layout.width, layout.height);
        //     return rect.Contains(localPoint);
        // }
    }
}