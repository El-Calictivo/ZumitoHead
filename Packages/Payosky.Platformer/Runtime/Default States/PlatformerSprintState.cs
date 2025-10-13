using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Payosky.Platformer.States
{
    /// <summary>
    /// <inheritdoc/>
    /// Represents the Idle state for a platformer player character.
    /// </summary>
    public class PlatformerSprintState : IPlayerState
    {
        private readonly PlatformerPlayerController _playerController;

        private readonly PlatformerInputActions _platformerInputActions;

        private Vector2 _moveInput;

        private const float MinMovementAxisValue = 0.65f;

        private const float MaxMovementAxisValue = 1f;

        private readonly PlayerStateTransition _toIdleState;

        public PlatformerSprintState(PlatformerPlayerController playerController)
        {
            _playerController = playerController;
            _platformerInputActions = _playerController.InputActions as PlatformerInputActions;
            _toIdleState = new PlayerStateTransition(() => !_platformerInputActions.Player.Sprint.inProgress || !_platformerInputActions.Player.Move.inProgress, () =>
            {
                _playerController.RendererController.SetAnimationSpeed(1);
                _playerController.SetState(new PlatformerIdleState(_playerController)).Forget();
            });
        }

        /// <inheritdoc/>
        public UniTask Start()
        {
            _platformerInputActions.Player.Move.performed += OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Move.canceled += OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Sprint.canceled += OnPLayerSprintInputActionChanged;

            OnPlayerMoving(_platformerInputActions.Player.Move.ReadValue<Vector2>());

            _toIdleState.Validate();

            return UniTask.CompletedTask;
        }

        /// <inheritdoc/>
        public void FixedUpdate()
        {
            _playerController.MovementController.MovementSubsystem.MoveHorizontally(_moveInput, MovementSubsystem.MovementMode.Sprint);
            _playerController.MovementController.GroundCheckSubsystem.Update();
        }

        private void OnPlayerMoveInputActionChanged(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    OnPlayerMoving(context.ReadValue<Vector2>());
                    break;
                case InputActionPhase.Canceled:
                    _toIdleState.Validate();
                    break;
            }
        }

        private void OnPlayerMoving(Vector2 movement)
        {
            _moveInput = SanitizeMovementInput(movement);
            _playerController.RendererController.SetAnimationSpeed(Mathf.Abs(_moveInput.x) * 1.5f);
            _playerController.RendererController.HandleVelocityDirection(_moveInput);
            _playerController.RendererController.PlayAnimation("Run");
        }

        private void OnPLayerSprintInputActionChanged(InputAction.CallbackContext context)
        {
            _toIdleState.Validate();
        }

        /// <inheritdoc/>
        public UniTask Exit()
        {
            _platformerInputActions.Player.Move.performed -= OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Move.canceled -= OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Sprint.canceled -= OnPLayerSprintInputActionChanged;

            return UniTask.CompletedTask;
        }

        private static Vector2 SanitizeMovementInput(Vector2 movement)
        {
            movement.x = movement.x switch
            {
                > 0 => Mathf.Clamp(movement.x, MinMovementAxisValue, MaxMovementAxisValue),
                < 0 => Mathf.Clamp(movement.x, -MaxMovementAxisValue, -MinMovementAxisValue),
                _ => movement.x
            };

            return movement;
        }
    }
}