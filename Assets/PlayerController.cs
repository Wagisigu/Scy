using System;
using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

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
                _rb.linearVelocityX = 0;
                StartCoroutine(Attack());
            }
            if (!_isAttacking) _rb.linearVelocity = new Vector2(_moveInput.x * _walkSpeed, _rb.linearVelocity.y);
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

    // TODO: Instead use an attack event in the animation itself and respond to that here
    IEnumerator Attack()
    {
        _animator.SetTrigger("Attack");
        // Wait until the animator enters an attack state, capture its state hash,
        // then wait until we leave that state. This ensures we wait specifically
        // for the attack state's exit instead of relying on normalizedTime of
        // whatever state is current (which could be Idle).
        int attackStateHash = 0;
        yield return new WaitUntil(() =>
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            // Common state name checks: exact name or in "Base Layer.Name" form
            if (state.IsName("Attack") || state.IsName("Base Layer.Attack"))
            {
                attackStateHash = state.fullPathHash;
                return true;
            }

            // As a fallback, check the current clip name contains "Punch" (useful for variant clips)
            var clips = _animator.GetCurrentAnimatorClipInfo(0);
            if (clips != null && clips.Length > 0 && clips[0].clip != null)
            {
                if (clips[0].clip.name.IndexOf("Attack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    attackStateHash = state.fullPathHash;
                    return true;
                }
            }

            return false;
        });

        // Wait while the animator is still in the captured attack state
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash);

        _isAttacking = false;
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
