using System;
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
    public class PlatformerWalkState : IPlayerState
    {
        private readonly PlatformerPlayerController _playerController;

        private readonly PlatformerInputActions _platformerInputActions;

        private Vector2 _moveInput;

        private const float MinMovementAxisValue = 0.65f;

        private const float MaxMovementAxisValue = 1f;

        private readonly PlayerStateTransition _toIdleState;

        private readonly PlayerStateTransition _toSprintingState;

        public PlatformerWalkState(PlatformerPlayerController playerController)
        {
            _playerController = playerController;
            _platformerInputActions = _playerController.InputActions as PlatformerInputActions;

            _toIdleState = new PlayerStateTransition(() => !_platformerInputActions.Player.Move.inProgress, () =>
            {
                _playerController.RendererController.SetAnimationSpeed(1);
                _playerController.SetState(new PlatformerIdleState(_playerController)).Forget();
            });

            _toSprintingState = new PlayerStateTransition(() => _platformerInputActions.Player.Sprint.inProgress,
                () => _playerController.SetState(new PlatformerSprintState(_playerController)).Forget()
            );
        }

        /// <inheritdoc/>
        public UniTask Start()
        {
            _platformerInputActions.Player.Move.performed += OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Move.canceled += OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Sprint.started += OnPlayerSprintInputChanged;

            OnPlayerMoving(_platformerInputActions.Player.Move.ReadValue<Vector2>());

            _toIdleState.Validate();
            _toSprintingState.Validate();

            return UniTask.CompletedTask;
        }

        /// <inheritdoc/>
        public void FixedUpdate()
        {
            _playerController.MovementController.MovementSubsystem.MoveHorizontally(_moveInput, MovementSubsystem.MovementMode.Walk);
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
            _playerController.RendererController.SetAnimationSpeed(Mathf.Abs(_moveInput.x));
            _playerController.RendererController.HandleVelocityDirection(_moveInput);
            _playerController.RendererController.PlayAnimation("Run");
        }

        private void OnPlayerSprintInputChanged(InputAction.CallbackContext context)
        {
            _toSprintingState.Validate();
        }

        /// <inheritdoc/>
        public UniTask Exit()
        {
            _platformerInputActions.Player.Move.performed -= OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Move.canceled -= OnPlayerMoveInputActionChanged;
            _platformerInputActions.Player.Sprint.started -= OnPlayerSprintInputChanged;

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