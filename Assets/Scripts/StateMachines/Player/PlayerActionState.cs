using UnityEngine;

/// <summary>
/// Base class for action states (Jump, Attack, etc.)
/// Provides common functionality for states that represent player actions.
/// </summary>
public class PlayerActionState : PlayerState
{
    #region Constructor
    public PlayerActionState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
    }
    #endregion
}
