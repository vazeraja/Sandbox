using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation {
    public class EdgeConnectorListener : IEdgeConnectorListener {
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        private ReanimatorGraphView reanimatorGraphView;
        
        static ReanimatorSearchWindowProvider edgeNodeCreateMenuWindow;

        public EdgeConnectorListener(ReanimatorGraphView reanimatorGraphView)
        {
            this.reanimatorGraphView = reanimatorGraphView;
            
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // when on of the port is null, then the edge was created and dropped outside of a port
            // if (edge.input == null || edge.output == null)
            //     ShowNodeCreationMenuFromEdge(edge as ReanimatorEdge, position);
           
        }

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

        // void ShowNodeCreationMenuFromEdge(ReanimatorEdge edgeView, Vector2 position)
        // {
        //     if (edgeNodeCreateMenuWindow == null)
        //         edgeNodeCreateMenuWindow = ScriptableObject.CreateInstance<ReanimatorSearchWindowProvider>();
        //
        //     edgeNodeCreateMenuWindow.Initialize(EditorWindow.focusedWindow, graphView);
        //     SearchWindow.Open(new SearchWindowContext(position + EditorWindow.focusedWindow.position.position),
        //         edgeNodeCreateMenuWindow);
        // }
        
    }
}