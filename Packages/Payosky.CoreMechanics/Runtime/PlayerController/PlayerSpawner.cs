using Payosky.Architecture.EventManager;
using Payosky.Architecture.Services;
using Payosky.CoreMechanics.Runtime.GameEvents;
using UnityEngine;
using UnityEngine.Animations;

namespace Payosky.CoreMechanics.PlayerController
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _playerPrefab;

        [SerializeField] private ParentConstraint _playerPositionReference;

        [SerializeField] private SpawnModes _spawnMode = SpawnModes.GameEvent;

        private Transform _spawnedPlayer;

        private void Start()
        {
            var gameEventService = ServiceLocator.Get<GameEventService>();

            switch (_spawnMode)
            {
                case SpawnModes.Start:
                    SpawnPlayer();
                    break;

                case SpawnModes.GameEvent:
                    gameEventService?.Register<SpawnPlayersGameEvent>(OnSpawnPlayer);
                    break;

                case SpawnModes.FallbackToStart:
                default:
                    if (gameEventService != null)
                    {
                        gameEventService.Register<SpawnPlayersGameEvent>(OnSpawnPlayer);
                    }
                    else
                    {
                        SpawnPlayer();
                    }

                    break;
            }
        }

        private void OnSpawnPlayer(SpawnPlayersGameEvent _)
        {
            SpawnPlayer();
        }

        [ContextMenu(nameof(SpawnPlayer))]
        public void SpawnPlayer()
        {
            if (_spawnedPlayer == null)
            {
                _spawnedPlayer = Instantiate(_playerPrefab);
                _playerPositionReference.AddSource(new ConstraintSource { sourceTransform = _spawnedPlayer, weight = 1 });
            }

            _spawnedPlayer.SetPositionAndRotation(transform.position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (ServiceLocator.TryGet(out GameEventService gameEventService))
            {
                gameEventService.Unregister<SpawnPlayersGameEvent>(OnSpawnPlayer);
            }
        }

        public enum SpawnModes
        {
            Start,

            GameEvent,

            FallbackToStart
        }
    }
}