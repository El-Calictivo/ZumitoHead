using DG.Tweening;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public class PlatformerRendererController : MonoBehaviour, IPlayerRendererController
    {
        private PlatformerPlayerController _controller;

        private Vector3 _visualsOriginalScale;

        private void Update()
        {
            var movementAxis = _controller.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            _controller.SpriteRenderer.flipX = movementAxis.x switch
            {
                < 0 => true,
                > 0 => false,
                _ => _controller.SpriteRenderer.flipX
            };

            UpdateAnimator();
        }

        protected virtual void UpdateAnimator()
        {
            if (_controller.MovementController.isGrounded)
            {
                _controller.Animator.Play(_controller.PlatformerInputActions.Player.Move.inProgress ? "Run" : "Idle");
            }
            else
            {
                switch (_controller.Rigidbody2D.linearVelocityY)
                {
                    case > 0.2f:
                        _controller.Animator.Play("Jump");
                        break;
                    case < -0.2f:
                        _controller.Animator.Play("Fall");
                        break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            _controller.SpriteRenderer.transform.DOShakeScale(0.1f, other.relativeVelocity * 0.1f, 24).SetRelative()
                .SetEase(Ease.InOutBounce)
                .OnComplete(() => _controller.SpriteRenderer.transform.localScale = _visualsOriginalScale);
        }

        public void Init(IPlayerController controller)
        {
            _controller = (PlatformerPlayerController)controller;

            _controller.OnRespawn += OnRespawn;
            _controller.OnDespawn += OnDespawn;

            _visualsOriginalScale = _controller.SpriteRenderer.transform.localScale;
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