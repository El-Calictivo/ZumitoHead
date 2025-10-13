using Cysharp.Threading.Tasks;
using Payosky.CoreMechanics.PlayerController;

namespace Payosky.Platformer.States
{
    /// <summary>
    /// <inheritdoc/>
    /// Represents the spawn state for a platformer player controller.
    /// </summary>
    public class PlatformerSpawnState : IPlayerState
    {
        private IPlayerController _playerController;

        public PlatformerSpawnState(IPlayerController playerController)
        {
            _playerController = playerController;
        }

        /// <inheritdoc/>
        public async UniTask Start()
        {
            _playerController.InputActions ??= new PlatformerInputActions();
            _playerController.InputActions.Enable();
            _playerController.MovementController.Init(_playerController);
            _playerController.RendererController.Init(_playerController);
            await _playerController.SetState(new PlatformerIdleState((PlatformerPlayerController)_playerController));
        }

        /// <inheritdoc/>
        public void Update()
        {
        }

        /// <inheritdoc/>
        public void FixedUpdate()
        {
        }

        /// <inheritdoc/>
        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}