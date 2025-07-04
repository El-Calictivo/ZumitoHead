using Payosky.CoreMechanics.GameEntitites;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Payosky.PlayerController.Runtime
{
    public interface IMovementControllerSubsystem
    {
        void Initalize(IPlayerController playerController);
        void Dispose();
        void Update();
    }

    public abstract class JumpSubsystem : IMovementControllerSubsystem
    {
        protected IPlayerController PlayerController;

        [Header("Jump")]
        public float JumpForce = 12;

        public float MaxJumpHoldTime = 0.3f;

        public virtual void Initalize(IPlayerController playerController)
        {
            PlayerController = playerController;
        }

        public abstract void Update();
        public abstract void HandleJump(InputAction.CallbackContext context);
        public abstract void Dispose();
    }

    public abstract class GroundCheckSubsystem : IMovementControllerSubsystem
    {
        protected IPlayerController PlayerController;

        public virtual void Initalize(IPlayerController playerController)
        {
            PlayerController = playerController;
        }

        public abstract void Update();
        public abstract void Dispose();
    }

    public abstract class MovementSubsystem : IMovementControllerSubsystem
    {
        protected IPlayerController PlayerController;

        [Header("Movement")]
        public float MovementSpeed = 10;

        public float MaxSpeed = 5;

        public virtual void Initalize(IPlayerController playerController)
        {
            PlayerController = playerController;
        }

        public abstract void Update();
        public abstract void Dispose();
    }
}