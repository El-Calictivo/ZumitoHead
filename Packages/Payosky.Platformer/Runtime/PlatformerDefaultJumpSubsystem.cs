using System;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultJumpSubsystem : JumpSubsystem
    {
        private PlatformerPlayerController _playerController;

        private PlatformerAttributes PlatformerAttributes => _playerController.MovementController.PlatformerAttributes;
        private PlatformerMovementController MovementController => _playerController.MovementController;

        public override void SetPlayerController(IPlayerController playerController)
        {
            _playerController = (PlatformerPlayerController)playerController;
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MovementController.isJumpCharging = true;

                if (!MovementController.isGrounded)
                {
                    MovementController.jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
                }
            }
            else if (context.canceled)
            {
                if (!MovementController.isJumpCharging || !(MovementController.jumpHoldCounter < PlatformerAttributes.maxJumpHoldTime)) return;
                MovementController.jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
                MovementController.isJumpCharging = false;
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
            MovementController.jumpBufferCounter = Mathf.Clamp(MovementController.jumpBufferCounter - Time.deltaTime, 0, PlatformerAttributes.jumpBufferTime);
            MovementController.coyoteTimeCounter = Mathf.Clamp(MovementController.coyoteTimeCounter - Time.deltaTime, 0, PlatformerAttributes.coyoteTime);

            if (!MovementController.isGrounded && Mathf.Approximately(MovementController.jumpHoldCounter, PlatformerAttributes.maxJumpHoldTime))
            {
                MovementController.jumpHoldCounter = 0;
            }


            if (MovementController.isJumpCharging)
            {
                MovementController.jumpHoldCounter += Time.deltaTime;
                MovementController.jumpHoldCounter = Mathf.Clamp(MovementController.jumpHoldCounter, 0, PlatformerAttributes.maxJumpHoldTime);

                if (Mathf.Approximately(MovementController.jumpHoldCounter, PlatformerAttributes.maxJumpHoldTime))
                {
                    MovementController.jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
                    MovementController.isJumpCharging = false;
                }

                if (MovementController.justLanded && MovementController.jumpHoldCounter > 0)
                {
                    MovementController.jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
                    MovementController.isJumpCharging = false;
                }
            }


            MovementController.justLanded = false;

            if (!MovementController.isGrounded && !(MovementController.coyoteTimeCounter > 0)) return;
            if (!(MovementController.jumpBufferCounter > 0)) return;

            var jumpForce = PlatformerAttributes.jumpForce * PlatformerAttributes.holdJumpCurve.Evaluate(MovementController.jumpHoldCounter / PlatformerAttributes.maxJumpHoldTime);
            jumpForce -= _playerController.Rigidbody2D.linearVelocityY;

            _playerController.Rigidbody2D.AddForceY(jumpForce, ForceMode2D.Impulse);
            MovementController.coyoteTimeCounter = 0;
            MovementController.jumpBufferCounter = 0;
            MovementController.jumpHoldCounter = 0;
        }
    }
}