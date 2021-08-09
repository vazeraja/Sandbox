
namespace Aarthificial.Reanimation.Nodes {
    
    public class RootNode : ReanimatorNode {
        
        public ReanimatorNode root;

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            return null;
        }
        public override ReanimatorNode Copy()
        {
            RootNode node = Instantiate(this);
            node.root = root.Copy();
            return node;
        }
    }
}