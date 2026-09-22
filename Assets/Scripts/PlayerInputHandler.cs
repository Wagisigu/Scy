using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    [SerializeField] private float _jumpInputBufferTime = 0.01f;
    private float _jumpInputStartedTime;

    public bool AttackInput { get; private set; }
    [SerializeField] private float _attackInputBufferTime = 0.01f;
    private float _attackInputStartTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (JumpInput && Time.time - _jumpInputStartedTime >= _jumpInputBufferTime) JumpInput = false;
        if (AttackInput && Time.time - _attackInputStartTime >= _attackInputBufferTime) AttackInput = false;
    }

    // Input handling
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 rawMovementInput = context.ReadValue<Vector2>();
        MoveInput = rawMovementInput.normalized;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpInput = true;
            _jumpInputStartedTime = Time.time;
        }
    }

    public void UseJumpInput() => JumpInput = false;

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        AttackInput = true;
        _attackInputStartTime = Time.time;
    }

    public void UseAttackInput() => AttackInput = false;
}
