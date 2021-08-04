using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Aarthificial.Reanimation.ResolutionGraph.Editor {
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

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            var saveService = GraphSaveService.GetInstance(graphView);
            saveService.Save();
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            var saveService = GraphSaveService.GetInstance(graphView);
            saveService.Save();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }
    }
}