using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Movement Input
    public Vector2 MoveInput { get; private set; }
    #endregion

    #region Jump Input
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    [SerializeField] private float _jumpInputBufferTime = 0.015f;
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
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            _jumpInputStartedTime = Time.time;
        }
        if (context.canceled)
        {
            JumpInputStop = true;
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

    public void UseJumpInputStop() => JumpInputStop = false;

    public void UseAttackInput() => AttackInput = false;
    #endregion
}
