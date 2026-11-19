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

    protected bool _ignoreAnimationEndCallback = false;
    protected float _groundHorizontalSpeed;
    protected float _airHorizontalSpeed;

    #region Constructor
    public PlayerState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName)
    {
        _playerController = playerController;
        _inputHandler = inputHandler;
        _playerData = playerData;
        _stateMachine = stateMachine;
        _animationName = animationName;
        _groundHorizontalSpeed = _playerData.WalkSpeed;
        _airHorizontalSpeed = _playerData.AirStrafeSpeed;
    }
    #endregion

    #region State Lifecycle
    public virtual void OnEnter()
    {
        _ignoreAnimationEndCallback = false;
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
        if (_inputHandler.SprintInput)
        {
            _groundHorizontalSpeed = _playerData.SprintSpeed;
            _airHorizontalSpeed = _playerData.AirSprintStrafeSpeed;
        }
        else
        {
            _groundHorizontalSpeed = _playerData.WalkSpeed;
            _airHorizontalSpeed = _playerData.AirStrafeSpeed;
        }
    }

    public virtual void FixedUpdate()
    {
    }
    #endregion
}
