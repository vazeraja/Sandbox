using Aarthificial.Reanimation.Editor;
using Aarthificial.Reanimation.Editor.Nodes;
using Aarthificial.Reanimation.Nodes;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation {
    public class InspectorCustomControl : ScrollView {
        public new class UxmlFactory : UxmlFactory<InspectorCustomControl, UxmlTraits> { }
    }
}