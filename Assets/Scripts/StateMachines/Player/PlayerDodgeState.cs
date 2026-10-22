using UnityEngine;

public class PlayerDodgeState : PlayerActionState
{
    public PlayerDodgeState(PlayerController playerController,
                            PlayerInputHandler inputHandler,
                            PlayerData playerData,
                            PlayerStateMachine stateMachine,
                            string animationName)
        : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetInvulnerability(true);
        int dodgeDirection = _inputHandler.MoveInput.x != 0 ? (int)Mathf.Sign(_inputHandler.MoveInput.x) : _playerController.FacingDirection;
        _playerController.SetVelocityX(_playerData.DodgeSpeed * dodgeDirection, -1);
    }

    public override void Update()
    {
        base.Update();
        if (_playerController.IsGrounded && _playerController.CurrentVelocity.x == 0)
        {
            _stateMachine.ChangeState(_playerController.IdleState);
        }
    }

    public void DodgeFinished()
    {
        _playerController.UnlockMovementControl();
        _stateMachine.ChangeState(_playerController.IdleState);
    }

    public override void OnExit()
    {
        base.OnExit();
        _playerController.SetInvulnerability(false);
    }
}
