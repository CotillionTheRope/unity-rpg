using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 8;
    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    #endregion
    #region States
    public PlayerStateMachine stateMacine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMacine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMacine, "Idle");
        moveState = new PlayerMoveState(this, stateMacine, "Move");
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
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2 (xVelocity, yVelocity);
    }
}
