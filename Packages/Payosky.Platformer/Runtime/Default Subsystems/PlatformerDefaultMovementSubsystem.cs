using System;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultMovementSubsystem : MovementSubsystem
    {
        private PlatformerPlayerController _platformerPlayerController;

        public override void Initalize(IPlayerController playerController)
        {
            base.Initalize(playerController);
            _platformerPlayerController = playerController as PlatformerPlayerController;
        }

        /// <summary>
        ///     Updates the movement mechanics of the platformer entity based on player input and movement constraints.
        /// </summary>
        public override void Update()
        {
            if (_platformerPlayerController is null) return;
            if (!_platformerPlayerController.PlatformerInputActions.Player.Move.inProgress) return;

            var movementAxis = _platformerPlayerController.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            if (Mathf.Abs(_platformerPlayerController.Rigidbody2D.linearVelocityX) < MaxSpeed * (_platformerPlayerController.MovementController.isGrounded ? 1 : _platformerPlayerController.MovementController.JumpSubsystem.MovementApexJumpModifier.Evaluate(_platformerPlayerController.Rigidbody2D.linearVelocityY)))
            {
                _platformerPlayerController.Rigidbody2D.AddForceX(movementAxis.x * MovementSpeed * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
    }
}