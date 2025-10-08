using System;
using System.Collections.Generic;
using Payosky.CoreMechanics.Damage;
using Payosky.CoreMechanics.PlayerController;
using Payosky.Platformer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZumitoGame.PowerUps
{
    [Serializable]
    public class GroundPoundJumpSubsystem : JumpSubsystem
    {
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

        public override void Initialize(IPlayerController playerController)
        {
            base.Initialize(playerController);
            playerController.OnEditorSelected += OnDrawGizmos;
        }

        public override void Dispose()
        {
            PlayerController.OnEditorSelected -= OnDrawGizmos;
        }

        public override void HandleJump(InputAction.CallbackContext context)
        {
            if (PlayerController is not PlatformerPlayerController platformerPlayerController || PlayerController.MovementController.MovementData is not PlatformerMovementData platformerMovementData) return;
            if (context.performed)
            {
                if (platformerMovementData.IsGrounded)
                {
                    platformerMovementData.JumpHoldCounter = 0;
                    platformerPlayerController.Rigidbody2D.linearVelocityY = 0;
                    platformerMovementData.IsJumpCharging = true;
                }
                else
                {
                    _doDamage = platformerMovementData.JumpHoldCounter / MaxJumpHoldTime >= groundPoundDamageThreshold;
                    if (_doDamage)
                    {
                        platformerPlayerController.Rigidbody2D.AddForceY(-Mathf.Abs(groundPoundForce));
                    }
                }
            }
            else if (context.canceled)
            {
                platformerMovementData.IsJumpCharging = false;
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
            if (PlayerController is not PlatformerPlayerController platformerPlayerController || PlayerController.MovementController.MovementData is not PlatformerMovementData platformerMovementData) return;

            if (platformerMovementData.IsJumpCharging)
            {
                platformerMovementData.JumpHoldCounter += Time.fixedDeltaTime;
                platformerMovementData.JumpHoldCounter = Mathf.Clamp(platformerMovementData.JumpHoldCounter, 0, MaxJumpHoldTime);

                platformerPlayerController.Rigidbody2D.AddForceY(JumpForce * (1 - platformerMovementData.JumpHoldCounter / MaxJumpHoldTime));

                if (Mathf.Approximately(platformerMovementData.JumpHoldCounter, MaxJumpHoldTime))
                {
                    platformerMovementData.JumpHoldCounter = MaxJumpHoldTime;
                    platformerMovementData.IsJumpCharging = false;
                }
            }

            if (platformerMovementData.JustLanded)
            {
                HandleOnLanded();
            }
        }

        public void HandleOnLanded()
        {
            if (PlayerController.MovementController.MovementData is not PlatformerMovementData platformerMovementData) return;
            platformerMovementData.IsJumpCharging = false;
            platformerMovementData.JumpHoldCounter = 0;

            if (_doDamage)
            {
                DoGroundPoundDamage();
                _doDamage = false;
            }
        }

        private void DoGroundPoundDamage()
        {
            var center = (Vector2)PlayerController.GameObject.transform.position + groundPoundOffset;

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
                Gizmos.DrawSphere((Vector2)PlayerController.GameObject.transform.position + groundPoundOffset, groundPoundRadius);
            }
            else
            {
                Gizmos.DrawWireSphere((Vector2)PlayerController.GameObject.transform.position + groundPoundOffset, groundPoundRadius);
            }
        }
    }
}