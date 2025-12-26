using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Core Components")]
    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }
    public StateMachine stateMachine { get; private set; }
    public Entity_Stats stats { get; private set; }
    
    public EntityDeadState deadState;
    public Collider2D cd { get; private set; } // 1. Added cached Collider

    [Header("Movement")]
    public int facingDirection { get; protected set; } = 1;
    protected bool isFacingRight = true;
    public float defaultGravity { get; protected set; }

    [Header("Setup")]
    [SerializeField] protected bool defaultFacingRight = true;

    [Header("Collision Checks")]
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected float wallCheckDistance;

    public bool isGrounded { get; private set; }
    public bool isTouchingWall { get; private set; }

    protected Bounds colliderBounds;

    private bool isKnocked;
    private Coroutine knockbackCoroutine;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        cd = GetComponent<Collider2D>(); // 2. Cache collider here
        stats = GetComponent<Entity_Stats>();
        stateMachine = new StateMachine();

        defaultGravity = rb.gravityScale;
    }

    public virtual void Start()
    {
        if (!defaultFacingRight) Flip();
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
        colliderBounds = cd.bounds; 
        HandleGroundDetection();
        HandleWallDetection();

        stateMachine.UpdateActiveState();
    }

    public virtual void SetVelocity(float x, float y)
    {
        if (isKnocked) return;
        rb.linearVelocity = new Vector2(x, y);
        HandleFlip(x);
    }
    public void RecieveKnockback(Vector2 knockback, float duration)
    {
        if(knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        knockbackCoroutine = StartCoroutine(KnockbackCo(knockback,duration));
    }
    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    public virtual void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public virtual void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
    }

    public virtual void HandleGroundDetection()
    {
        // No changes needed to logic, just the timing
        float colliderWidth = colliderBounds.size.x;
        Vector2 centerPoint = colliderBounds.center;
        Vector2 leftPoint = new Vector2(centerPoint.x - colliderWidth / 2, centerPoint.y);
        Vector2 rightPoint = new Vector2(centerPoint.x + colliderWidth / 2, centerPoint.y);

        RaycastHit2D leftHit = Physics2D.Raycast(leftPoint, Vector2.down, groundCheckDistance, whatIsGround);
        RaycastHit2D centerHit = Physics2D.Raycast(centerPoint, Vector2.down, groundCheckDistance, whatIsGround);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPoint, Vector2.down, groundCheckDistance, whatIsGround);

        int groundHitCount = (leftHit ? 1 : 0) + (centerHit ? 1 : 0) + (rightHit ? 1 : 0);
        
        isGrounded = groundHitCount >= 2;
    }

    public virtual void HandleWallDetection()
    {
        isTouchingWall = Physics2D.Raycast(transform.position, Vector3.right * facingDirection, wallCheckDistance, whatIsGround);
    }
    public virtual void DamageImpact(Transform damageSource)
    {
        //Debug.Log("Hit by" + damageSource.name);
    }
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }
    public virtual void Die()
    {
        stateMachine.ChangeState(deadState);
    }
    public virtual void OnDrawGizmos()
    {
        // (Gizmos code remains the same)
        if (GetComponent<Collider2D>() != null)
        {
            Bounds bounds = GetComponent<Collider2D>().bounds;
            float colliderWidth = bounds.size.x;
            Vector2 centerPoint = bounds.center;
            Vector2 leftPoint = new Vector2(centerPoint.x - colliderWidth / 2, centerPoint.y);
            Vector2 rightPoint = new Vector2(centerPoint.x + colliderWidth / 2, centerPoint.y);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftPoint, new Vector2(leftPoint.x, leftPoint.y - groundCheckDistance));
            Gizmos.DrawLine(centerPoint, new Vector2(centerPoint.x, centerPoint.y - groundCheckDistance));
            Gizmos.DrawLine(rightPoint, new Vector2(rightPoint.x, rightPoint.y - groundCheckDistance));
        }
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * facingDirection, 0f));
    }
}