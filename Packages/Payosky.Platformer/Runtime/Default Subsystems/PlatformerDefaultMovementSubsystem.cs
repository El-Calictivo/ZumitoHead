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
            platformerPlayerController.Rigidbody2D.linearVelocityX = movementAxis * MovementSpeed * Time.deltaTime;
        }
    }
}