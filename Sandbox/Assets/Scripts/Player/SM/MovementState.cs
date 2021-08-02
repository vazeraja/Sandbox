using ThunderNut.StateMachine;
using UnityEngine;


public class MovementState : State {
    public PlayerController player => agent as PlayerController;

    public override void Enter()
    {
        player.State = AnimState.Movement;
    }

    public override void Update()
    { }

    public override void FixedUpdate()
    {
        UpdateMovementState();
    }

    public override void Exit()
    { }


    private void UpdateMovementState()
    {
        var previousVelocity = player.collisionDetection.Velocity;
        var velocityChange = Vector2.zero;

        if (player.MovementDirection.x > 0)
            player.FacingDirection = 1;
        else if (player.MovementDirection.x < 0)
            player.FacingDirection = -1;

        if (player.wantsToJump && player.IsJumping) {
            player.wasOnTheGround = false;
            float currentJumpSpeed = player.IsFirstJump
                ? player.firstJumpSpeed
                : player.secondJumpSpeed;
            currentJumpSpeed *= player.jumpFallOff.Evaluate(player.JumpCompletion);
            velocityChange.y = currentJumpSpeed - previousVelocity.y;

            if (player.collisionDetection.ceilingContact.HasValue)
                player.jumpStopwatch.Reset();
        }
        else if (player.collisionDetection.groundContact.HasValue) {
            player.jumpsLeft = player.numberOfJumps;
            player.wasOnTheGround = true;
            player.canDash = true;
        }
        else {
            if (player.wasOnTheGround) {
                player.jumpsLeft -= 1;
                player.wasOnTheGround = false;
            }

            velocityChange.y = (-player.fallSpeed - previousVelocity.y) / 8;
        }

        velocityChange.x = player.IsCrouching
            ? (player.MovementDirection.x * player.crouchSpeed - previousVelocity.x) / 4
            : (player.MovementDirection.x * player.walkSpeed - previousVelocity.x) / 4;

        if (player.collisionDetection.wallContact.HasValue) {
            var wallDirection = (int) Mathf.Sign(player.collisionDetection.wallContact.Value.point.x -
                                                 player.transform.position.x);
            var walkDirection = (int) Mathf.Sign(player.MovementDirection.x);

            if (walkDirection == wallDirection)
                velocityChange.x = 0;
        }

        player.collisionDetection.rigidbody2D.AddForce(velocityChange, ForceMode2D.Impulse);
    }
}