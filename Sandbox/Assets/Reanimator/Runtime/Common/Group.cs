using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.Nodes;
using UnityEngine;

namespace Aarthificial.Reanimation {


    [Serializable]
    public class Group {
        public List<string> innerNodeGUIDs = new List<string>();
        public string title = "Comment Block";
        public Rect position;


        public Color color = new Color(0, 0, 0, 0.3f);
        public Vector2 size;

        public Group() { }

        public Group(string title, Vector2 position)
        {
            this.title = title;
            this.position.position = position;
        }

        public virtual void OnCreated()
        {
            size = new Vector2(400, 200);
            position.size = size;
        }
    }
    
}