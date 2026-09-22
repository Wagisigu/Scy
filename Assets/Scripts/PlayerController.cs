using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    #region Component References
    private Rigidbody2D _rb;
    private Health _health;
    private Animator _animator;
    private PlayerInputHandler _inputHandler;
    [SerializeField] private PlayerData _playerData;
    #endregion

    #region Contact Checks
    [Header("Ground Check Setup")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    public bool IsGrounded;
    public bool IsWalled;
    #endregion

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerState WalkState { get; private set; }
    public PlayerState IdleState { get; private set; }
    public PlayerState JumpState { get; private set; }
    public PlayerState AttackState { get; private set; }
    public PlayerState AirState { get; private set; }
    public PlayerState WallState { get; private set; }
    #endregion

    private float _initialGravity;
    private bool _isMovementControlLocked = false; // Flag to lock velocity changes
    public int WallDirection { get; private set; } // 1 for right wall, -1 for left wall

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        StateMachine = new PlayerStateMachine();
        WalkState = new PlayerWalkState(this, _inputHandler, _playerData, StateMachine, "Walking");
        IdleState = new PlayerIdleState(this, _inputHandler, _playerData, StateMachine, "Idle");
        JumpState = new PlayerJumpState(this, _inputHandler, _playerData, StateMachine, "");
        AttackState = new PlayerAttackState(this, _inputHandler, _playerData, StateMachine, "Attacking");
        AirState = new PlayerAirState(this, _inputHandler, _playerData, StateMachine, "Airborne");
        WallState = new PlayerWallState(this, _inputHandler, _playerData, StateMachine, "WallClinging");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        StateMachine.Initialize(IdleState);
        _initialGravity = _rb.gravityScale;
    }

    // Update is called once per frame
    public void Update()
    {
        _animator.SetFloat("VerticalSpeed", _rb.linearVelocityY);
        _animator.SetFloat("HorizontalSpeed", _rb.linearVelocityX);
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _checkRadius, _groundLayer);
        IsWalled = Physics2D.OverlapCircle(_wallCheckPoint.position, _checkRadius, _groundLayer);
        if (IsWalled)
        {
            WallDirection = _wallCheckPoint.position.x > transform.position.x ? 1 : -1;
        }
        else
        {
            WallDirection = 0;
        }
        StateMachine.Update();
    }

    public void SetVelocityX(float x, float lockControl = 0)
    {
        if (_isMovementControlLocked) return;
        if (lockControl != 0) LockMovementControlForSecs(lockControl);
        _rb.linearVelocityX = x;
        Flip();
    }

    public void SetVelocityY(float y, float lockControl = 0)
    {
        if (_isMovementControlLocked) return;
        if (lockControl != 0) LockMovementControlForSecs(lockControl);
        _rb.linearVelocityY = y;
    }

    public void SetVelocity(Vector2 velocity, float lockControl = 0)
    {
        if (_isMovementControlLocked) return;
        if (lockControl != 0) LockMovementControlForSecs(lockControl);
        _rb.linearVelocity = velocity;
        Flip();
    }

    public void DisableGravity()
    {
        _rb.gravityScale = 0;
    }

    public void EnableGravity()
    {
        _rb.gravityScale = _initialGravity;
    }

    public void PlayAnimation(string animationName)
    {
        _animator.Play(animationName);
    }

    public void LockMovementControlForSecs(float seconds)
    {
        _isMovementControlLocked = true;
        StartCoroutine(LockMovementControlCoroutine(seconds));
    }

    private IEnumerator LockMovementControlCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _isMovementControlLocked = false;
    }

    private void OnEnable()
    {
        // Subscribe to event listeners
        _health.OnHealthChanged.AddListener(HandleHealthChanged);
        _health.OnDeath.AddListener(HandleDeath);
    }

    private void OnDisable()
    {
        // Unsubscribe from event listeners
        _health.OnHealthChanged.RemoveListener(HandleHealthChanged);
        _health.OnDeath.RemoveListener(HandleDeath);
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Debug.Log($"Player Health Updated: {currentHealth} / {maxHealth}");
    }

    private void HandleDeath()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void Flip()
    {
        if (_inputHandler.MoveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_inputHandler.MoveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void ManualFlip(int direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _checkRadius);
        }
    }

    public void PrintMessage(string Message)
    {
        print(Message);
    }
}
