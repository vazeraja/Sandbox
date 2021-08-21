using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation {
    public class EdgeConnectorListener : IEdgeConnectorListener {
        // public readonly ReanimatorGraphView graphView;
        //
        // Dictionary<Edge, ReanimatorPort> edgeInputPorts = new Dictionary<Edge, ReanimatorPort>();
        // Dictionary<Edge, ReanimatorPort> edgeOutputPorts = new Dictionary<Edge, ReanimatorPort>();
        //
        // static ReanimatorSearchWindowProvider edgeNodeCreateMenuWindow;
        //
        // public EdgeConnectorListener(ReanimatorGraphView graphView)
        // {
        //     this.graphView = graphView;
        // }
        //
        // public virtual void OnDropOutsidePort(Edge edge, Vector2 position)
        // {
        //     this.graphView.RegisterCompleteObjectUndo("Disconnect edge");
        //
        //     //If the edge was already existing, remove it
        //     if (!edge.isGhostEdge)
        //         graphView.Disconnect(edge as EdgeView);
        //
        //     // when on of the port is null, then the edge was created and dropped outside of a port
        //     if (edge.input == null || edge.output == null)
        //         ShowNodeCreationMenuFromEdge(edge as EdgeView, position);
        // }
        //
        // public virtual void OnDrop(GraphView graphView, Edge edge)
        // {
        //     var edgeView = edge as EdgeView;
        //     bool wasOnTheSamePort = false;
        //
        //     if (edgeView?.input == null || edgeView?.output == null)
        //         return;
        //
        //     //If the edge was moved to another port
        //     if (edgeView.isConnected) {
        //         if (edgeInputPorts.ContainsKey(edge) && edgeOutputPorts.ContainsKey(edge))
        //             if (edgeInputPorts[edge] == edge.input && edgeOutputPorts[edge] == edge.output)
        //                 wasOnTheSamePort = true;
        //
        //         if (!wasOnTheSamePort)
        //             this.graphView.Disconnect(edgeView);
        //     }
        //
        //     if (edgeView.input.node == null || edgeView.output.node == null)
        //         return;
        //
        //     edgeInputPorts[edge] = edge.input as PortView;
        //     edgeOutputPorts[edge] = edge.output as PortView;
        //     try {
        //         this.graphView.RegisterCompleteObjectUndo("Connected " + edgeView.input.node.name + " and " +
        //                                                   edgeView.output.node.name);
        //         if (!this.graphView.Connect(edge as EdgeView, autoDisconnectInputs: !wasOnTheSamePort))
        //             this.graphView.Disconnect(edge as EdgeView);
        //     }
        //     catch (System.Exception) {
        //         this.graphView.Disconnect(edge as EdgeView);
        //     }
        // }
        //
        // void ShowNodeCreationMenuFromEdge(EdgeView edgeView, Vector2 position)
        // {
        //     if (edgeNodeCreateMenuWindow == null)
        //         edgeNodeCreateMenuWindow = ScriptableObject.CreateInstance<CreateNodeMenuWindow>();
        //
        //     edgeNodeCreateMenuWindow.Initialize(graphView, EditorWindow.focusedWindow, edgeView);
        //     SearchWindow.Open(new SearchWindowContext(position + EditorWindow.focusedWindow.position.position),
        //         edgeNodeCreateMenuWindow);
        // }

        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;
        
        public EdgeConnectorListener()
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
            if (edge.input.capacity == Port.Capacity.Single)
                foreach (Edge edgeToDelete in edge.input.connections)
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
            if (edge.output.capacity == Port.Capacity.Single)
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
}