using System;
using Payosky.Architecture.Editor.Attributes;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public sealed class PlatformerMovementController : MonoBehaviour, IPlayerMovementController
    {
        [field: Space(5)]
        [field: Header("Subsystems")]
        [field: SerializeReference] [field: SubclassSelector]
        public MovementSubsystem MovementSubsystem { private set; get; }

        [field: SerializeReference] [field: SubclassSelector]
        public JumpSubsystem JumpSubsystem { private set; get; }

        [field: SerializeReference] [field: SubclassSelector]
        public GroundCheckSubsystem GroundCheckSubsystem { private set; get; }

        [Header("Movement State Information")]
        [ReadOnly]
        public float coyoteTimeCounter;

        [ReadOnly] public bool isGrounded;

        [ReadOnly] public bool isJumpCharging;

        [ReadOnly] public bool justLanded;

        [ReadOnly] public float jumpBufferCounter;

        [ReadOnly] public float jumpHoldCounter;

        [ReadOnly] public Vector2 lastGroundedPosition;

        private PlatformerPlayerController _controller;

        IPlayerController IPlayerControllerComponent.PlayerController
        {
            get => _controller;
            set => _controller = (PlatformerPlayerController)value;
        }

        public void Init(IPlayerController controller)
        {
            _controller = (PlatformerPlayerController)controller;

            _controller.OnRespawn += OnRespawn;
            _controller.OnDespawn += OnDespawn;

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
            _controller.OnRespawn -= OnRespawn;
            _controller.OnDespawn -= OnDespawn;

            UnRegisterJumpSubsystem();
            UnRegisterMovementSubsystem();
            UnRegisterGroundCheckSubsystem();
        }

        public void OnSpawn(IRespawnable respawnable)
        {
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
            if (subsystem == null) return;

            MovementSubsystem = subsystem;
            MovementSubsystem.Initalize(_controller);
        }

        public void RegisterJumpSubsystem(JumpSubsystem subsystem)
        {
            if (subsystem is null) return;

            JumpSubsystem = subsystem;
            JumpSubsystem.Initalize(_controller);
            _controller.PlatformerInputActions.Player.Jump.performed += JumpSubsystem.HandleJump;
            _controller.PlatformerInputActions.Player.Jump.canceled += JumpSubsystem.HandleJump;
        }

        public void RegisterGroundCheckSubsystem(GroundCheckSubsystem subsystem)
        {
            if (subsystem == null) return;

            GroundCheckSubsystem = subsystem;
            GroundCheckSubsystem.Initalize(_controller);
        }

        public void UnRegisterMovementSubsystem()
        {
            if (GroundCheckSubsystem is null) return;
            GroundCheckSubsystem.Dispose();
            GroundCheckSubsystem = null;
        }

        public void UnRegisterJumpSubsystem()
        {
            if (JumpSubsystem is null) return;
            _controller.PlatformerInputActions.Player.Jump.performed -= JumpSubsystem.HandleJump;
            _controller.PlatformerInputActions.Player.Jump.canceled -= JumpSubsystem.HandleJump;
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