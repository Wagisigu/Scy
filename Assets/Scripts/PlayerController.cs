using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour, IHealth
{

    public static PlayerController Instance;
    private Rigidbody2D _rb;
    private Animator _animator;

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private int _currentHealth = 4;
    [SerializeField] private float _invulDuration = 1.0f;
    private float _lastDamageTime = -Mathf.Infinity;
    public bool IsInvulnerable => Time.time < _lastDamageTime + _invulDuration;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float _walkSpeed = 1;
    private Vector2 _moveInput;

    [Header("Jump Movement Settings")]
    [SerializeField] private float _jumpForce = 1;
    private bool _jumpInput;

    [Header("Ground Check Setup")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool _attackInput;
    private bool _isAttacking;

    private bool _isGrounded;

    private void Awake()
    {
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
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatus();
        SendStatus();
    }

    public void TakeDamage(int damage)
    {
        if (IsInvulnerable) return;

        _lastDamageTime = Time.time;

        // Set health to 0 if total damage causes health to go negative
        _currentHealth = Math.Max(0, _currentHealth - damage);
        Debug.Log($"Player took {damage} damage! Remaining: {_currentHealth}");

        // Initiate death sequence if health goes to 0.
        if (_currentHealth == 0) Die();
    }

    public bool Heal(int hp)
    {
        // If health is at max, don't pick up the hp
        if (_currentHealth == _maxHealth) return false;

        // Set health to max if total goes above max
        _currentHealth = Math.Min(_maxHealth, _currentHealth + hp);
        Debug.Log($"Player was healed for {hp} health! Current health: {_currentHealth}");
        return true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void FinishAttack()
    {
        _isAttacking = false;
        _rb.gravityScale = 8; // Restore gravity scale after attack
    }

    private void FixedUpdate()
    {
        UpdateCharacter();
    }

    private void UpdateStatus()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, checkRadius, groundLayer);
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

    private void UpdateCharacter()
    {
        Flip();
        if (_isGrounded)
        {
            if (!_isAttacking && _jumpInput) _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            else if (_attackInput)
            {
                _isAttacking = true;
                _animator.SetTrigger("Attack");
            }
            if (!_isAttacking) _rb.linearVelocity = new Vector2(_moveInput.x * _walkSpeed, _rb.linearVelocity.y);
        }
        else
        {
            if (_attackInput)
            {
                _isAttacking = true;
                _animator.SetTrigger("Attack");

                // Zero out current velocity so they don't slide or keep moving up/down
                _rb.linearVelocity = Vector2.zero;
                // Set gravity scale to 0 to prevent falling during the attack
                _rb.gravityScale = 0;
            }
        }

        _attackInput = false;
        _jumpInput = false;
    }

    private void SendStatus()
    {
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_rb.linearVelocityX));
        _animator.SetFloat("VerticalSpeed", _rb.linearVelocityY);
    }

    // Input handling
    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        _jumpInput = true; 
    }

    private void OnAttack()
    {
        _attackInput = true;
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
