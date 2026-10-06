using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController Instance;
    #endregion

    #region Component References
    private Rigidbody2D _rb;
    private Health _health;
    private Animator _animator;
    private PlayerInputHandler _inputHandler;
    [SerializeField] private PlayerData _playerData;
    #endregion

    [SerializeField] private Transform _attackHitboxPoint; // Point from which the attack hitbox is checked
    [SerializeField] private LayerMask _enemyLayer;

    #region Contact Checks
    [Header("Ground Check Setup")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    public bool IsGrounded;
    public bool IsWalled;
    public int WallDirection { get; private set; } // 1 for right wall, -1 for left wall
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

    #region Movement Control
    public bool IsOrientationLocked = false; // Flag to lock orientation changes
    private float _initialGravity;
    private bool _isMovementControlLocked = false; // Flag to lock velocity changes
    #endregion

    #region Velocity Accessors
    public Vector2 CurrentVelocity => _rb.linearVelocity;
    public float CurrentVelocityY => _rb.linearVelocityY;
    public float CurrentVelocityX => _rb.linearVelocityX;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        if (_playerData == null)
        {
            Debug.LogError("PlayerData is not assigned! Please assign PlayerData in the Inspector.");
            return;
        }

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

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
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
    #endregion

    #region Movement & Velocity
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

    public void CutJump(float multiplier)
    {
        // Cuts upward velocity immediately if releasing early mid-ascent
        if (_rb.linearVelocityY > 0f)
        {
            _rb.linearVelocityY *= multiplier;
        }
    }
    #endregion

    #region Gravity Control
    public void DisableGravity()
    {
        _rb.gravityScale = 0;
    }

    public void EnableGravity()
    {
        _rb.gravityScale = _initialGravity;
    }
    #endregion

    #region Animation
    public void PlayAnimation(string animationName)
    {
        _animator.Play(animationName);
    }
    #endregion

    #region Movement Control
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
    #endregion

    #region Facing Direction
    private void Flip()
    {
        if (IsOrientationLocked) return;

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
    #endregion

    #region Event Handlers
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Debug.Log($"Player Health Updated: {currentHealth} / {maxHealth}");
    }

    private void HandleDeath()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);
    }

    private void EnableAttackHitbox()
    {
        // Detect collisions with enemies here using a capsule collider
        DrawDebugCapsule2D(
            _attackHitboxPoint.position,
            _attackHitboxPoint.localScale,
            CapsuleDirection2D.Horizontal,
            _attackHitboxPoint.rotation.eulerAngles.z,
            Color.red,
            0.1f);
        Collider2D[] enemyColliders = Physics2D.OverlapCapsuleAll(
            _attackHitboxPoint.position, 
            _attackHitboxPoint.localScale, 
            CapsuleDirection2D.Horizontal,
            _attackHitboxPoint.rotation.eulerAngles.z, 
            _enemyLayer);

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            Debug.Log($"Hit enemy: {enemyCollider.name}");
        }
    }
    #endregion

    public static void DrawDebugCapsule2D(Vector2 center, Vector2 size, CapsuleDirection2D direction, float angleDegree, Color color, float duration = 0.04f)
    {
        float angleRad = angleDegree * Mathf.Deg2Rad;
        float width = size.x;
        float height = size.y;

        // Calculate core dimensions based on orientation
        float radius = (direction == CapsuleDirection2D.Vertical) ? width / 2f : height / 2f;
        float halfHeight = height / 2f;
        float halfWidth = width / 2f;

        // Local positions for the straight side line segments
        Vector2 localTopLeft, localTopRight, localBotLeft, localBotRight;

        if (direction == CapsuleDirection2D.Vertical)
        {
            float offset = halfHeight - radius;
            localTopLeft = new Vector2(-radius, offset);
            localTopRight = new Vector2(radius, offset);
            localBotLeft = new Vector2(-radius, -offset);
            localBotRight = new Vector2(radius, -offset);
        }
        else // Horizontal capsule orientation
        {
            float offset = halfWidth - radius;
            localTopLeft = new Vector2(-offset, radius);
            localTopRight = new Vector2(offset, radius);
            localBotLeft = new Vector2(-offset, -radius);
            localBotRight = new Vector2(offset, -radius);
        }

        // Matrix helper function to transform local positions into rotated world positions
        Vector2 TransformPoint(Vector2 localPoint)
        {
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);
            float tx = localPoint.x * cos - localPoint.y * sin;
            float ty = localPoint.x * sin + localPoint.y * cos;
            return center + new Vector2(tx, ty);
        }

        // Convert core structural anchor bounds to world points
        Vector2 wTL = TransformPoint(localTopLeft);
        Vector2 wTR = TransformPoint(localTopRight);
        Vector2 wBL = TransformPoint(localBotLeft);
        Vector2 wBR = TransformPoint(localBotRight);

        // Draw the straight capsule edges
        Debug.DrawLine(wTL, wBL, color, duration);
        Debug.DrawLine(wTR, wBR, color, duration);

        // Draw the semi-circular end caps dynamically using points
        int segments = 8;
        Vector2 prevCapStart = (direction == CapsuleDirection2D.Vertical) ? wTR : wTR;

        // Draw Top/Right Cap
        float startAngle = (direction == CapsuleDirection2D.Vertical) ? 0f : -90f;
        Vector2 capCenter = (direction == CapsuleDirection2D.Vertical) ? TransformPoint(new Vector2(0, halfHeight - radius)) : TransformPoint(new Vector2(halfWidth - radius, 0));

        for (int i = 0; i <= segments; i++)
        {
            float a = angleDegree + startAngle + (i * (180f / segments));
            float rad = a * Mathf.Deg2Rad;
            Vector2 nextPoint = capCenter + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            if (i > 0) Debug.DrawLine(prevCapStart, nextPoint, color, duration);
            prevCapStart = nextPoint;
        }

        // Draw Bottom/Left Cap
        startAngle = (direction == CapsuleDirection2D.Vertical) ? 180f : 90f;
        capCenter = (direction == CapsuleDirection2D.Vertical) ? TransformPoint(new Vector2(0, -halfHeight + radius)) : TransformPoint(new Vector2(-halfWidth + radius, 0));
        prevCapStart = (direction == CapsuleDirection2D.Vertical) ? wBL : wBL;

        for (int i = 0; i <= segments; i++)
        {
            float a = angleDegree + startAngle + (i * (180f / segments));
            float rad = a * Mathf.Deg2Rad;
            Vector2 nextPoint = capCenter + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            if (i > 0) Debug.DrawLine(prevCapStart, nextPoint, color, duration);
            prevCapStart = nextPoint;
        }
    }

}
