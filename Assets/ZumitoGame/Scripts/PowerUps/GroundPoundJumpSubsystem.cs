using System;
using Payosky.Platformer;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZumitoGame.PowerUps
{
    [Serializable]
    public class GroundPoundJumpSubsystem : JumpSubsystem
    {
        private PlatformerPlayerController _platformerPlayerController;

        private PlatformerMovementController _movementController => _platformerPlayerController.MovementController;

        public override void Initalize(IPlayerController playerController)
        {
            base.Initalize(playerController);
            _platformerPlayerController = playerController as PlatformerPlayerController;
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _movementController.isJumpCharging = true;

                if (!_movementController.isGrounded)
                {
                    _movementController.jumpBufferCounter = JumpBufferTime;
                }
            }
            else if (context.canceled)
            {
                if (!_movementController.isJumpCharging || !(_movementController.jumpHoldCounter < MaxJumpHoldTime)) return;
                _movementController.jumpBufferCounter = JumpBufferTime;
                _movementController.isJumpCharging = false;
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
            if (_platformerPlayerController is null) return;

            _movementController.jumpBufferCounter = Mathf.Clamp(_movementController.jumpBufferCounter - Time.deltaTime, 0, JumpBufferTime);
            _movementController.coyoteTimeCounter = Mathf.Clamp(_movementController.coyoteTimeCounter - Time.deltaTime, 0, CoyoteTime);

            if (!_movementController.isGrounded && Mathf.Approximately(_movementController.jumpHoldCounter, MaxJumpHoldTime))
            {
                _movementController.jumpHoldCounter = 0;
            }


            if (_movementController.isJumpCharging)
            {
                _movementController.jumpHoldCounter += Time.deltaTime;
                _movementController.jumpHoldCounter = Mathf.Clamp(_movementController.jumpHoldCounter, 0, MaxJumpHoldTime);

                if (Mathf.Approximately(_movementController.jumpHoldCounter, MaxJumpHoldTime))
                {
                    _movementController.jumpBufferCounter = JumpBufferTime;
                    _movementController.isJumpCharging = false;
                }

                if (_movementController.justLanded && _movementController.jumpHoldCounter > 0)
                {
                    _movementController.jumpBufferCounter = JumpBufferTime;
                    _movementController.isJumpCharging = false;
                }
            }


            _movementController.justLanded = false;

            if (!_movementController.isGrounded && !(_movementController.coyoteTimeCounter > 0)) return;
            if (!(_movementController.jumpBufferCounter > 0)) return;

            var jumpForce = JumpForce * HoldJumpCurve.Evaluate(_movementController.jumpHoldCounter / MaxJumpHoldTime);
            jumpForce -= _platformerPlayerController.Rigidbody2D.linearVelocityY;

            _platformerPlayerController.Rigidbody2D.AddForceY(jumpForce, ForceMode2D.Impulse);
            _movementController.coyoteTimeCounter = 0;
            _movementController.jumpBufferCounter = 0;
            _movementController.jumpHoldCounter = 0;
        }
    }
}