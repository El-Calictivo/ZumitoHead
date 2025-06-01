using Cysharp.Threading.Tasks;

namespace Payosky.CoreMechanics.Runtime
{
    public interface IRespawnable
    {
        UniTask Despawn();
        UniTask Respawn();
    }
}