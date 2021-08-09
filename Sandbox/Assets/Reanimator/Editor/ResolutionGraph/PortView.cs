using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation {
    public class PortView : Port {
        protected PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
            base(portOrientation, portDirection, portCapacity, type)
        {
            
            
            
        }
    }
}