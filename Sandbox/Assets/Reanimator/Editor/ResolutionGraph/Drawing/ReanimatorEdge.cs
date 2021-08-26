using System;
using Aarthificial.Reanimation.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    
    public class ReanimatorEdge : Edge {
        public EdgeData edgeData => userData as EdgeData;

        public bool isConnected = false;
        readonly string edgeStyle = "Styles/EdgeView";
        protected ReanimatorGraphView owner => ((input ?? output) as ReanimatorPort)?.ownerNode.reanimatorGraphView;

        public ReanimatorEdge() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
            AddToClassList("reanimatorEdge");
        }

        public override void OnSelected()
        {
            base.OnSelected();
            RemoveFromClassList("unselected");
            AddToClassList("selected");
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            RemoveFromClassList("selected");
            AddToClassList("unselected");
        }
    }
}