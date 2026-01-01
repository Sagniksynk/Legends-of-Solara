using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_StunnedState stunnedState;

    [Header("Battle Details")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float attackCoolDown = 1.5f;
    [HideInInspector] public float lastTimeAttacked;
    public float battleTime = 4f;

    public float minRetreatDistance = 1f;
    public Vector2 retreatVelocity;

    [Header("Movement Details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("Player Detection Details")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;

    private float defaultMoveSpeed;
    private float defaultBattleSpeed;
    private Vector2 defaultRetreatVelocity;

    public override void Awake()
    {
        base.Awake();
        defaultMoveSpeed = moveSpeed;
        defaultBattleSpeed = battleMoveSpeed;
        defaultRetreatVelocity = retreatVelocity;
    }

    public RaycastHit2D PlayerDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Entity_Stats targetStats = hit.collider.GetComponent<Entity_Stats>();
            if (targetStats != null && targetStats.currentHealth <= 0)
            {
                return default;
            }
            return hit;
        }
        return default;
    }

    public virtual bool IsSafeToWalk()
    {
        return true;
    }

    public override void DamageImpact(Transform damageSource)
    {
        base.DamageImpact(damageSource);

        // Don't change battle target or state when chilled (prevents turning/resetting)
        if (isChilled)
            return;

        battleState.SetTarget(damageSource);

        if (stateMachine.currentState == stunnedState)
            return;
        if (stateMachine.currentState == attackState)
            return;
        if (stateMachine.currentState == battleState)
            return;

        stateMachine.ChangeState(battleState);
    }

    protected override void ApplySlow(float slowPercentage)
    {
        // Slow down ALL movement speeds
        moveSpeed = defaultMoveSpeed * (1 - slowPercentage);
        battleMoveSpeed = defaultBattleSpeed * (1 - slowPercentage);
        retreatVelocity = defaultRetreatVelocity * (1 - slowPercentage);

        // Slow down animation
        animator.speed = 1 - slowPercentage;
    }

    protected override void RestoreSpeed()
    {
        moveSpeed = defaultMoveSpeed;
        battleMoveSpeed = defaultBattleSpeed;
        retreatVelocity = defaultRetreatVelocity;
        animator.speed = 1f;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * minRetreatDistance), playerCheck.position.y));
    }
}