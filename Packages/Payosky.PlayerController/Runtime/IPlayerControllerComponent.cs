using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.Runtime;

namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerControllerComponent
    {
        void OnPlayerDespawned(IRespawnable player);

        void OnPlayerRespawned(IRespawnable respawned);
    }
}