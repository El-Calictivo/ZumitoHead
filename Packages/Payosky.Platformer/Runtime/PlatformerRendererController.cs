using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Payosky.CoreMechanics.PlayerController;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public sealed class PlatformerRendererController : MonoBehaviour, IPlayerRendererController
    {
        public IPlayerController PlayerController { get; private set; }

        private Vector3 _visualsOriginalScale;

        private CancellationTokenSource _springTokenSource;

        private void OnEnable()
        {
            if (TryGetComponent(out IPlayerController controller))
            {
                Init(controller);
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        private void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            if (PlayerController is not PlatformerPlayerController platformerPlayerController || PlayerController.MovementController.MovementData is not PlatformerMovementData platformerMovementData)
            {
                return;
            }

            PlayerController.Animator.speed = 1;
            var movementAxis = platformerPlayerController.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            if (movementAxis != Vector2.zero)
            {
                platformerPlayerController.SpriteRenderer.flipX = movementAxis.x switch
                {
                    < 0 => true,
                    > 0 => false,
                    _ => platformerPlayerController.SpriteRenderer.flipX
                };
            }

            if (platformerMovementData.IsGrounded)
            {
                if (platformerPlayerController.PlatformerInputActions.Player.Move.inProgress && movementAxis != Vector2.zero)
                {
                    platformerPlayerController.Animator.speed = Mathf.Abs(movementAxis.x);
                    platformerPlayerController.Animator.Play("Run");
                }
                else
                {
                    platformerPlayerController.Animator.Play("Idle");
                }
            }
            else
            {
                switch (platformerPlayerController.Rigidbody2D.linearVelocityY)
                {
                    case > 0.2f:
                        platformerPlayerController.Animator.Play("Jump");
                        break;
                    case < -0.2f:
                        platformerPlayerController.Animator.Play("Fall");
                        break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.SpriteRenderer.transform.DOShakeScale(0.1f, Mathf.Clamp(other.relativeVelocity.y * 0.1f, 0.2f, 5), 24).SetRelative()
                    .SetEase(Ease.InOutBounce)
                    .OnComplete(() => platformerPlayerController.SpriteRenderer.transform.localScale = _visualsOriginalScale);
            }
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
            if (PlayerController is not PlatformerPlayerController { MovementController: PlatformerMovementController { MovementData: PlatformerMovementData { IsGrounded: true } } } platformerPlayerController)
            {
                return;
            }


            _springTokenSource?.Cancel();
            _springTokenSource = new CancellationTokenSource();

            try
            {
                await platformerPlayerController.SpriteRenderer.transform.DOScale(
                        new Vector3(_visualsOriginalScale.x * 1.4f, _visualsOriginalScale.y * 0.6f, 1),
                        platformerPlayerController.MovementController.JumpSubsystem.MaxJumpHoldTime)
                    .WithCancellation(_springTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
            }
            finally
            {
                platformerPlayerController.SpriteRenderer.transform.localScale = _visualsOriginalScale;
            }
        }

        public void Init(IPlayerController controller)
        {
            PlayerController = controller;
            PlayerController.RendererController = this;

            PlayerController.OnRespawn += OnRespawn;
            PlayerController.OnDespawn += OnDespawn;

            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                _visualsOriginalScale = platformerPlayerController.SpriteRenderer.transform.localScale;
                platformerPlayerController.PlatformerInputActions.Player.Jump.performed += HandleSpringAnimation;
                platformerPlayerController.PlatformerInputActions.Player.Jump.canceled += CancelSpringAnimation;
            }
        }

        public void Dispose()
        {
            PlayerController.OnRespawn -= OnRespawn;
            PlayerController.OnDespawn -= OnDespawn;

            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.PlatformerInputActions.Player.Jump.performed -= HandleSpringAnimation;
                platformerPlayerController.PlatformerInputActions.Player.Jump.canceled -= CancelSpringAnimation;
            }
        }

        public void OnRespawn(IRespawnable respawnable)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.SpriteRenderer.enabled = true;
            }
        }

        public void OnDespawn(IRespawnable respawnable)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.SpriteRenderer.enabled = false;
            }
        }
    }
}