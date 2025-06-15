using UnityEngine.InputSystem;

namespace Payosky.PlayerController.Runtime
{
    public interface IMovementControllerSubsystem
    {
        public void SetPlayerController(IPlayerController playerController);
        void Update();
    }

    public abstract class JumpSubsystem : IMovementControllerSubsystem
    {
        public abstract void SetPlayerController(IPlayerController playerController);

        public abstract void Update();
        public abstract void HandleJump(InputAction.CallbackContext context);
    }

    public abstract class GroundCheckSubsystem : IMovementControllerSubsystem
    {
        public abstract void SetPlayerController(IPlayerController playerController);
        public abstract void Update();
    }

    public abstract class MovementSubsystem : IMovementControllerSubsystem
    {
        public abstract void SetPlayerController(IPlayerController playerController);

        public abstract void Update();
    }
}