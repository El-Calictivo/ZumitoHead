using System;
using Payosky.PlayerController.Runtime;
using UnityEngine;

namespace Payosky.Platformer
{
    [Serializable]
    public class PlatformerDefaultGroundedSubsystem : GroundCheckSubsystem
    {
        private PlatformerPlayerController _platformerPlayerController;

        private PlatformerMovementController _movementController => _platformerPlayerController.MovementController;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private Vector3 groundCheckOffset = new(0, 0.7f, 0);

        [SerializeField] private float groundCheckRadius = 0.15f;

        [SerializeField] private float horizontalRespawnMargin = 1.5f;

        [Space(5)]
        [Header("Roof Check")]
        [SerializeField] private Vector3 roofCheckOffset = new(0, 0.7f, 0);

        [SerializeField] private float roofCheckRadius = 0.15f;

        private Color _groundCheckColor;

        private Color _roofCheckColor;

        public override void Initalize(IPlayerController playerController)
        {
            base.Initalize(playerController);
            _platformerPlayerController = playerController as PlatformerPlayerController;
        }

        /// Updates the grounded and roof-check states for the platformer player controller by determining if the player is
        /// grounded or colliding with a roof. This method also manages coyote time, jump buffering, and jump charging states.
        /// During the update:
        /// - A ground check is performed using a 2D circle cast based on configured offset and radius.
        /// - A roof check is also performed using a 2D circle cast at a separate offset and radius.
        /// - If the player is grounded:
        /// - The coyote time counter is reset.
        /// - The last grounded position is updated based on the player's horizontal velocity, considering a horizontal respawn margin.
        /// - If the player is not grounded but is jump charging:
        /// - Jump buffering is activated.
        /// - Charging states are reset.
        /// - Ground and roof states are visually represented through debug gizmo colors.
        /// This method is called once per frame, typically within the MonoBehaviour's Update() method.
        public override void Update()
        {
            if (_platformerPlayerController is null) return;

            var groundCheck = Physics2D.OverlapCircle(_platformerPlayerController.transform.position + groundCheckOffset, groundCheckRadius, groundLayer);
            var roofCheck = Physics2D.OverlapCircle(_platformerPlayerController.transform.position + roofCheckOffset, roofCheckRadius, groundLayer);

            var wasGrounded = _movementController.isGrounded;
            _movementController.isGrounded = groundCheck && groundCheck != roofCheck && _platformerPlayerController.Rigidbody2D.linearVelocityY <= 0;

            if (!wasGrounded && _movementController.isGrounded)
            {
                _movementController.justLanded = true;
            }

            if (_movementController.isGrounded)
            {
                _movementController.coyoteTimeCounter = _movementController.JumpSubsystem.CoyoteTime;

                _movementController.lastGroundedPosition = _platformerPlayerController.transform.position;
                switch (_platformerPlayerController.Rigidbody2D.linearVelocity.x)
                {
                    case < 0:
                        _movementController.lastGroundedPosition.x += horizontalRespawnMargin;
                        break;
                    case > 0:
                        _movementController.lastGroundedPosition.x -= horizontalRespawnMargin;
                        break;
                }
            }

            _groundCheckColor = _movementController.isGrounded ? Color.green : Color.red;
            _roofCheckColor = roofCheck ? Color.green : Color.red;
        }
    }
}