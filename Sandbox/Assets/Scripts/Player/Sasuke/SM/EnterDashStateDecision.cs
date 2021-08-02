using ThunderNut.StateMachine;

public class EnterDashStateDecision : Decision
{
    public PlayerController player => agent as PlayerController;

    public override bool Decide() {
        return player.State == AnimState.Dash;
    }
}


