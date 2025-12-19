using UnityEngine;

public class Enemy_StunnedState : EnemyState
{
    private float stunTimer;
    public Enemy_StunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0, 0);
        enemy.rb.gravityScale = enemy.defaultGravity;
        enemy.rb.linearVelocity = new Vector2(-enemy.facingDirection * 7, 7);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        stunTimer -= Time.deltaTime;
        if(stunTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
    public void SetStunDuration(float duration)
    {
        stunTimer = duration;
    }
}
