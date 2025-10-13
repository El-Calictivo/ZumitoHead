using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.PlayerController;
using UnityEngine.InputSystem;

namespace Payosky.Platformer.States
{
    /// <summary>
    /// <inheritdoc/>
    /// Represents the Idle state for a platformer player character.
    /// </summary>
    public class PlatformerIdleState : IPlayerState
    {
        private readonly PlatformerPlayerController _playerController;

        private readonly PlatformerInputActions _platformerInputActions;

        private readonly PlayerStateTransition _toMovingState;

        public PlatformerIdleState(PlatformerPlayerController playerController)
        {
            _playerController = playerController;
            _platformerInputActions = _playerController.InputActions as PlatformerInputActions;
            _toMovingState = new PlayerStateTransition(() => _platformerInputActions.Player.Move.inProgress, () =>
            {
                if (_platformerInputActions.Player.Sprint.inProgress)
                {
                    _playerController.SetState(new PlatformerSprintState(_playerController)).Forget();
                }
                else
                {
                    _playerController.SetState(new PlatformerWalkState(_playerController)).Forget();
                }
            });
        }

        /// <inheritdoc/>
        public UniTask Start()
        {
            _platformerInputActions.Player.Move.started += OnPlayerMovementInputChanged;
            _playerController.RendererController.PlayAnimation("Idle");
            _toMovingState.Validate();

            return UniTask.CompletedTask;
        }

        /// <inheritdoc/>
        public void Update()
        {
        }

        /// <inheritdoc/>
        public void FixedUpdate()
        {
            _playerController.MovementController.GroundCheckSubsystem.Update();
        }

        private void OnPlayerMovementInputChanged(InputAction.CallbackContext context)
        {
            _toMovingState.Validate();
        }

        /// <inheritdoc/>
        public UniTask Exit()
        {
            _platformerInputActions.Player.Move.started -= OnPlayerMovementInputChanged;
            return UniTask.CompletedTask;
        }
    }
}