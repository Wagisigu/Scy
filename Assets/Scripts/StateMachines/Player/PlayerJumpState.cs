using UnityEngine;

public class PlayerJumpState : PlayerActionState
{
    public PlayerJumpState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetVelocityY(_playerData.JumpForce);
        _stateMachine.ChangeState(_playerController.AirState);
    }
}
