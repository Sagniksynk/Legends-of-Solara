using UnityEngine;

public class PlayerJumpState : PlayerAiredState
{
    private Player player;
    public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(player.rb.linearVelocity.x, player.jumpForce);

    }
    public override void Update()
    {
        base.Update();
        if (player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }

    }
}
