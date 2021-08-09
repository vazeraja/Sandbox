using ThunderNut.StateMachine;

public class EnterMovementStateDecision : Decision
{
    public PlayerController player => agent as PlayerController;

    public override bool Decide() {
        return player.State == AnimState.Movement;
    }
}


