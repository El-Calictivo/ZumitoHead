using System;
using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    public sealed class PlatformerPlayerController : MonoBehaviour, IPlayerController
    {
        [Header("Components")]
        [field: SerializeField] public Rigidbody2D Rigidbody2D { private set; get; }

        public PlatformerInputActions PlatformerInputActions { private set; get; }
        [field: SerializeField] public SpriteRenderer SpriteRenderer { private set; get; }

        public Action<IPlayerController> OnSpawn;

        public Action<IRespawnable> OnRespawn;

        public Action<IRespawnable> OnDespawn;

        private void Awake()
        {
            PlatformerInputActions = new PlatformerInputActions();
        }

        private void OnEnable()
        {
            PlatformerInputActions.Enable();
        }

        private void OnDisable()
        {
            PlatformerInputActions.Disable();
        }

        private void Start()
        {
            OnSpawn?.Invoke(this);
        }

        public async UniTask Despawn()
        {
            PlatformerInputActions.Disable();
            OnDespawn?.Invoke(this);
            await UniTask.Delay(1500);
        }

        public UniTask Respawn()
        {
            PlatformerInputActions.Enable();
            OnRespawn?.Invoke(this);
            return UniTask.CompletedTask;
        }
    }
}