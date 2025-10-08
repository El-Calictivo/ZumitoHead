using Payosky.Architecture.Editor.Attributes;
using Payosky.CoreMechanics.PlayerController;
using UnityEngine;

namespace Payosky.Platformer
{
    public sealed class PlatformerMovementData : IPlayerMovementData
    {
        [ReadOnly]
        public float CoyoteTimeCounter;

        [ReadOnly]
        public bool IsGrounded;

        [ReadOnly]
        public bool IsJumpCharging;

        [ReadOnly]
        public bool JustLanded;

        [ReadOnly]
        public float JumpBufferCounter;

        [ReadOnly]
        public float JumpHoldCounter;

        [ReadOnly]
        public Vector2 LastGroundedPosition;
    }
}