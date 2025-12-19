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

        player.UseDash(); // Trigger the cooldown
        dashStartTime = Time.time;

        // Determine dash direction based on player input
        // If no input, dash in the direction the player is facing
        dashDirection = player.moveInput;
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(player.facingDirection, 0);
        }
        else
        {
            dashDirection.Normalize(); // Normalize for consistent speed in all directions
        }

        // Freeze player vertically and apply dash speed in the chosen direction
        player.rb.gravityScale = 0;
        player.SetVelocity(player.dashSpeed * dashDirection.x, player.dashSpeed * dashDirection.y);
    }

    public override void Update()
    {
        base.Update();
        
        // Face the direction of the dash if there's horizontal movement
        if (dashDirection.x != 0)
        {
            player.HandleFlip(dashDirection.x);
        }

        // After dash duration, transition to another state
        if (Time.time >= dashStartTime + player.dashDuration)
        {
            // Apply a small residual velocity for a less abrupt stop
            player.SetVelocity(player.movespeed * dashDirection.x * 0.5f, 0); 

            if (player.isGrounded)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        // Restore gravity
        player.rb.gravityScale = player.defaultGravity;
        // Don't reset velocity here to allow for the residual velocity from Update()
    }
}