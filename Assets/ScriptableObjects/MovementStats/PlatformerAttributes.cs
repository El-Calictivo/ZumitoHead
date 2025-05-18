using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Platformer Attributes", menuName = "Scriptable Objects/Platformer Attributes")]
public class PlatformerAttributes : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed = 10;

    public float maxSpeed = 5;

    [Space(10)]
    [Header("Jump")]
    public float jumpForce = 12;

    public float maxJumpHoldTime = 0.3f;

    public float jumpBufferTime = 0.3f;

    public float coyoteTime = 0.3f;

    public AnimationCurve holdJumpCurve;

    public AnimationCurve movementApexJumpModifier;
}