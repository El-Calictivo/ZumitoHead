using System;
using System.Collections.Generic;
using Payosky.CoreMechanics.Damage;
using Payosky.Platformer;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZumitoGame.PowerUps
{
    [Serializable]
    public class GroundPoundJumpSubsystem : JumpSubsystem
    {
        private PlatformerPlayerController _platformerPlayerController;

        private PlatformerMovementController MovementController => _platformerPlayerController.MovementController;

        [Space(5)]
        [Header("GroundPond config")]
        [SerializeField] private Vector2 groundPoundOffset = new(0, -0.5f);

        [SerializeField] private float groundPoundRadius = 0.5f;

        [SerializeField] private float groundPoundForce = 100f;

        [SerializeField] [Range(0f, 1f)] private float groundPoundDamageThreshold = 0.7f;

        [SerializeField] private LayerMask damageableLayerMask = default;

        [Space(5)]
        [Header("Config")]
        [SerializeField] private bool drawGizmos = false;

        private bool _doDamage;

        public override void Initalize(IPlayerController playerController)
        {
            base.Initalize(playerController);
            _platformerPlayerController = playerController as PlatformerPlayerController;
            if (_platformerPlayerController != null) _platformerPlayerController.OnEditorSelected += OnDrawGizmos;
        }

        public override void Dispose()
        {
            _platformerPlayerController.OnEditorSelected -= OnDrawGizmos;
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (MovementController.isGrounded)
                {
                    MovementController.jumpHoldCounter = 0;
                    _platformerPlayerController.Rigidbody2D.linearVelocityY = 0;
                    MovementController.isJumpCharging = true;
                }
                else
                {
                    _doDamage = MovementController.jumpHoldCounter / MaxJumpHoldTime >= groundPoundDamageThreshold;
                    if (_doDamage)
                    {
                        _platformerPlayerController.Rigidbody2D.AddForceY(-Mathf.Abs(groundPoundForce));
                    }
                }
            }
            else if (context.canceled)
            {
                MovementController.isJumpCharging = false;
            }
        }

        /// <summary>
        ///     Handles the mechanics necessary for initiating, maintaining, and regulating the jump behavior of a platformer
        ///     entity.
        ///     Updates the jump-related counters including jump buffer and coyote time, evaluates jump inputs and constraints,
        ///     and applies corresponding forces to facilitate a responsive jumping system.
        /// </summary>
        public override void Update()
        {
            if (_platformerPlayerController is null) return;

            if (MovementController.isJumpCharging)
            {
                MovementController.jumpHoldCounter += Time.fixedDeltaTime;
                MovementController.jumpHoldCounter = Mathf.Clamp(MovementController.jumpHoldCounter, 0, MaxJumpHoldTime);

                _platformerPlayerController.Rigidbody2D.AddForceY(JumpForce * (1 - MovementController.jumpHoldCounter / MaxJumpHoldTime));

                if (Mathf.Approximately(MovementController.jumpHoldCounter, MaxJumpHoldTime))
                {
                    MovementController.jumpHoldCounter = MaxJumpHoldTime;
                    MovementController.isJumpCharging = false;
                }
            }

            if (MovementController.justLanded)
            {
                HandleOnLanded();
            }
        }

        public void HandleOnLanded()
        {
            MovementController.isJumpCharging = false;
            _platformerPlayerController.MovementController.jumpHoldCounter = 0;

            if (_doDamage)
            {
                DoGroundPoundDamage();
                _doDamage = false;
            }
        }

        private void DoGroundPoundDamage()
        {
            var center = (Vector2)_platformerPlayerController.transform.position + groundPoundOffset;

            var results = new List<Collider2D>();
            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = damageableLayerMask,
                useTriggers = true
            };

            Physics2D.OverlapCircle(center, groundPoundRadius, filter, results);

            HashSet<Rigidbody2D> processedBodies = new();

            foreach (var col in results)
            {
                var rb = col.attachedRigidbody;
                if (!rb || !processedBodies.Add(rb)) continue;

                if (rb.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.DealDamage(1);
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = _doDamage ? Color.green : Color.red;
            if (_doDamage)
            {
                Gizmos.DrawSphere((Vector2)_platformerPlayerController.transform.position + groundPoundOffset, groundPoundRadius);
            }
            else
            {
                Gizmos.DrawWireSphere((Vector2)_platformerPlayerController.transform.position + groundPoundOffset, groundPoundRadius);
            }
        }
    }
}