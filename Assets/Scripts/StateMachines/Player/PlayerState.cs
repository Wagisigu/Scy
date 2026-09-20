using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController _playerController;
    protected PlayerInputHandler _inputHandler;
    protected PlayerData _playerData;
    protected PlayerStateMachine _stateMachine;
    protected string _animationName;

    public PlayerState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName)
    {
        _playerController = playerController;
        _inputHandler = inputHandler;
        _playerData = playerData;
        _stateMachine = stateMachine;
        _animationName = animationName;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (_inputHandler.AttackInput)
        {
            _stateMachine.ChangeState(_playerController.AttackState);
            _inputHandler.UseAttackInput();
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnEnter()
    {
        if (!string.IsNullOrEmpty(_animationName))
        {
            _playerController.PlayAnimation(_animationName);
        }
    }

    public virtual void OnExit()
    {

    }

}
