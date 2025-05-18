using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "Platformer Attributes", menuName = "Scriptable Objects/Platformer Attributes")]
public class PlatformerAttributes : ScriptableObject
{
    public float movementSpeed = 10;
    public AnimationCurve jumpForce;
    public float maxSpeed = 5;
    public float onAirMovementMultiplier = 0.25f;
    private float CoyoteTime = 0.3f;
}