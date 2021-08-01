using System;
using System.Collections.Generic;
using Aarthificial.Reanimation.ResolutionGraph.Editor;

namespace Aarthificial.Reanimation.Nodes {
    
    [Serializable]
    public class SaveData {
        public List<GroupBlock> groupBlocks = new List<GroupBlock>();
    }
    
    public class BaseNode : ReanimatorNode {
        
        public ReanimatorNode root;

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return null;
        }
        public override ReanimatorNode Copy()
        {
            BaseNode node = Instantiate(this);
            node.root = root.Copy();
            return node;
        }
    }
}