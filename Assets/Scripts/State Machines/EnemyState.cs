using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        this.enemy = enemy;

    }
    public override void Update()
    {
        base.Update();
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     stateMachine.ChangeState(enemy.attackState);
        // }
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        enemy.animator.SetFloat("moveAnimSpeedMultiplier",enemy.moveAnimSpeedMultiplier);
        enemy.animator.SetFloat("battleAnimSpeedMultiplier",battleAnimSpeedMultiplier);
        enemy.animator.SetFloat("xVelocity",enemy.rb.linearVelocity.x);
    }

    
}
