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

        /// <summary>
        ///     Updates the movement mechanics of the platformer entity based on player input and movement constraints.
        /// </summary>
        public override void Update()
        {
            if (PlayerController is not PlatformerPlayerController
                {
                    MovementController: PlatformerMovementController { MovementData: PlatformerMovementData platformerMovementData },
                    PlatformerInputActions:
                    {
                        Player:
                        {
                            Move:
                            {
                                inProgress: true
                            }
                        }
                    }
                } platformerPlayerController)
            {
                return;
            }

            var movementAxis = platformerPlayerController.PlatformerInputActions.Player.Move.ReadValue<Vector2>().x;

            movementAxis *= platformerMovementData.IsGrounded ? 1 : MovementApexJumpModifier.Evaluate(platformerPlayerController.Rigidbody2D.linearVelocityY);

            switch (platformerPlayerController.PlatformerInputActions.Player.Sprint.inProgress)
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