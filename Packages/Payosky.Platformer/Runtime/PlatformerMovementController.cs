using System;
using Payosky.CoreMechanics.PlayerController;
using Payosky.CoreMechanics.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [Serializable]
    public sealed class PlatformerMovementController : IPlayerMovementController
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

        public void Init(IPlayerController controller)
        {
            PlayerController = controller;
            PlayerController.MovementController = this;

            RegisterJumpSubsystem(JumpSubsystem);
            RegisterMovementSubsystem(MovementSubsystem);
            RegisterGroundCheckSubsystem(GroundCheckSubsystem);
        }

        public void Dispose()
        {
            UnRegisterJumpSubsystem();
            UnRegisterMovementSubsystem();
            UnRegisterGroundCheckSubsystem();
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

            // platformerPlayerController.PlatformerInputActions.Player.Jump.performed += JumpSubsystem.HandleJump;
            // platformerPlayerController.PlatformerInputActions.Player.Jump.canceled += JumpSubsystem.HandleJump;
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

            // platformerPlayerController.PlatformerInputActions.Player.Jump.performed -= JumpSubsystem.HandleJump;
            // platformerPlayerController.PlatformerInputActions.Player.Jump.canceled -= JumpSubsystem.HandleJump;
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