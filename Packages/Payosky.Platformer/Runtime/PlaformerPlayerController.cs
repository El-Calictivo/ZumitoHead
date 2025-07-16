using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    public sealed class PlatformerPlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private string id = "Platformer.Player";

        [Header("Components")]
        [field: SerializeField] public Rigidbody2D Rigidbody2D { private set; get; }

        [field: SerializeField] public Animator Animator { private set; get; }
        [field: SerializeField] public PlatformerMovementController MovementController { private set; get; }
        [field: SerializeField] public PlatformerRendererController RendererController { private set; get; }
        [field: SerializeField] public SpriteRenderer SpriteRenderer { private set; get; }

        public Action<IRespawnable> OnDespawn;

        public Action<IRespawnable> OnRespawn;

        public Action OnEditorSelected;

        public PlatformerInputActions PlatformerInputActions { private set; get; }

        private void Awake()
        {
            PlatformerInputActions = new PlatformerInputActions();
        }

        private void Start()
        {
            RegisterEntity();
        }

        private void OnEnable()
        {
            PlatformerInputActions.Enable();
            InitComponents();
        }

        private void OnDisable()
        {
            PlatformerInputActions.Disable();
            DisposeComponents();
        }

        private void OnDrawGizmosSelected()
        {
            OnEditorSelected?.Invoke();
        }

        public string GetID()
        {
            return id;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
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

        async UniTask IRespawnable.Despawn()
        {
            PlatformerInputActions.Disable();
            OnDespawn?.Invoke(this);
            await UniTask.Delay(1500);
        }

        UniTask IRespawnable.Respawn()
        {
            PlatformerInputActions.Enable();
            OnRespawn?.Invoke(this);
            return UniTask.CompletedTask;
        }

        public void InitComponents(bool includeInactive = false)
        {
            MovementController?.Init(this);
            RendererController?.Init(this);
        }

        public void DisposeComponents()
        {
            MovementController?.Dispose();
            RendererController?.Dispose();
        }

        public void DealDamage(float damage)
        {
        }
    }
}