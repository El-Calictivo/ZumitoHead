using System;
using Cysharp.Threading.Tasks;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.GameEntitites;
using Payosky.CoreMechanics.PlayerController;
using Payosky.CoreMechanics.Runtime;
using Payosky.Platformer.States;
using Payosky.Utilities.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Payosky.Platformer
{
    public sealed class PlatformerPlayerController : MonoBehaviour, IPlayerController
    {
        [field: SerializeField] public string EntityID { get; private set; } = "Platformer.Player";
        public IPlayerState CurrentState { get; private set; }
        public GameObject GameObject => gameObject;

        [field: Header("Player Controller Components")]
        [field: SerializeReference] [field: SubclassSelector]
        public IPlayerMovementController MovementController { get; set; }

        [field: SerializeReference] [field: SubclassSelector]
        public IPlayerRendererController RendererController { get; set; }

        [field: Header("Components")]
        [field: SerializeField] public Rigidbody2D Rigidbody2D { private set; get; }

        [field: SerializeField] public Animator Animator { private set; get; }

        [field: SerializeField] public Renderer Renderer { private set; get; }

        //Events
        public event Action<IRespawnable> OnDespawn;
        public event Action<IRespawnable> OnRespawn;
        public event Action OnEditorSelected;

        public IInputActionCollection2 InputActions { set; get; }

        [Header("Settings")]
        [SerializeField] private bool debug;

        public async UniTask SetState(IPlayerState playerState)
        {
            if (CurrentState != null)
            {
                if (debug) Debug.Log($"{typeof(PlatformerPlayerController).GetLoggingTag()}: Exiting {CurrentState.GetType().Name} State");
                await CurrentState.Exit();
            }

            CurrentState = playerState;
            if (CurrentState != null)
            {
                if (debug) Debug.Log($"{typeof(PlatformerPlayerController).GetLoggingTag()}: Starting {CurrentState.GetType().Name} State");
                await CurrentState.Start();
            }
        }

        private void Start()
        {
            RegisterEntity();
            SetState(new PlatformerSpawnState(this)).Forget();
        }

        private void Update()
        {
            CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
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
            await UniTask.Delay(1500);
        }

        async UniTask IRespawnable.Respawn()
        {
            await SetState(new PlatformerSpawnState(this));
        }

        public void DealDamage(float damage)
        {
        }
    }
}