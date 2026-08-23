using System;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    private Rigidbody2D _rb;
    private Animator _animator;

    [Header("Horizontal Movement Settings")]
    [SerializeField] private float _walkSpeed = 1;
    private Vector2 _moveInput;

    [Header("Jump Movement Settings")]
    [SerializeField] private float _jumpForce = 1;

    [Header("Ground Check Setup")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

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
        Flip();
        Move();
        SendStatus();
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

    private void Move()
    {
        _rb.linearVelocity = new Vector2(_moveInput.x * _walkSpeed, _rb.linearVelocity.y);
    }

    private void SendStatus()
    {
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("HorizontalSpeed", Mathf.Abs(_rb.linearVelocity.x));
    }

    // Input handling
    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        if (_isGrounded) _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
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
