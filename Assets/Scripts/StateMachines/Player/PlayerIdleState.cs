using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    #region Constructor
    public PlayerIdleState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion

    #region State Lifecycle
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    #endregion

    #region State Updates
    public override void Update()
    {
        base.Update();
        if (_inputHandler.MoveInput.x != 0)
        {
            _stateMachine.ChangeState(_playerController.WalkState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate(); 
        _playerController.SetVelocityX(0);
    }
    #endregion
}
