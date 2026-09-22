using UnityEngine;

public class PlayerAttackState : PlayerActionState
{
    #region Constructor
    public PlayerAttackState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
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
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Attack Callbacks
    public void AttackFinished()
    {
        _stateMachine.ChangeState(_playerController.IdleState);
    }
    #endregion
}
