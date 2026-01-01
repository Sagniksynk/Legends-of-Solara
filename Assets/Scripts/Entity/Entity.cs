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
    public Collider2D cd { get; private set; }

    [Header("Visuals & FX")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject popUpTextPrefab;
    [Header("Status Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color igniteColor = new Color(1f, 0.4f, 0.4f);
    [SerializeField] private Color chillColor = new Color(0.4f, 0.4f, 1f);

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

    [Header("Status Effects")]
    public bool isIgnited;
    public bool isChilled;

    private float igniteTimer;
    private float igniteDamageCooldown = 0.5f;
    private float igniteDamage;

    private float chillTimer;

    public bool isGrounded { get; private set; }
    public bool isTouchingWall { get; private set; }

    protected Bounds colliderBounds;

    private bool isKnocked;
    private Coroutine knockbackCoroutine;

    public virtual void Awake()
    {
        stateMachine = new StateMachine();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        cd = GetComponent<Collider2D>();
        stats = GetComponent<Entity_Stats>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        defaultGravity = rb.gravityScale;
    }

    public virtual void Start()
    {
        if (!defaultFacingRight) Flip();
    }

    public virtual void Update()
    {
        stateMachine.UpdateActiveState();

        if (isIgnited) ApplyIgniteLogic();
        if (isChilled) ApplyChillLogic();

        UpdateStatusColor();
    }

    private void UpdateStatusColor()
    {
        if (sr == null) return;

        if (isIgnited)
        {
            sr.color = igniteColor;
        }
        else if (isChilled)
        {
            sr.color = chillColor;
        }
        else
        {
            sr.color = defaultColor;
        }
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
        // CRITICAL FIX: When chilled, completely block movement
        if (isKnocked || isChilled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(x, y);
        HandleFlip(x);
    }

    public void RecieveKnockback(Vector2 knockback, float duration)
    {
        // When chilled, apply NO knockback force at all
        if (isChilled)
        {
            // Just stop movement completely - no knockback routine needed
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Normal knockback for non-ice attacks
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        knockbackCoroutine = StartCoroutine(KnockbackCo(knockback, duration));
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
        if (isChilled) return;
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

    public void Ignite(float seconds, float damagePerTick)
    {
        igniteTimer = seconds;
        igniteDamage = damagePerTick;
        isIgnited = true;
    }

    private void ApplyIgniteLogic()
    {
        igniteTimer -= Time.deltaTime;
        igniteDamageCooldown -= Time.deltaTime;

        if (igniteDamageCooldown < 0)
        {
            igniteDamageCooldown = 0.5f;

            if (stats != null)
            {
                stats.DecreaseHealth(igniteDamage);

                if (popUpTextPrefab != null)
                {
                    float randomX = Random.Range(-0.5f, 0.5f);
                    float randomY = Random.Range(0.5f, 1f);
                    Vector3 offset = new Vector3(randomX, randomY, 0);

                    GameObject newText = Instantiate(popUpTextPrefab, transform.position + offset, Quaternion.identity);
                    newText.GetComponent<FloatingText>().Setup(igniteDamage.ToString(), false);
                }
            }
        }

        if (igniteTimer < 0)
        {
            isIgnited = false;
        }
    }

    public void Chill(float seconds, float slowPercentage)
    {
        chillTimer = seconds;

        if (!isChilled)
        {
            ApplySlow(slowPercentage);
            isChilled = true;

            // CRITICAL: Stop all movement immediately when frozen
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ApplyChillLogic()
    {
        chillTimer -= Time.deltaTime;
        if (chillTimer < 0)
        {
            isChilled = false;
            RestoreSpeed();
        }
    }

    protected virtual void ApplySlow(float slowPercentage)
    {
        // Default implementation (empty)
    }

    protected virtual void RestoreSpeed()
    {
        // Default implementation (empty)
    }

    public virtual void OnDrawGizmos()
    {
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