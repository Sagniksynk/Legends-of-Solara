using UnityEngine;

public class PlayerWallSlideState : EntityState
{
    private Player player;
    public PlayerWallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0); // Stop immediately upon touching wall
    }

    public override void Update()
    {
        base.Update();

        if (player.jumpInput)
        {
            player.SetVelocity(player.wallJumpForce.x * -player.facingDirection, player.wallJumpForce.y);
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (player.isGrounded)
        {
            // Fix: Explicitly zero out velocity to prevent "sliding" on the floor for 1 frame
            player.SetVelocity(0, 0); 
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (!player.isTouchingWall || player.moveInput.x * player.facingDirection < 0)
        {
            stateMachine.ChangeState(player.fallState);
            return; // Added return for safety
        }

        // Apply slide speed
        player.SetVelocity(0, -player.wallSlideSpeed);
    }
}