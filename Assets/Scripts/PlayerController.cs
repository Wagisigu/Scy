using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    public Rigidbody2D _rb { get; private set; }
    private Health _health;
    public Animator _animator { get; private set; }

    [Header("Horizontal Movement Settings")]
    [SerializeField] public float _walkSpeed = 1;
    public Vector2 _moveInput { get; private set; }

    [Header("Jump Movement Settings")]
    [SerializeField] private float _jumpForce = 1;
    [SerializeField] private Vector2 _wallJumpForce = Vector2.one;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    [SerializeField] private float _wallSlideGravityScale = 0.5f;
    private bool _jumpInput;

    [Header("Ground Check Setup")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private float _normalGravityScale = 1.0f;
    private bool _attackInput;
    private bool _isAttacking;
    private bool _isWallSliding;
    private int _directionOfWall; // 1 for right, -1 for left, 0 for no wall
    private bool _canInput = true;

    private bool _isGrounded;
    private bool _isWalled;

    private PlayerStateMachineManager _stateMachineManager { get; set; }
    private PlayerState _playerGroundState { get; set; }
    private PlayerState _playerAirState { get; set; }
    private PlayerState _playerWallState { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _normalGravityScale = _rb.gravityScale;

        _stateMachineManager = new PlayerStateMachineManager();
        _playerGroundState = new PlayerGroundState(this);
        _playerAirState = new PlayerAirState(this);
        _playerWallState = new PlayerWallState(this);

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
    void Start()
    {
        _stateMachineManager.Initialize(_playerGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        Flip();
        _stateMachineManager.Update();
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

    public void FinishAttack()
    {
        _isAttacking = false;
        _rb.gravityScale = _normalGravityScale; // Restore gravity scale after attack
    }

    private void FixedUpdate()
    {
        _stateMachineManager.FixedUpdate();
    }

    private void UpdateStatus()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, checkRadius, groundLayer);
        bool isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, checkRadius, groundLayer);
        if (isWalled && !_isWalled)
        {
            _directionOfWall = (int) transform.localScale.x; // Determine the direction of the wall based on character facing
        }
        _isWalled = isWalled;
    }

    private void Flip()
    {
        if (_moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private bool CanFlip()
    {
        return !_isAttacking && !_isWallSliding;
    }

    private void UpdateCharacter()
    {
        // Flip the character based on movement input
        if (CanFlip()) Flip();

        // Move the character horizontally based on input
        if (_canInput) _rb.linearVelocityX = _moveInput.x * _walkSpeed;

        // Handle queued jump input
        if (_jumpInput && _isGrounded)
        {
            _rb.linearVelocityY = _jumpForce;
        }

        // Handle queued attack input
        if (_attackInput && !_isAttacking)
        {
            _isAttacking = true;
            _rb.gravityScale = 0; // Disable gravity during attack
            _animator.SetTrigger("Attack");
        }

        // Handle wall sliding
        if (_isWalled && !_isGrounded)
        {
            // Handle first frame of wall sliding
            if (!_isWallSliding)
            {
                _isWallSliding = true;
                _rb.gravityScale = _wallSlideGravityScale;
                _rb.linearVelocityY = 0;
            }

            // Handle jump input while wall sliding
            if (_jumpInput)
            {
                _rb.linearVelocity = new Vector2(_wallJumpForce.x * -_directionOfWall, _wallJumpForce.y);
                StartCoroutine(SuspendControl(_jumpBufferTime)); // Suspend control for a short duration after wall jump
            }
        }
        else if (_isWallSliding)
        {
            _isWallSliding = false;
            _rb.gravityScale = _normalGravityScale; // Restore gravity scale after wall slide
        }

        _attackInput = false;
        _jumpInput = false;
    }

    private void SendStatus()
    {
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_rb.linearVelocityX));
        _animator.SetFloat("VerticalSpeed", _rb.linearVelocityY);
        _animator.SetBool("IsWalled", _isWalled);
    }

    private IEnumerator SuspendControl(float duration)
    {
        _canInput = false;
        _jumpInput = false;
        _attackInput = false;
        _moveInput = Vector2.zero;
        yield return new WaitForSeconds(duration);
        _canInput = true;
    }

    // Input handling
    private void OnMove(InputValue inputValue)
    {
        if (_canInput) _moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        if (_canInput) _jumpInput = true;
    }

    private void OnAttack()
    {
        if (_canInput) _attackInput = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
        }
    }
}
