using UnityEngine;

public class PlayerAiredState : EntityState
{
    private Player player;
    public PlayerAiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }

    public override void Update()
    {
        base.Update();
        if (player.jumpInput && Time.time < player.lastGroundedTime + player.coyoteTime)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.dashInput && player.CanDash())
        {
            stateMachine.ChangeState(player.dashState);
        }
        if (player.isTouchingWall && player.moveInput.x * player.facingDirection >= 0 && Time.time > player.lastGroundedTime + player.wallJumpGracePeriod)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        if (player.moveInput.x != 0)
        {
            player.SetVelocity(player.moveInput.x * (player.movespeed * player.inAirMoveSpeedMultiplier), player.rb.linearVelocity.y);
        }
    }
}