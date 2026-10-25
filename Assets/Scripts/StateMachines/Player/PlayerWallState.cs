using UnityEngine;

public class PlayerWallState : PlayerState
{
    private bool _isPushingOffWall = false; // Track if the player is pushing off the wall
    private double _pushingOffWallStartTime = 0; // Track the time when the player started pushing off the wall
    private bool _hasPushedOffWall = false; // Track if the player has already pushed off the wall

    #region Constructor
    public PlayerWallState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion

    #region State Lifecycle
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.CanDoubleJump = true; // Reset double jump when entering wall state
        _playerController.DisableGravity();
        _playerController.UnlockMovementControl();
        _playerController.SetVelocityY(0); // Stop vertical movement when entering wall state
        // Clear any buffered jump input when hitting the wall so a stale jump
        // doesn't immediately trigger a wall-jump unintentionally.
        _inputHandler.UseJumpInput();
    }

    public override void OnExit()
    {
        base.OnExit();
        _playerController.EnableGravity();
    }
    #endregion

    #region State Updates
    public override void Update()
    {
        base.Update();
        if (!_playerController.IsWalled)
        {
            _stateMachine.ChangeState(_playerController.AirState);
        }
        bool isCurrentlyPushingOffWall = Mathf.Abs(_inputHandler.MoveInput.x) > 0.01f && _playerController.WallDirection * _inputHandler.MoveInput.x < 0;
        if (isCurrentlyPushingOffWall)
        {
            if (!_isPushingOffWall)
            {
                _isPushingOffWall = true;
                _pushingOffWallStartTime = Time.time;
            }
            else
            {
                // Check if the player has been pushing off the wall for longer than the allowed time
                if (Time.time - _pushingOffWallStartTime > _playerData.WallPushOffTime)
                {
                    _hasPushedOffWall = true;
                }
            }
        }
        else
        {
            _isPushingOffWall = false;
            _hasPushedOffWall = false; // Reset the push-off state when the player is not pushing off the wall
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_hasPushedOffWall) _playerController.SetVelocityX(_inputHandler.MoveInput.x * _playerData.WalkSpeed);
        if (_inputHandler.JumpInput)
        {
            _playerController.SetVelocity(new Vector2(_playerData.WallJumpForce.x * -_playerController.WallDirection, _playerData.WallJumpForce.y), _playerData.WallJumpLockTime);
            _playerController.ManualFlip(-_playerController.WallDirection); // Flip the player to face the opposite direction after wall jump
            _stateMachine.ChangeState(_playerController.AirState);
            _inputHandler.UseJumpInput();
        }
    }
    #endregion
}
