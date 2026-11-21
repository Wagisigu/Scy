using UnityEngine;

public abstract class FlyingEnemyState
{
    #region Protected References
    protected PlayerInputHandler _inputHandler;
    protected PlayerStateMachine _stateMachine;
    protected string _animationName;
    #endregion

    public FlyingEnemyState(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine, string animationName)
    {
        _inputHandler = inputHandler;
        _stateMachine = stateMachine;
        _animationName = animationName;
    }

    #region State Lifecycle
    public virtual void OnEnter()
    {
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
