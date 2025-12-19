using UnityEngine;

public class PlayerBasicAttackState : EntityState
{
    private Player player;
    private float attackVelocityTimer;
    private const int FirstComboIndex = 1;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private bool comboAttackQueued;
    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        this.player = player;
        if (comboLimit != player.attackVelocities.Length)
        {
            comboLimit = player.attackVelocities.Length;
        }
    }
    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;

        if (Time.time > player.lastAttackTime + player.comboResetTime || comboIndex > comboLimit)
        {
            comboIndex = FirstComboIndex;
        }

        player.animator.SetInteger("basicAttackIndex", comboIndex);

        GenerateAttackVelocity();
        comboIndex++;
    }
    public override void Update()
    {
        base.Update();
        if (player.dashInput && player.CanDash())
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
        player.HandleFlip(player.moveInput.x);
        HandleAttackVelocity();
        if (player.attackInput)
        {
            QueueNextAttack();
        }
        if (triggerCalled)
        {
            if (comboAttackQueued)
            {
                player.animator.SetBool(animBoolName, false);
                player.EnterAttackStateWithDelay();
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.SetLastAttackTime();
    }
    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
        {
            comboAttackQueued = true;
        }
    }
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;
        if (attackVelocityTimer < 0)
        {
            player.SetVelocity(0, player.rb.linearVelocity.y);
        }
    }
    private void GenerateAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        int velocityIndex = comboIndex -1;
        if (velocityIndex < player.attackVelocities.Length)
        {
            Vector2 currentVelocity = player.attackVelocities[velocityIndex];
            player.SetVelocity(currentVelocity.x * player.facingDirection, currentVelocity.y);
        }
    }
}
