using System.Collections;
using UnityEngine;

public class Player : Entity
{

    [Header("Attack Details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 8f;
    public float jumpForce;

    [Header("Dash Info")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

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
    public PlayerCounterAttackState counterAttackState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMacine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMacine, "Idle");
        moveState = new PlayerMoveState(this, stateMacine, "Move");
        jumpState = new PlayerJumpState(this, stateMacine, "Jump");
        airState = new PlayerAirState(this, stateMacine, "Jump");
        dashState = new PlayerDashState(this, stateMacine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMacine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMacine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMacine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMacine, "CounterAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMacine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMacine.currentState.Update();

        CheckForDashInput();
    }

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
