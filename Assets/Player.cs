using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Attack Details")]
    public Vector2[] attackMovement;

    public bool isBusy {  get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 8f;
    public float jumpForce;

    [Header("Dash Info")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    [Header("Collision Info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    #endregion
    #region States
    public PlayerStateMachine stateMacine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }

    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMacine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMacine, "Idle");
        moveState = new PlayerMoveState(this, stateMacine, "Move");
        jumpState = new PlayerJumpState(this, stateMacine, "Jump");
        airState  = new PlayerAirState(this, stateMacine, "Jump");
        dashState = new PlayerDashState(this, stateMacine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMacine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMacine, "Jump");
        
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMacine, "Attack");
    }

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMacine.Initialize(idleState);
    }

    private void Update()
    {
        stateMacine.currentState.Update();

        CheckForDashInput();
    }

    #region Velocity
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2 (xVelocity, yVelocity);
        FlipController(xVelocity);
    }

    public void ZeroVelocity() => rb.linearVelocity = new Vector2(0, 0);
    #endregion

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion

    #region Flip
    public void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion

    private void CheckForDashInput()
    {
        dashUsageTimer -= Time.deltaTime;

        if (IsWallDetected())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMacine.ChangeState(dashState);
        }
    }

    public void AnimationTrigger() => stateMacine.currentState.AnimationFinishTrigger();

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false; 
    }
         
}
