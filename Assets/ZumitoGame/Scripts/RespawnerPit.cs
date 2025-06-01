using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace ZumitoGame
{
    public class RespawnerPit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var respawnable = other.GetComponentInParent<IRespawnable>();
            if (respawnable != null)
            {
                HandleRespawn(respawnable).Forget();
            }
        }

        private async UniTask HandleRespawn(IRespawnable respawnable)
        {
            await respawnable.Despawn();
            await respawnable.Respawn();
        }
    }
}