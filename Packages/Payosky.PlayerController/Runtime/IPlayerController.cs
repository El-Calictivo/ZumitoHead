using Payosky.CoreMechanics.Runtime;

namespace Payosky.PlayerController.Runtime
{
    public interface IPlayerController : IRespawnable
    {
        void InitComponents(bool includeInactive = false);
        void DisposeComponents();
    }
}