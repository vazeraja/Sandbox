using ThunderNut.StateMachine;
public class EnterHitStateDecision : Decision {
    
    public PlayerController player => agent as PlayerController;

    public override bool Decide() {
        return player.State == AnimState.Hit;
    }
}