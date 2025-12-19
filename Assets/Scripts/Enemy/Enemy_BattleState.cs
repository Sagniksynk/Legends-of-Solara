using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Entity_Health playerHealth;
    private int moveDir;
    private float battleTimer;
    private float retreatTimer; 

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        retreatTimer = 0; 
        
        if (player == null)
        {
            player = enemy.PlayerDetection().transform;
        }
        if(player != null)
        {
            playerHealth = player.GetComponent<Entity_Health>();
            if(playerHealth != null)
            {
                playerHealth.OnDie += HandleTargetDeath;
            }
        }
        if (player != null && ShouldRetreat()) 
        {
            int playerDir = DirectionToPlayer();

            if (playerDir != enemy.facingDirection)
            {
                enemy.Flip();
            }

            enemy.rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -playerDir, enemy.retreatVelocity.y);
            
            retreatTimer = 1f; 
        }

        battleTimer = enemy.battleTime;
    }
    public override void Exit()
    {
        base.Exit();
        if(playerHealth !=null)
        {
            playerHealth.OnDie -= HandleTargetDeath;
        }
    }

    private void HandleTargetDeath()
    {
        stateMachine.ChangeState(enemy.idleState);
    }

    public override void Update()
    {
        base.Update();
        if(playerHealth!=null && playerHealth.isDead)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        if (retreatTimer > 0)
        {
            retreatTimer -= Time.deltaTime;
            if(retreatTimer< 0.9f && enemy.isGrounded)
            {
                retreatTimer = 0;
            }
            else
            {
                return;
            }
        }
        if (player == null)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        if (enemy.PlayerDetection())
        {
            battleTimer = enemy.battleTime;
        }
        else
        {
            if(battleTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 15)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
            battleTimer -= Time.deltaTime;
        }
        moveDir = DirectionToPlayer();

        if (WithinAttackRange() && moveDir == enemy.facingDirection)
        {
            if (CanAttack())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            enemy.SetVelocity(enemy.battleMoveSpeed * moveDir, enemy.rb.linearVelocity.y);
        }
    }

    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;
    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;
    private bool CanAttack()
    {
        if(Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }
    private float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null) return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
    public void SetTarget(Transform target)
    {
        this.player = target;
        if (player != null)
        {
            playerHealth = player.GetComponent<Enemy_Health>();
        }
    }
}