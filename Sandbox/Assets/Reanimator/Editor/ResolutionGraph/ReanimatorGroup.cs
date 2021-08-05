using Aarthificial.Reanimation.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation {
    public sealed class ReanimatorGroup : Group {
        private GroupBlock block;
        private readonly ReanimatorGraphView graphView;

        public ReanimatorGroup(ReanimatorGraphView graphView, GroupBlock block)
        {
            this.block = block;
            this.graphView = graphView;
            autoUpdateGeometry = true;
            title = block.Title;
        }

        // protected override void OnGroupRenamed(string oldName, string newName)
        // {
        //     GraphSaveService.GetInstance(graphView).Save();
        // }

        // protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        // {
        //     GraphSaveService.GetInstance(graphView).Save();
        // }
 
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }
    }
}