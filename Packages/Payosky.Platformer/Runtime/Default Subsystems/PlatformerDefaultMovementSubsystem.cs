using System;
using Payosky.CoreMechanics.PlayerController;
using UnityEngine;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultMovementSubsystem : MovementSubsystem
    {
        public AnimationCurve MovementApexJumpModifier;

        public override void Dispose()
        {
        }

        public override void MoveHorizontally(Vector2 movement, MovementMode mode = MovementMode.Walk)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                switch (mode)
                {
                    case MovementMode.Walk:
                        platformerPlayerController.Rigidbody2D.AddRelativeForceX(movement.x * MovementSpeed);
                        platformerPlayerController.Rigidbody2D.linearVelocityX = Math.Clamp(platformerPlayerController.Rigidbody2D.linearVelocityX, -MaxMovementSpeed, MaxMovementSpeed);
                        break;

                    case MovementMode.Sprint:
                        platformerPlayerController.Rigidbody2D.AddRelativeForceX(movement.x * SprintMovementSpeed);
                        platformerPlayerController.Rigidbody2D.linearVelocityX = Math.Clamp(platformerPlayerController.Rigidbody2D.linearVelocityX, -SprintMaxMovementSpeed, SprintMaxMovementSpeed);
                        break;
                }
            }
        }

        /// <summary>
        ///     Updates the movement mechanics of the platformer entity based on player input and movement constraints.
        /// </summary>
        public override void Update()
        {
            if (PlayerController is not PlatformerPlayerController
                {
                    MovementController: PlatformerMovementController { MovementData: PlatformerMovementData platformerMovementData },
                    InputActions: PlatformerInputActions
                    {
                        Player:
                        {
                            Move:
                            {
                                inProgress: true
                            }
                        }
                    } platformerInputAction
                } platformerPlayerController)
            {
                return;
            }

            var movementAxis = platformerInputAction.Player.Move.ReadValue<Vector2>().x;

            movementAxis *= platformerMovementData.IsGrounded ? 1 : MovementApexJumpModifier.Evaluate(platformerPlayerController.Rigidbody2D.linearVelocityY);

            switch (platformerInputAction.Player.Sprint.inProgress)
            {
                case true:
                    platformerPlayerController.Rigidbody2D.AddRelativeForceX(movementAxis * SprintMovementSpeed);
                    platformerPlayerController.Rigidbody2D.linearVelocityX = Math.Clamp(platformerPlayerController.Rigidbody2D.linearVelocityX, -SprintMaxMovementSpeed, SprintMaxMovementSpeed);
                    break;

                case false:
                    platformerPlayerController.Rigidbody2D.AddRelativeForceX(movementAxis * MovementSpeed);
                    platformerPlayerController.Rigidbody2D.linearVelocityX = Math.Clamp(platformerPlayerController.Rigidbody2D.linearVelocityX, -MaxMovementSpeed, MaxMovementSpeed);
                    break;
            }
        }
    }
}