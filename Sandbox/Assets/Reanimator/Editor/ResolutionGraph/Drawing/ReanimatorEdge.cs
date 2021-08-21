using UnityEditor.Experimental.GraphView;

namespace Aarthificial.Reanimation {
    public class ReanimatorEdge : Edge {
        
        public bool isConnected = false;
        protected ReanimatorGraphView owner => ((input ?? output) as ReanimatorPort)?.owner.reanimatorGraphView;
        
    }
}