using System.Collections;
using UnityEngine;

// Player now inherits from Entity instead of MonoBehaviour
public class Player : Entity
{
    [Header("Player-Specific Details")]
    public PlayerInputSet input { get; private set; }
    public Vector2 moveInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool attackInput { get; private set; }

    [Header("Player States")]
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerBasicAttackState basicAttackState { get; private set; }

    [Header("Movement Details")]
    public float movespeed;
    public float jumpForce = 5f;
    public float coyoteTime = 0.15f; 
    [Range(0,1)]
    public float inAirMoveSpeedMultiplier = 0.7f;
    
    [Header("Dash Details")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    [Header("Wall Interaction")]
    public float wallSlideSpeed = 1.5f;
    [Header("Timers")]
    public float wallJumpGracePeriod = 0.15f;
    public Vector2 wallJumpForce = new Vector2(7, 10);

    [Header("Attack Details")]
    public Vector2[] attackVelocities;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 0.7f;
    private Coroutine queuedAttackCo;
    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public float lastAttackTime { get; private set; }
    public float lastDashTime { get; private set; }
    public float lastGroundedTime { get; private set; }
    public override void Awake()
    {
        base.Awake(); 
        input = new PlayerInputSet();

        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "wallSlide");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
        deadState = new PlayerDeathState(this, stateMachine, "die");
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public override void Start()
    {
        stateMachine.Initialize(idleState);
    }

    public override void Update()
    {
        base.Update(); 
        jumpBufferCounter -= Time.deltaTime;
        if (input.Player.Jump.WasPerformedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (input.Player.Jump.WasPerformedThisFrame())
            jumpInput = true;

        if (input.Player.Dash.WasPerformedThisFrame())
            dashInput = true;

        if (input.Player.Attack.WasPerformedThisFrame())
            attackInput = true;
    }
    public bool HasBufferedJump()
    {
        return jumpBufferCounter > 0;
    }

    public void UseJumpBuffer()
    {
        jumpBufferCounter = 0;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        jumpInput = false;
        dashInput = false;
        attackInput = false;
    }
    public bool CanDash()
    {
        return Time.time >= lastDashTime + dashCooldown;
    }

    public void UseDash()
    {
        lastDashTime = Time.time;
    }

    public void SetLastAttackTime()
    {
        lastAttackTime = Time.time;
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }
       queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }
}