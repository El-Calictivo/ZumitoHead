namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerMovementController : IPlayerControllerComponent
    {
        void RegisterMovementSubsystem(MovementSubsystem subsystem);
        void RegisterJumpSubsystem(JumpSubsystem subsystem);
        void RegisterGroundCheckSubsystem(GroundCheckSubsystem subsystem);

        void UnRegisterMovementSubsystem();
        void UnRegisterJumpSubsystem();
        void UnRegisterGroundCheckSubsystem();

        MovementSubsystem MovementSubsystem { get; }
        JumpSubsystem JumpSubsystem { get; }
        GroundCheckSubsystem GroundCheckSubsystem { get; }
    }
}