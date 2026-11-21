using UnityEngine;

public class FlyingEnemyIdleState : FlyingEnemyState
{
    public FlyingEnemyIdleState(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine, string animationName) : base(inputHandler, stateMachine, animationName)
    {
    }
}
