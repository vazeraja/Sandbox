using UnityEngine.UIElements;

namespace Aarthificial.Reanimation {
    public class TwoPanelCustomControl : TwoPaneSplitView {
        public new class UxmlFactory : UxmlFactory<TwoPanelCustomControl, TwoPaneSplitView.UxmlTraits> { }
    }
}
