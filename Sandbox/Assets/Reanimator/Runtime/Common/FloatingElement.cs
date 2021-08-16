using System;
using UnityEngine;

namespace Aarthificial.Reanimation.Common {
    
    [System.Serializable]
    public class FloatingElement {
        public static readonly Vector2 defaultSize = new Vector2(250, 250);
        public Rect position = new Rect(Vector2.zero, defaultSize);
        public string title = "Animation Preview";
        public bool opened;
    }
}