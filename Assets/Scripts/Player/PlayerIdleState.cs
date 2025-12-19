using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    private Player player;
    public PlayerIdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
        this.player = player;
    }
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, 0f);
    }
    public override void Update()
    {
        base.Update();
        bool isPushingIntoWall = player.isTouchingWall && player.moveInput.x * player.facingDirection > 0;
        if (player.moveInput.x != 0 && !isPushingIntoWall)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
   
}
