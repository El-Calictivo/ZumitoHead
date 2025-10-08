using Payosky.CoreMechanics.PlayerController;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(IPlayerController))]
    public sealed class PlatformerMovementController : MonoBehaviour, IPlayerMovementController
    {
        public IPlayerController PlayerController { get; private set; }
        public IPlayerMovementData MovementData => movementData;

        [field: Space(5)]
        [field: Header("Subsystems")]
        [field: SerializeReference] [field: SubclassSelector]
        public MovementSubsystem MovementSubsystem { private set; get; }

        [field: Space(5)]
        [field: SerializeReference] [field: SubclassSelector]
        public JumpSubsystem JumpSubsystem { private set; get; }

        [field: Space(5)]
        [field: SerializeReference] [field: SubclassSelector]
        public GroundCheckSubsystem GroundCheckSubsystem { private set; get; }

        [Space(5)]
        [SerializeReference]
        private PlatformerMovementData movementData = new();

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

        public void Init(IPlayerController controller)
        {
            PlayerController = controller;
            PlayerController.MovementController = this;

            PlayerController.OnRespawn += OnRespawn;
            PlayerController.OnDespawn += OnDespawn;

            RegisterJumpSubsystem(JumpSubsystem);
            RegisterMovementSubsystem(MovementSubsystem);
            RegisterGroundCheckSubsystem(GroundCheckSubsystem);
        }

        private void FixedUpdate()
        {
            GroundCheckSubsystem?.Update();
            JumpSubsystem?.Update();
            MovementSubsystem?.Update();
        }

        public void Dispose()
        {
            PlayerController.OnRespawn -= OnRespawn;
            PlayerController.OnDespawn -= OnDespawn;

            UnRegisterJumpSubsystem();
            UnRegisterMovementSubsystem();
            UnRegisterGroundCheckSubsystem();
        }

        public void OnSpawn(IRespawnable respawnable)
        {
        }

        public void OnRespawn(IRespawnable respawnable)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                platformerPlayerController.Rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        public void OnDespawn(IRespawnable respawnable)
        {
            if (PlayerController is PlatformerPlayerController platformerPlayerController)
            {
                platformerPlayerController.Rigidbody2D.bodyType = RigidbodyType2D.Static;
            }
        }

        public void RegisterMovementSubsystem(MovementSubsystem subsystem)
        {
            if (subsystem == null) return;
            MovementSubsystem = subsystem;
            MovementSubsystem.Initialize(PlayerController);
        }

        public void RegisterJumpSubsystem(JumpSubsystem subsystem)
        {
            if (PlayerController is not PlatformerPlayerController platformerPlayerController) return;

            JumpSubsystem = subsystem;
            JumpSubsystem.Initialize(PlayerController);

            platformerPlayerController.PlatformerInputActions.Player.Jump.performed += JumpSubsystem.HandleJump;
            platformerPlayerController.PlatformerInputActions.Player.Jump.canceled += JumpSubsystem.HandleJump;
        }

        public void RegisterGroundCheckSubsystem(GroundCheckSubsystem subsystem)
        {
            if (subsystem == null) return;

            GroundCheckSubsystem = subsystem;
            GroundCheckSubsystem.Initialize(PlayerController);
        }

        public void UnRegisterMovementSubsystem()
        {
            if (GroundCheckSubsystem is null) return;
            GroundCheckSubsystem.Dispose();
            GroundCheckSubsystem = null;
        }

        public void UnRegisterJumpSubsystem()
        {
            if (PlayerController is not PlatformerPlayerController platformerPlayerController) return;

            platformerPlayerController.PlatformerInputActions.Player.Jump.performed -= JumpSubsystem.HandleJump;
            platformerPlayerController.PlatformerInputActions.Player.Jump.canceled -= JumpSubsystem.HandleJump;
            JumpSubsystem.Dispose();
            JumpSubsystem = null;
        }

        public void UnRegisterGroundCheckSubsystem()
        {
            if (GroundCheckSubsystem is null) return;
            GroundCheckSubsystem.Dispose();
            GroundCheckSubsystem = null;
        }
    }
}