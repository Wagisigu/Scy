using UnityEngine;

public class PlayerStateMachine
{
    #region State
    private PlayerState _currentPlayerState;
    #endregion

    #region State Management
    public void Initialize(PlayerState startingState)
    {
        _currentPlayerState = startingState;
        _currentPlayerState.OnEnter();
    }

    public void ChangeState(PlayerState newState)
    {
        _currentPlayerState.OnExit();
        _currentPlayerState = newState;
        _currentPlayerState.OnEnter();
    }
    #endregion

    #region State Updates
    public void Update()
    {
        _currentPlayerState.Update();
    }

    public void FixedUpdate()
    {
        _currentPlayerState.FixedUpdate();
    }
    #endregion
}
