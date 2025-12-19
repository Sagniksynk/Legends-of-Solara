using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    private float stateTimer;
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0, enemy.rb.linearVelocity.y);
        stateTimer = enemy.idleTime;
    }
    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
