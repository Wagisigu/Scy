using UnityEngine;

public class PlayerJumpState : PlayerActionState
{
    #region Constructor
    public PlayerJumpState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion

    #region State Lifecycle
    public override void OnEnter()
    {
        base.OnEnter();
        _inputHandler.UseJumpInput();
        _playerController.SetVelocityY(_playerData.JumpForce);
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _playerData.WalkSpeed);
    }
    #endregion

    #region State Updates
    public override void Update()
    {
        base.Update();

        // State Transitions
        if (_playerController.IsGrounded && _playerController.CurrentVelocityY <= 0.01f)
        {
            _stateMachine.ChangeState(_playerController.IdleState);
        }
        else if (_playerController.IsWalled)
        {
            _stateMachine.ChangeState(_playerController.WallState);
        }
        else if (_inputHandler.AttackInput)
        {
            _stateMachine.ChangeState(_playerController.AttackState);
            _inputHandler.UseAttackInput();
        }
        else if (_inputHandler.DodgeInput)
        {
            _inputHandler.UseDodgeInput();
            _stateMachine.ChangeState(_playerController.DodgeState);
        }
        else if (_inputHandler.JumpInput)
        {
            if (_playerController.CanDoubleJump)
            {
                _playerController.CanDoubleJump = false; // Consume double jump
                _stateMachine.ChangeState(_playerController.DoubleJumpState);
                _inputHandler.UseJumpInput();
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _playerData.WalkSpeed);
    }

    public override void OnExit()
    {
        base.OnExit();
        _ignoreAnimationEndCallback = true; // Don't want to automatically transition to AirState when the jump animation ends
    }
    #endregion

    #region Jump Callbacks
    public void JumpFinished()
    {
        if (!_ignoreAnimationEndCallback)
        {
            _stateMachine.ChangeState(_playerController.AirState);
        }
    }
    #endregion
}
