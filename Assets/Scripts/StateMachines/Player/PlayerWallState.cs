using UnityEngine;

public class PlayerWallState : PlayerState
{
    public PlayerWallState(PlayerController playerController, PlayerInputHandler inputHandler, PlayerData playerData, PlayerStateMachine stateMachine, string animationName) : base(playerController, inputHandler, playerData, stateMachine, animationName)
    {
        
    }
}
