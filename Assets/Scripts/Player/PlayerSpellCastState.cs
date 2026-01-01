using UnityEngine;

public class PlayerSpellCastState : EntityState
{
    private Player player;
    private Player_MagicController magicController;

    public PlayerSpellCastState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
        this.magicController = player.GetComponent<Player_MagicController>();
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0); // Stop moving while casting
        magicController.ConsumeMana(); // Deduct mana immediately
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // Exit state when animation finishes
        if (triggerCalled)
        {
            if (player.isGrounded)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
                stateMachine.ChangeState(player.fallState);
        }
    }
}