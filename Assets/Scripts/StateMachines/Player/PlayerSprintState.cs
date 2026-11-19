using UnityEngine;

public class PlayerSprintState : PlayerGroundState
{
    public PlayerSprintState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }

    #region State Updates
    public override void Update()
    {
        base.Update();
        if (_inputHandler.MoveInput.x == 0)
        {
            _stateMachine.ChangeState(_playerController.IdleState);
        }
        else if (!_inputHandler.SprintInput)
        {
            _stateMachine.ChangeState(_playerController.WalkState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _groundHorizontalSpeed);
    }
    #endregion
}
