using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public sealed class PlatformerMovementController : MonoBehaviour, IPlayerMovementController
    {
        [field: Space(5)]
        [field: SerializeField] public PlatformerAttributes PlatformerAttributes { private set; get; }

        [field: Space(5)]
        [field: Header("Subsystems")]
        [field: SerializeReference] [field: SubclassSelector]
        public MovementSubsystem MovementSubsystem { private set; get; }

        [field: SerializeReference] [field: SubclassSelector]
        public JumpSubsystem JumpSubsystem { private set; get; }

        [field: SerializeReference] [field: SubclassSelector]
        public GroundCheckSubsystem GroundCheckSubsystem { private set; get; }

        [Header("Movement State Information")]
        [HideInInspector] public float coyoteTimeCounter;

        [HideInInspector] public bool doHoldJump;

        [HideInInspector] public bool isGrounded;

        [HideInInspector] public bool isJumpCharging;

        [HideInInspector] public float jumpBufferCounter;

        [HideInInspector] public float jumpHoldCounter;

        [HideInInspector] public Vector2 lastGroundedPosition;

        private PlatformerPlayerController _controller;

        private void Update()
        {
            GroundCheckSubsystem?.Update();
            JumpSubsystem?.Update();
            MovementSubsystem?.Update();
        }

        public void Init(IPlayerController controller)
        {
            _controller = (PlatformerPlayerController)controller;

            _controller.OnRespawn += OnRespawn;
            _controller.OnDespawn += OnDespawn;

            _controller.PlatformerInputActions.Player.Jump.performed += JumpSubsystem.HandleJump;
            _controller.PlatformerInputActions.Player.Jump.canceled += JumpSubsystem.HandleJump;

            MovementSubsystem.SetPlayerController(_controller);
            JumpSubsystem.SetPlayerController(_controller);
            GroundCheckSubsystem.SetPlayerController(_controller);
        }

        public void Dispose()
        {
            _controller.OnRespawn -= OnRespawn;
            _controller.OnDespawn -= OnDespawn;

            _controller.PlatformerInputActions.Player.Jump.performed -= JumpSubsystem.HandleJump;
            _controller.PlatformerInputActions.Player.Jump.canceled -= JumpSubsystem.HandleJump;
        }

        public void OnRespawn(IRespawnable respawnable)
        {
            transform.position = lastGroundedPosition;
            _controller.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _controller.Rigidbody2D.linearVelocity = Vector2.zero;
        }

        public void OnDespawn(IRespawnable respawnable)
        {
            _controller.Rigidbody2D.bodyType = RigidbodyType2D.Static;
        }

        public void RegisterMovementSubsystem(MovementSubsystem subsystem)
        {
            MovementSubsystem = subsystem;
            MovementSubsystem?.SetPlayerController(_controller);
        }

        public void RegisterJumpSubsystem(JumpSubsystem subsystem)
        {
            JumpSubsystem = subsystem;
            JumpSubsystem?.SetPlayerController(_controller);
        }

        public void RegisterGroundCheckSubsystem(GroundCheckSubsystem subsystem)
        {
            GroundCheckSubsystem = subsystem;
            GroundCheckSubsystem?.SetPlayerController(_controller);
        }

        public void OnSpawn(IRespawnable respawnable)
        {
        }
    }
}