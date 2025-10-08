using System;
using Payosky.CoreMechanics.PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultJumpSubsystem : JumpSubsystem
    {
        public float CoyoteTime = 0.3f;

        public AnimationCurve HoldJumpCurve;

        public float JumpBufferTime = 0.3f;

        public override void Dispose()
        {
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (PlayerController.MovementController is not PlatformerMovementController { MovementData: PlatformerMovementData movementData }) return;
            if (context.performed)
            {
                movementData.IsJumpCharging = true;

                if (!movementData.IsGrounded)
                {
                    movementData.JumpBufferCounter = JumpBufferTime;
                }
            }
            else if (context.canceled)
            {
                if (!movementData.IsJumpCharging || !(movementData.JumpHoldCounter < MaxJumpHoldTime)) return;
                movementData.JumpBufferCounter = JumpBufferTime;
                movementData.IsJumpCharging = false;
            }
        }

        /// <summary>
        ///     Handles the mechanics necessary for initiating, maintaining, and regulating the jump behavior of a platformer
        ///     entity.
        ///     Updates the jump-related counters including jump buffer and coyote time, evaluates jump inputs and constraints,
        ///     and applies corresponding forces to facilitate a responsive jumping system.
        /// </summary>
        public override void Update()
        {
            if (PlayerController is not PlatformerPlayerController
                {
                    MovementController: PlatformerMovementController
                    {
                        MovementData: PlatformerMovementData movementData
                    }
                } platformerPlayerController)
            {
                return;
            }

            movementData.JumpBufferCounter = Mathf.Clamp(movementData.JumpBufferCounter - Time.deltaTime, 0, JumpBufferTime);
            movementData.CoyoteTimeCounter = Mathf.Clamp(movementData.CoyoteTimeCounter - Time.deltaTime, 0, CoyoteTime);

            switch (movementData.IsGrounded)
            {
                case true:
                    movementData.CoyoteTimeCounter = CoyoteTime;
                    break;
                case false when Mathf.Approximately(movementData.JumpHoldCounter, MaxJumpHoldTime):
                    movementData.JumpHoldCounter = 0;
                    break;
            }

            if (movementData.IsJumpCharging)
            {
                movementData.JumpHoldCounter += Time.deltaTime;
                movementData.JumpHoldCounter = Mathf.Clamp(movementData.JumpHoldCounter, 0, MaxJumpHoldTime);

                if (Mathf.Approximately(movementData.JumpHoldCounter, MaxJumpHoldTime))
                {
                    movementData.JumpBufferCounter = JumpBufferTime;
                    movementData.IsJumpCharging = false;
                }

                if (movementData.JustLanded && movementData.JumpHoldCounter > 0)
                {
                    movementData.JumpBufferCounter = JumpBufferTime;
                    movementData.IsJumpCharging = false;
                }
            }

            movementData.JustLanded = false;

            if (!movementData.IsGrounded && !(movementData.CoyoteTimeCounter > 0)) return;
            if (!(movementData.JumpBufferCounter > 0)) return;

            var jumpForce = JumpForce * HoldJumpCurve.Evaluate(movementData.JumpHoldCounter / MaxJumpHoldTime);
            jumpForce -= platformerPlayerController.Rigidbody2D.linearVelocityY;

            platformerPlayerController.Rigidbody2D.AddForceY(jumpForce, ForceMode2D.Impulse);
            movementData.CoyoteTimeCounter = 0;
            movementData.JumpBufferCounter = 0;
            movementData.JumpHoldCounter = 0;
        }
    }
}