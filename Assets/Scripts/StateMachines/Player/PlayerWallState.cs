using System.Collections;
using UnityEngine;

public class PlayerWallState : PlayerState
{
    public PlayerWallState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
        
    }

    public override void Update()
    {
        base.Update();
        if (!_playerController.IsWalled)
        {
            _stateMachine.ChangeState(_playerController.AirState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _playerData.WalkSpeed);
        if (_inputHandler.JumpInput)
        {
            _playerController.SetVelocity(new Vector2(_playerData.WallJumpForce.x * -_playerController.WallDirection, _playerData.WallJumpForce.y), _playerData.WallJumpLockTime);
            _playerController.ManualFlip(-_playerController.WallDirection); // Flip the player to face the opposite direction after wall jump)
            _stateMachine.ChangeState(_playerController.AirState);
            _inputHandler.UseJumpInput();
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.DisableGravity();
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
}
