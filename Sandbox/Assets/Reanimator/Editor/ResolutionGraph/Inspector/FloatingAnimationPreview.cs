using UnityEditor.Experimental.GraphView;

namespace Aarthificial.Reanimation {
    public class FloatingAnimationPreview : FloatingGraphElement {
        private ReanimatorGraphView reanimatorGraphView;

        public FloatingAnimationPreview()
        {
            title = "Animation Preview";
        }

        protected override void Initialize(GraphView graphView)
        {
            reanimatorGraphView = graphView as ReanimatorGraphView;
        }
    }
}