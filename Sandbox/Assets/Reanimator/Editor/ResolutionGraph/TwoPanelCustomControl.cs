using UnityEngine.UIElements;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
    public class TwoPanelCustomControl : TwoPaneSplitView {
        public new class UxmlFactory : UxmlFactory<TwoPanelCustomControl, TwoPaneSplitView.UxmlTraits> { }
    }
}
