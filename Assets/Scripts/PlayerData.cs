using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
[Header("Horizontal Movement Settings")]
    [field: SerializeField] public float WalkSpeed {get; private set; } = 1;

[Header("Jump Movement Settings")]
    [field: SerializeField] public float JumpForce {get; private set; } = 1;
    [field: SerializeField] public Vector2 WallJumpForce {get; private set; } = Vector2.one;
    [field: SerializeField] public float WallSlideGravityScale {get; private set; } = 0.5f;
    [field: SerializeField] public float WallJumpLockTime {get; private set; } = 0.1f;
}
