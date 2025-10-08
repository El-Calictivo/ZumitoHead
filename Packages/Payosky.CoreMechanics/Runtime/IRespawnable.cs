using System;
using Cysharp.Threading.Tasks;

namespace Payosky.CoreMechanics.Runtime
{
    public interface IRespawnable
    {
        UniTask Despawn();
        UniTask Respawn();

        //Actions
        event Action<IRespawnable> OnDespawn;
        event Action<IRespawnable> OnRespawn;
    }
}