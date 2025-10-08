using Payosky.CoreMechanics.Runtime;

namespace Payosky.CoreMechanics.PlayerController
{
    public interface IPlayerControllerComponent
    {
        public IPlayerController PlayerController { get; }
        public void Init(IPlayerController playerController);
        public void Dispose();
        public void OnRespawn(IRespawnable respawnable);
        public void OnDespawn(IRespawnable respawnable);
    }
}