using UnityEngine;

public class PlayerAirState : PlayerState
{
    #region Constructor
    public PlayerAirState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
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
        if (_playerController.IsGrounded)
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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.SetVelocityX(_inputHandler.MoveInput.x * _playerData.WalkSpeed);
    }
    #endregion
}
