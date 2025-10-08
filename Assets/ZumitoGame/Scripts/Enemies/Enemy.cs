using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZumitoGame.Entities
{
    public class Enemy : MonoBehaviour, IGameEntity
    {
        public GameObject GameObject => gameObject;
        [field: SerializeField] public string EntityID { get; protected set; } = "";
        [field: SerializeField] public Animator Animator { private set; get; }

        public void RegisterEntity()
        {
            if (ServiceLocator.TryGet(out GameEntityService entityService))
            {
                entityService.AddEntity(this);
            }
        }

        public void UnregisterEntity()
        {
            if (ServiceLocator.TryGet(out GameEntityService entityService))
            {
                entityService.RemoveEntity(this);
            }
        }

        public void DealDamage(float damage)
        {
            HandleDeactivation().Forget();
        }

        public async UniTaskVoid HandleDeactivation()
        {
            Animator.Play("Destroyed");

            await UniTask.WaitUntil(() =>
            {
                var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName("Destroyed") && stateInfo.normalizedTime >= 1f;
            });

            gameObject.SetActive(false);
        }
    }
}