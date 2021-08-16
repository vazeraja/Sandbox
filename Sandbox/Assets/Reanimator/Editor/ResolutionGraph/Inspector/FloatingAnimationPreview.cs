using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class FloatingAnimationPreview : FloatingGraphElement {
        private ReanimatorGraphView reanimatorGraphView;

        public FloatingAnimationPreview()
        {
            title = "Animation Preview";
            scrollable = true;
            
            // content.Add(new IMGUIContainer(() => {
            //     if (GUILayout.Button("Yeehaw")) {
            //         Debug.Log("Yeehaw");
            //     }
            // }));
        }

        protected override void Initialize(GraphView graphView)
        {
            reanimatorGraphView = graphView as ReanimatorGraphView;
        }
    }
}