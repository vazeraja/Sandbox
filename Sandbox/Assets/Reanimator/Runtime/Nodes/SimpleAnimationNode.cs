using System;
using System.Collections.Generic;
using System.Linq;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "simple_animation", menuName = "Reanimator/Simple Animation", order = 400)]
    public class SimpleAnimationNode : AnimationNode<SimpleCel> {
        public IEnumerable<SimpleCel> sprites { 
            get => cels;
            set => cels = value as SimpleCel[];
        }
        public ControlDriver ControlDriver {
            get => controlDriver;
            set => controlDriver = value;
        }

        public DriverDictionary Drivers {
            get => drivers;
            set => drivers = value;
        }
    }
}