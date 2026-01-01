using UnityEngine;

public class PlayerDashState : EntityState
{
    private Player player;
    private float dashStartTime;
    private Vector2 dashDirection;

    public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();

        player.UseDash();
        dashStartTime = Time.time;

        // Direction Logic
        dashDirection = player.moveInput;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(player.facingDirection, 0);
        }
        else
        {
            dashDirection.Normalize();
        }

        // 1. Turn off Gravity
        player.rb.gravityScale = 0;

        // 2. Apply Speed
        player.SetVelocity(player.dashSpeed * dashDirection.x, player.dashSpeed * dashDirection.y);
    }

    public override void Update()
    {
        base.Update();

        // Face Direction
        if (dashDirection.x != 0)
        {
            player.HandleFlip(dashDirection.x);
        }

        // Timer Logic
        if (Time.time >= dashStartTime + player.dashDuration)
        {
            // Note: We do NOT set velocity here anymore. 
            // We let the next state (Idle/Fall) handle physics.
            // Setting velocity here is risky if we transition immediately.

            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // 3. RESTORE GRAVITY (Crucial)
        // Ensure 'player.defaultGravity' is actually set to something like 3 or 5 in Player.cs!
        // If unsure, hardcode it to 1f or 3f to test.
        player.rb.gravityScale = player.defaultGravity;

        // 4. KILL VERTICAL MOMENTUM (The Fix for "Flying Up")
        // If we dashed Up-Right, we have huge Y velocity. 
        // We must kill that so we don't launch into space when gravity returns.
        player.SetVelocity(player.rb.linearVelocity.x, 0);
    }
}