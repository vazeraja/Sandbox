using System;
using System.Collections.Generic;
using System.Reflection;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Aarthificial.Reanimation {
    public class ReanimatorPort : Port {

        private const string styleName = "PortView";

        public ReanimatorGraphNode owner;
        public event UnityAction<ReanimatorPort, Edge> OnConnected;
        public event UnityAction<ReanimatorPort, Edge> OnDisconnected;

        private List<Edge> edges = new List<Edge>();

        protected FieldInfo fieldInfo;

        public ReanimatorPort(Direction direction, Capacity capacity) : base(Orientation.Horizontal, direction,
            capacity, typeof(bool))
        {
            var connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);

            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));
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

            inputNode?.OnPortConnected(edge.input as ReanimatorPort, edge);
            outputNode?.OnPortConnected(edge.output as ReanimatorPort, edge);
        }

        public override void Disconnect(Edge edge)
        {
            OnDisconnected?.Invoke(this, edge);
            base.Disconnect(edge);
            
            // if (!(edge as Edge).isConnected)
            //     return ;

            var inputNode = (edge.input as ReanimatorPort)?.owner;
            var outputNode = (edge.output as ReanimatorPort)?.owner;
            
            inputNode?.OnPortDisconnected(edge.input as ReanimatorPort, edge);
            outputNode?.OnPortDisconnected(edge.output as ReanimatorPort, edge);
            
            edges.Remove(edge);
        }

        // public override bool ContainsPoint(Vector2 localPoint) {
        //     Rect rect = new Rect(0, 0, layout.width, layout.height);
        //     return rect.Contains(localPoint);
        // }
    }
}