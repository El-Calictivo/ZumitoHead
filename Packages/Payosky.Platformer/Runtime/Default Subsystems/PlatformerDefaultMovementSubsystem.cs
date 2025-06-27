using System;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultMovementSubsystem : MovementSubsystem
    {
        private PlatformerPlayerController _playerController;

        private PlatformerAttributes PlatformerAttributes => _playerController.MovementController.PlatformerAttributes;

        public override void SetPlayerController(IPlayerController playerController)
        {
            _playerController = (PlatformerPlayerController)playerController;
        } // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        ///     Updates the movement mechanics of the platformer entity based on player input and movement constraints.
        /// </summary>
        public override void Update()
        {
            if (!_playerController.PlatformerInputActions.Player.Move.inProgress) return;
            var movementAxis = _playerController.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            if (Mathf.Abs(_playerController.Rigidbody2D.linearVelocityX) < PlatformerAttributes.maxSpeed * (_playerController.MovementController.isGrounded ? 1 : PlatformerAttributes.movementApexJumpModifier.Evaluate(_playerController.Rigidbody2D.linearVelocityY)))
            {
                _playerController.Rigidbody2D.AddForceX(movementAxis.x * PlatformerAttributes.movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
    }
}