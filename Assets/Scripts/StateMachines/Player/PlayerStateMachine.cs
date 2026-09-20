using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState _currentPlayerState;

    public void Initialize(PlayerState startingState)
    {
        _currentPlayerState = startingState;
        _currentPlayerState.OnEnter();
    }

    public void Update()
    {
        _currentPlayerState.Update();
    }

    public void FixedUpdate()
    {
        _currentPlayerState.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        _currentPlayerState.OnExit();
        _currentPlayerState = newState;
        _currentPlayerState.OnEnter();
    }
}
