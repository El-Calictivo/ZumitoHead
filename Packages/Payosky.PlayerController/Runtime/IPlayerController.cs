using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Runtime;

namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerController : IRespawnable, IGameEntity
    {
        void InitComponents(bool includeInactive = false);
        void DisposeComponents();
    }
}