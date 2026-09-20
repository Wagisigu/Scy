using System;
using System.Collections;
using System.Data;
using Unity.InferenceEngine.Tokenization.PreTokenizers;
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
    private Rigidbody2D _rb;
    private Health _health;
    private Animator _animator;

    [Header("Ground Check Setup")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    public bool IsGrounded;
    public bool IsWalled;

    [SerializeField] private PlayerData _playerData;
    private PlayerInputHandler _inputHandler;

    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerState GroundState { get; private set; }
    public PlayerState WalkState { get; private set; }
    public PlayerState IdleState { get; private set; }
    public PlayerState ActionState { get; private set; }
    public PlayerState JumpState { get; private set; }
    public PlayerState AttackState { get; private set; }
    public PlayerState AirState { get; private set; }
    public PlayerState WallState { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        StateMachine = new PlayerStateMachine();
        GroundState = new PlayerGroundState(this, _inputHandler, _playerData, StateMachine, "");
        WalkState = new PlayerWalkState(this, _inputHandler, _playerData, StateMachine, "Walking");
        IdleState = new PlayerIdleState(this, _inputHandler, _playerData, StateMachine, "Idle");
        ActionState = new PlayerActionState(this, _inputHandler, _playerData, StateMachine, "");
        JumpState = new PlayerJumpState(this, _inputHandler, _playerData, StateMachine, "");
        AttackState = new PlayerAttackState(this, _inputHandler, _playerData, StateMachine, "Attacking");
        AirState = new PlayerAirState(this, _inputHandler, _playerData, StateMachine, "Airborne");
        WallState = new PlayerWallState(this, _inputHandler, _playerData, StateMachine, "WallSliding");

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
    }

    // Update is called once per frame
    public void Update()
    {
        _animator.SetFloat("VerticalSpeed", _rb.linearVelocityY);
        _animator.SetFloat("HorizontalSpeed", _rb.linearVelocityX);
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _checkRadius, _groundLayer);
        IsWalled = Physics2D.OverlapCircle(_wallCheckPoint.position, _checkRadius, _groundLayer);
        StateMachine.Update();
    }

    public void MoveX(float x)
    {
        _rb.linearVelocityX = x;
        Flip();
    }

    public void MoveY(float y)
    {
        _rb.linearVelocityY = y;
    }

    public void PlayAnimation(string animationName)
    {
        print("Playing animation: " + animationName);
        _animator.Play(animationName);
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
