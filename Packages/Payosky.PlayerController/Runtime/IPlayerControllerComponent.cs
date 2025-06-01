using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.Runtime;

namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerControllerComponent
    {
        public void Init(IPlayerController playerController);
        public void Dispose();
        public void OnRespawn(IRespawnable respawnable);
        public void OnDespawn(IRespawnable respawnable);
    }
}