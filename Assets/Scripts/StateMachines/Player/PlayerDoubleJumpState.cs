using UnityEngine;

public class PlayerDoubleJumpState : PlayerJumpState
{
    #region Constructor
    public PlayerDoubleJumpState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion

    #region Jump Callbacks
    public void DoubleJumpFinished()
    {
        if (!_ignoreAnimationEndCallback)
        {
            _stateMachine.ChangeState(_playerController.AirState);
        }
    }
    #endregion
}
