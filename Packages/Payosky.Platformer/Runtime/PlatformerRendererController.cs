using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public class PlatformerRendererController : MonoBehaviour, IPlayerRendererController
    {
        private PlatformerPlayerController _controller;

        IPlayerController IPlayerControllerComponent.PlayerController
        {
            get => _controller;
            set => _controller = (PlatformerPlayerController)value;
        }

        private Vector3 _visualsOriginalScale;

        private CancellationTokenSource _springTokenSource;

        private void Update()
        {
            UpdateAnimator();
        }

        protected virtual void UpdateAnimator()
        {
            _controller.Animator.speed = 1;
            var movementAxis = _controller.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            if (movementAxis != Vector2.zero)
            {
                _controller.SpriteRenderer.flipX = movementAxis.x switch
                {
                    < 0 => true,
                    > 0 => false,
                    _ => _controller.SpriteRenderer.flipX
                };
            }

            if (_controller.MovementController.isGrounded)
            {
                if (_controller.PlatformerInputActions.Player.Move.inProgress && movementAxis != Vector2.zero)
                {
                    _controller.Animator.speed = Mathf.Abs(movementAxis.x);
                    _controller.Animator.Play("Run");
                }
                else
                {
                    _controller.Animator.Play("Idle");
                }
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
            _controller.SpriteRenderer.transform.DOShakeScale(0.1f, Mathf.Clamp(other.relativeVelocity.y * 0.1f, 0.2f, 5), 24).SetRelative()
                .SetEase(Ease.InOutBounce)
                .OnComplete(() => _controller.SpriteRenderer.transform.localScale = _visualsOriginalScale);
        }

        private void HandleSpringAnimation(InputAction.CallbackContext obj)
        {
            HandleSpringAnimationAsync(obj).Forget();
        }

        private void CancelSpringAnimation(InputAction.CallbackContext obj)
        {
            _springTokenSource?.Cancel();
        }

        private async UniTaskVoid HandleSpringAnimationAsync(InputAction.CallbackContext callbackContext)
        {
            if (!_controller.MovementController.isGrounded)
            {
                return;
            }

            _springTokenSource?.Cancel();
            _springTokenSource = new CancellationTokenSource();

            try
            {
                await _controller.SpriteRenderer.transform.DOScale(
                        new Vector3(_visualsOriginalScale.x * 1.4f, _visualsOriginalScale.y * 0.6f, 1),
                        _controller.MovementController.JumpSubsystem.MaxJumpHoldTime)
                    .WithCancellation(_springTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
            }
            finally
            {
                _controller.SpriteRenderer.transform.localScale = _visualsOriginalScale;
            }
        }

        public void Init(IPlayerController controller)
        {
            _controller = (PlatformerPlayerController)controller;

            _controller.OnRespawn += OnRespawn;
            _controller.OnDespawn += OnDespawn;

            _visualsOriginalScale = _controller.SpriteRenderer.transform.localScale;

            _controller.PlatformerInputActions.Player.Jump.performed += HandleSpringAnimation;
            _controller.PlatformerInputActions.Player.Jump.canceled += CancelSpringAnimation;
        }

        public void Dispose()
        {
            _controller.OnRespawn -= OnRespawn;
            _controller.OnDespawn -= OnDespawn;
            _controller.PlatformerInputActions.Player.Jump.performed -= HandleSpringAnimation;
            _controller.PlatformerInputActions.Player.Jump.canceled -= CancelSpringAnimation;
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