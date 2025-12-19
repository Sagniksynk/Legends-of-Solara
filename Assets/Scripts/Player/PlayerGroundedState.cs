using UnityEngine;

public class PlayerGroundedState : EntityState
{
    private Player player;
    public PlayerGroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }
    public override void Update()
    {
        base.Update();


        if (player.dashInput && player.CanDash())
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }


        if (!player.isGrounded && player.rb.linearVelocity.y<0.1f)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }


        if(player.jumpInput)
        {
            player.stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.attackInput)
        {
            player.stateMachine.ChangeState(player.basicAttackState);
            return;
        }
        if(player.HasBufferedJump() || player.jumpInput) 
        {
            player.UseJumpBuffer();
            player.stateMachine.ChangeState(player.jumpState);
            return;
        }
    }
}
