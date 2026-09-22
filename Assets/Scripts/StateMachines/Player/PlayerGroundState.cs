using UnityEngine;

public class PlayerGroundState : PlayerState
{

    public PlayerGroundState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : 
        base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
        
    }

    public override void Update()
    {
        base.Update();

        if (_inputHandler.JumpInput)
        {
            _inputHandler.UseJumpInput();
            _stateMachine.ChangeState(_playerController.JumpState);
        }
        else if (!_playerController.IsGrounded)
        {
            _stateMachine.ChangeState(_playerController.AirState);
        }
        else if (_inputHandler.AttackInput)
        {
            _inputHandler.UseAttackInput();
            _stateMachine.ChangeState(_playerController.AttackState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }


}
