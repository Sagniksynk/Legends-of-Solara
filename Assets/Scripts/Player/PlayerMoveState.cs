using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private Player player;
    public PlayerMoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
        this.player = player;
    }
    public override void Update()
    {
        base.Update();

        bool isPushingIntoWall = player.isTouchingWall && player.moveInput.x * player.facingDirection > 0;
        if (stateMachine.currentState != this)
            return;

        if (player.moveInput.x == 0 || isPushingIntoWall)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        
        player.SetVelocity(player.moveInput.x * player.movespeed, player.rb.linearVelocity.y);
    }
}
