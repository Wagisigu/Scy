using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    #region Constructor
    public PlayerWalkState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion

    #region State Updates
    public override void Update()
    {
        base.Update();
        if (_inputHandler.MoveInput.x == 0)
        {
            _stateMachine.ChangeState(_playerController.IdleState);
        }
        else if (_inputHandler.SprintInput)
        {
            _stateMachine.ChangeState(_playerController.SprintState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _groundHorizontalSpeed);
    }
    #endregion
}
