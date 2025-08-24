using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace Payosky.CoreMechanics.PlayerController
{
    public interface IPlayerController : IRespawnable, IGameEntity
    {
        GameObject GameObject { get; }
        void InitComponents(bool includeInactive = false);
        void DisposeComponents();
    }
}