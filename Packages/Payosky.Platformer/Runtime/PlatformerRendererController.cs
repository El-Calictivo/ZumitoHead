using System;
using DG.Tweening;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public sealed class PlatformerRendererController : MonoBehaviour, IPlayerRendererController
    {
        private PlatformerPlayerController _controller;

        private Vector3 _visualsOriginalScale;

        public void Init(IPlayerController controller)
        {
            _controller = (PlatformerPlayerController)controller;

            _controller.OnRespawn += OnRespawn;
            _controller.OnDespawn += OnDespawn;

            _visualsOriginalScale = _controller.SpriteRenderer.transform.localScale;
        }

        private void Update()
        {
            var movementAxis = _controller.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            _controller.SpriteRenderer.flipX = movementAxis.x switch
            {
                < 0 => true,
                > 0 => false,
                _ => _controller.SpriteRenderer.flipX
            };
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            _controller.SpriteRenderer.transform.DOShakeScale(0.1f, other.relativeVelocity * 0.1f, 24).SetRelative()
                .SetEase(Ease.InOutBounce)
                .OnComplete(() => _controller.SpriteRenderer.transform.localScale = _visualsOriginalScale);
        }

        public void Dispose()
        {
            _controller.OnRespawn -= OnRespawn;
            _controller.OnDespawn -= OnDespawn;
        }

        public void OnRespawn(IRespawnable respawnable)
        {
            _controller.SpriteRenderer.enabled = true;
        }

        public void OnDespawn(IRespawnable respawnable)
        {
            _controller.SpriteRenderer.enabled = false;
        }
    }
}