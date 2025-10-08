using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.PlayerController;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    public sealed class PlatformerPlayerController : MonoBehaviour, IPlayerController
    {
        public GameObject GameObject => gameObject;
        [field: SerializeField] public string EntityID { get; private set; } = "Platformer.Player";

        [field: Header("Components")]
        [field: SerializeField] public Rigidbody2D Rigidbody2D { private set; get; }

        [field: SerializeField] public Animator Animator { private set; get; }

        public IPlayerMovementController MovementController { get; set; }
        public IPlayerRendererController RendererController { get; set; }

        [field: SerializeField] public SpriteRenderer SpriteRenderer { private set; get; }

        //Evemts
        public event Action<IRespawnable> OnDespawn;
        public event Action<IRespawnable> OnRespawn;
        public event Action OnEditorSelected;

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
        }

        private void OnDisable()
        {
            PlatformerInputActions.Disable();
        }

        private void OnDrawGizmosSelected()
        {
            OnEditorSelected?.Invoke();
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

        public void DealDamage(float damage)
        {
        }
    }
}