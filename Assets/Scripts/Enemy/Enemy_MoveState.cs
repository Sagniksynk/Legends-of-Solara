using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        Enemy_Skeleton skeleton = enemy as Enemy_Skeleton;
        if(enemy.isTouchingWall || !skeleton.IsSafeToWalk())
        {
            enemy.Flip();
        }

    }
    public override void Update()
    {
        base.Update();
        Enemy_Skeleton skeleton = enemy as Enemy_Skeleton;
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection, enemy.rb.linearVelocity.y);
        if(enemy.isTouchingWall || !skeleton.IsSafeToWalk())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        
    }

}
