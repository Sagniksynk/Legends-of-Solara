using UnityEngine;

public class Enemy_Skeleton : Enemy, ICounterable
{
    [Header("Skeleton Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float skeletonGroundCheckDistance;
    private bool manualCounterWindow = false;
    private Enemy_Vfx enemyVfx;
    public override void Awake()
    {
        base.Awake();
        enemyVfx = GetComponent<Enemy_Vfx>();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this,stateMachine,"attack");
        battleState = new Enemy_BattleState(this,stateMachine,"battle");
        stunnedState = new Enemy_StunnedState(this,stateMachine,"stunned");
        deadState = new EntityDeadState(this, stateMachine, "die");
    }
    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    public override void Update()
    {
        base.Update();
        if(stateMachine.currentState != attackState)
        {
            if (manualCounterWindow)
            {
                manualCounterWindow = false;
                if(enemyVfx != null) enemyVfx.EnableAttackAlert(false);
            }
        }
    }
    // public bool IsGroundDetected()
    // {
    //     return Physics2D.Raycast(groundCheck.position, Vector2.down, skeletonGroundCheckDistance, whatIsGround);
    // }
    public bool CanBeCountered()
    {
        return manualCounterWindow;
    }
    public void StunFor(float duration)
    {
        if (GetComponent<Entity_Health>().isDead) return;
        stunnedState.SetStunDuration(duration);
        stateMachine.ChangeState(stunnedState);
    }
    public void OpenCounterAttackWindow()
    {
        manualCounterWindow=true;
    }
    public void CloseCounterAttackWindow()
    {
        manualCounterWindow=false;
    }
    public override bool IsSafeToWalk()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, skeletonGroundCheckDistance, whatIsGround);
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - skeletonGroundCheckDistance));
        }
    }


}
