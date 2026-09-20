using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
        
    }

    public override void Update()
    {
        base.Update();
        if (_playerController.IsGrounded)
        {
            _stateMachine.ChangeState(_playerController.IdleState);
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
