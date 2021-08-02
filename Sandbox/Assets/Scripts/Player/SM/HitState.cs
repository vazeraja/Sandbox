using UnityEngine;
using ThunderNut.StateMachine;

public class HitState : State {
    public PlayerController player => agent as PlayerController;

    public override void Enter() {
        var relativePosition =
            (Vector2) player.transform.InverseTransformPoint(player.collisionData.transform.position);
        var direction = (player.collisionDetection.rigidbody2D.centerOfMass - relativePosition).normalized;

        player.hitStopwatch.Split();
        player.collisionDetection.rigidbody2D.AddForce(
            direction * player.hitForce - player.collisionDetection.Velocity,
            ForceMode2D.Impulse
        );
    }

    public override void Update() { }

    public override void FixedUpdate() {
        player.FacingDirection = player.collisionDetection.rigidbody2D.velocity.x < 0 ? -1 : 1;

        player.collisionDetection.rigidbody2D.AddForce(Physics2D.gravity * 4);
        if (player.hitStopwatch.IsFinished &&
            (player.collisionDetection.IsGrounded || player.collisionDetection.IsTouchingWall)) {
            player.hitStopwatch.Split();
            player.EnterMovementState();
        }
    }

    public override void Exit() { }
}