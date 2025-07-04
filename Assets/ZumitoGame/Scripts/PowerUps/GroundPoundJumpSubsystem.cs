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

        [SerializeField] private float _groundPoundForce = 100f;

        public override void Initalize(IPlayerController playerController)
        {
            base.Initalize(playerController);
            _platformerPlayerController = playerController as PlatformerPlayerController;
        }

        public override void Dispose()
        {
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_movementController.isGrounded)
                {
                    _movementController.jumpHoldCounter = 0;
                    _platformerPlayerController.Rigidbody2D.linearVelocityY = 0;
                    _movementController.isJumpCharging = true;
                }
                else
                {
                    _platformerPlayerController.Rigidbody2D.AddForceY(-Mathf.Abs(_groundPoundForce) * (_movementController.jumpHoldCounter / MaxJumpHoldTime));
                }
            }
            else if (context.canceled)
            {
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

            if (_movementController.isJumpCharging)
            {
                _movementController.jumpHoldCounter += Time.fixedDeltaTime;
                _movementController.jumpHoldCounter = Mathf.Clamp(_movementController.jumpHoldCounter, 0, MaxJumpHoldTime);

                _platformerPlayerController.Rigidbody2D.AddForceY(JumpForce * (1 - _movementController.jumpHoldCounter / MaxJumpHoldTime));

                if (Mathf.Approximately(_movementController.jumpHoldCounter, MaxJumpHoldTime))
                {
                    _movementController.isJumpCharging = false;
                }
            }

            if (_movementController.justLanded)
            {
                HandleOnLanded();
            }
        }

        public void HandleOnLanded()
        {
            _movementController.isJumpCharging = false;
            _platformerPlayerController.MovementController.jumpHoldCounter = 0;
        }
    }
}