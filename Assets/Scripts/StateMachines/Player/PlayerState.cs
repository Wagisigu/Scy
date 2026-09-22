using UnityEngine;

public abstract class PlayerState
{
    #region Protected References
    protected PlayerController _playerController;
    protected PlayerInputHandler _inputHandler;
    protected PlayerData _playerData;
    protected PlayerStateMachine _stateMachine;
    protected string _animationName;
    #endregion

    #region Constructor
    public PlayerState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName)
    {
        _playerController = playerController;
        _inputHandler = inputHandler;
        _playerData = playerData;
        _stateMachine = stateMachine;
        _animationName = animationName;
    }
    #endregion

    #region State Lifecycle
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
    #endregion

    #region State Updates
    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }
    #endregion
}
