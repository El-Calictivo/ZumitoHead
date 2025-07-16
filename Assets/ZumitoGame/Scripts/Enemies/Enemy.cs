using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using UnityEngine;

namespace ZumitoGame.Entities
{
    public class Enemy : MonoBehaviour, IGameEntity
    {
        [SerializeField] private string _id;

        [field: SerializeField] public Animator Animator { private set; get; }

        public string GetID()
        {
            return _id;
        }

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