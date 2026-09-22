using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Movement Input
    public Vector2 MoveInput { get; private set; }
    #endregion

    #region Jump Input
    public bool JumpInput { get; private set; }
    [SerializeField] private float _jumpInputBufferTime = 0.01f;
    private float _jumpInputStartedTime;
    #endregion

    #region Attack Input
    public bool AttackInput { get; private set; }
    [SerializeField] private float _attackInputBufferTime = 0.01f;
    private float _attackInputStartTime;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
    }

    private void Update()
    {
        if (JumpInput && Time.time - _jumpInputStartedTime >= _jumpInputBufferTime) JumpInput = false;
        if (AttackInput && Time.time - _attackInputStartTime >= _attackInputBufferTime) AttackInput = false;
    }
    #endregion

    #region Input Callbacks
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

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        AttackInput = true;
        _attackInputStartTime = Time.time;
    }
    #endregion

    #region Input Consumption
    public void UseJumpInput() => JumpInput = false;

    public void UseAttackInput() => AttackInput = false;
    #endregion
}
