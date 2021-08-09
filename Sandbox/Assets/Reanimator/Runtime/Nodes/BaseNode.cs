
namespace Aarthificial.Reanimation.Nodes {
    
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