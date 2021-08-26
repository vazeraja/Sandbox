using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Aarthificial.Reanimation {

    public class ReanimatorPort : Port {

        private const string styleName = "PortView";

        public ReanimatorGraphNode ownerNode;
        public event UnityAction<ReanimatorPort, Edge> OnConnected;
        public event UnityAction<ReanimatorPort, Edge> OnDisconnected;

        private List<Edge> edges = new List<Edge>();

        protected FieldInfo fieldInfo;

        public ReanimatorPort(ReanimatorGraphNode reanimatorGraphNode, Direction direction, Capacity capacity) : base(Orientation.Horizontal, direction,
            capacity, typeof(bool))
        {
            ownerNode = reanimatorGraphNode;
            var connectorListener = new EdgeConnectorListener(reanimatorGraphNode.reanimatorGraphView);
            m_EdgeConnector = new EdgeConnector<ReanimatorEdge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);

            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));
        }
        public override void Connect(Edge edge)
        {
            OnConnected?.Invoke(this, edge);
            base.Connect(edge);

            var inputNode = (edge.input as ReanimatorPort)?.ownerNode;
            var outputNode = (edge.output as ReanimatorPort)?.ownerNode;

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

            var inputNode = (edge.input as ReanimatorPort)?.ownerNode;
            var outputNode = (edge.output as ReanimatorPort)?.ownerNode;
            
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