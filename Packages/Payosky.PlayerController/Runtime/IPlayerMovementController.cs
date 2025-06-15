namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerMovementController : IPlayerControllerComponent
    {
        void RegisterMovementSubsystem(MovementSubsystem subsystem);
        void RegisterJumpSubsystem(JumpSubsystem subsystem);
        void RegisterGroundCheckSubsystem(GroundCheckSubsystem subsystem);
    }
}