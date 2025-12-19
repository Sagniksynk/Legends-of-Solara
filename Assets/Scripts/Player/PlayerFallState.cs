using UnityEngine;

public class PlayerFallState : PlayerAiredState
{
    private Player player;
    public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
    }

    public override void Update()
    {
        base.Update(); // This runs the PlayerAiredState logic, including air movement

        if (player.isGrounded)
        {
            // NEW, SMARTER LANDING LOGIC:
            // Check if the player is holding a move button.
            bool isPushingIntoWall = player.isTouchingWall && player.moveInput.x * player.facingDirection > 0;

            if (player.moveInput.x != 0 && !isPushingIntoWall)
            {
                // If they are, land directly in the Move state.
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                // If not, land in the Idle state.
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
}
