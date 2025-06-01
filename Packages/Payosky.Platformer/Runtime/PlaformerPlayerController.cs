using System;
using System.Collections.Generic;
using System.Linq;
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

        public Action<IRespawnable> OnRespawn;

        public Action<IRespawnable> OnDespawn;

        private List<IPlayerControllerComponent> _components = new();

        private void Awake()
        {
            PlatformerInputActions = new PlatformerInputActions();
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
            foreach (var component in GetComponentsInChildren<IPlayerControllerComponent>(includeInactive))
            {
                if (_components.Contains(component)) continue;
                component.Init(this);
                _components.Add(component);
            }
        }

        public void DisposeComponents()
        {
            foreach (var component in _components.ToList()) component.Dispose();

            _components.Clear();
        }
    }
}