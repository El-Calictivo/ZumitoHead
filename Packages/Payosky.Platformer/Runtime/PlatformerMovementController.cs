using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Payosky.CoreMechanics.Runtime;
using Payosky.PlayerController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Payosky.Platformer
{
    [RequireComponent(typeof(PlatformerPlayerController))]
    public sealed class PlatformerMovementController : MonoBehaviour, IPlayerMovementController
    {
        [field: SerializeField] private PlatformerPlayerController playerController;

        [field: Space(5)]
        [field: SerializeField] public PlatformerAttributes PlatformerAttributes { private set; get; }

        [Space(5)]
        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private Vector3 groundCheckOffset = new(0, 0.7f, 0);

        [SerializeField] private float groundCheckRadius = 0.15f;

        [SerializeField] private float horizontalRespawnMargin = 1.5f;

        [Space(5)]
        [Header("Roof Check")]
        [SerializeField] private Vector3 roofCheckOffset = new(0, 0.7f, 0);

        [FormerlySerializedAs("roofCheckRadious")] [SerializeField]
        private float roofCheckRadius = 0.15f;

        private Color _groundCheckColor = Color.cyan;

        private Color _roofCheckColor = Color.cyan;

        private float _coyoteTimeCounter;

        private bool _doHoldJump;

        private bool _isGrounded;

        private bool _isJumpCharging;

        private float _jumpBufferCounter;

        private float _jumpHoldCounter;

        private Vector3 _visualsOriginaScale;

        private Vector2 _lastGroundedPosition;

        private void OnValidate()
        {
            if (!playerController) TryGetComponent(out playerController);
        }

        private void Awake()
        {
            _visualsOriginaScale = playerController.SpriteRenderer.transform.localScale;
        }

        private void OnEnable()
        {
            playerController.OnRespawn += OnPlayerRespawned;
            playerController.OnDespawn += OnPlayerDespawned;
            playerController.PlatformerInputActions.Player.Jump.performed += HandleJump;
            playerController.PlatformerInputActions.Player.Jump.canceled += HandleJump;
        }

        private void OnDisable()
        {
            playerController.OnRespawn -= OnPlayerRespawned;
            playerController.OnDespawn -= OnPlayerDespawned;
            playerController.PlatformerInputActions.Player.Jump.performed -= HandleJump;
            playerController.PlatformerInputActions.Player.Jump.canceled -= HandleJump;
        }

        private void Update()
        {
            JumpUpdate();
        }

        private void FixedUpdate()
        {
            GroundCheck();
            HandleMovement();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            playerController.SpriteRenderer.transform.DOShakeScale(0.1f, other.relativeVelocity * 0.1f, 24).SetRelative()
                .SetEase(Ease.InOutBounce)
                .OnComplete(() => playerController.SpriteRenderer.transform.localScale = _visualsOriginaScale);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _groundCheckColor;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
            Gizmos.color = _roofCheckColor;
            Gizmos.DrawWireSphere(transform.position + roofCheckOffset, roofCheckRadius);
        }

        private void JumpUpdate()
        {
            _jumpBufferCounter = Mathf.Clamp(_jumpBufferCounter - Time.deltaTime, 0, PlatformerAttributes.jumpBufferTime);
            _coyoteTimeCounter = Mathf.Clamp(_coyoteTimeCounter - Time.deltaTime, 0, PlatformerAttributes.coyoteTime);

            if (_isGrounded && _isJumpCharging)
            {
                _jumpHoldCounter += Time.deltaTime;
                _jumpHoldCounter = Mathf.Clamp(_jumpHoldCounter, 0, PlatformerAttributes.maxJumpHoldTime);

                if (Mathf.Approximately(_jumpHoldCounter, PlatformerAttributes.maxJumpHoldTime))
                {
                    MarkBufferReadyToJump();
                }
            }

            if (_isGrounded || _coyoteTimeCounter > 0)
            {
                if (_jumpBufferCounter > 0)
                {
                    var jumpForce = PlatformerAttributes.jumpForce * (_doHoldJump ? PlatformerAttributes.holdJumpCurve.Evaluate(_jumpHoldCounter) : 1);
                    jumpForce -= playerController.Rigidbody2D.linearVelocityY;

                    playerController.Rigidbody2D.AddForceY(jumpForce, ForceMode2D.Impulse);
                    _doHoldJump = false;
                    _coyoteTimeCounter = 0;
                    _jumpBufferCounter = 0;
                    _jumpHoldCounter = 0;
                }
            }
        }

        private void MarkBufferReadyToJump(bool useHoldModifier = true)
        {
            _jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
            _doHoldJump = useHoldModifier;
            _isJumpCharging = false;
        }

        private void GroundCheck()
        {
            var groundCheck = Physics2D.OverlapCircle(transform.position + groundCheckOffset, groundCheckRadius, groundLayer);
            var roofCheck = Physics2D.OverlapCircle(transform.position + roofCheckOffset, roofCheckRadius, groundLayer);

            _isGrounded = groundCheck && groundCheck != roofCheck && playerController.Rigidbody2D.linearVelocityY <= 0;

            if (_isGrounded)
            {
                _coyoteTimeCounter = PlatformerAttributes.coyoteTime;

                _lastGroundedPosition = transform.position;
                switch (playerController.Rigidbody2D.linearVelocity.x)
                {
                    case < 0:
                        _lastGroundedPosition.x += horizontalRespawnMargin;
                        break;
                    case > 0:
                        _lastGroundedPosition.x -= horizontalRespawnMargin;
                        break;
                }
            }
            else if (_isJumpCharging)
            {
                MarkBufferReadyToJump(false);
            }

            _groundCheckColor = _isGrounded ? Color.green : Color.red;
            _roofCheckColor = roofCheck ? Color.green : Color.red;
        }

        private void HandleMovement()
        {
            var movementAxis = playerController.PlatformerInputActions.Player.Move.ReadValue<Vector2>();

            playerController.SpriteRenderer.flipX = movementAxis.x switch
            {
                < 0 => true,
                > 0 => false,
                _ => playerController.SpriteRenderer.flipX
            };

            if (Mathf.Abs(playerController.Rigidbody2D.linearVelocityX) < PlatformerAttributes.maxSpeed * (_isGrounded ? 1 : PlatformerAttributes.movementApexJumpModifier.Evaluate(playerController.Rigidbody2D.linearVelocityY)))
            {
                playerController.Rigidbody2D.AddForceX(movementAxis.x * PlatformerAttributes.movementSpeed);
            }
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_isGrounded)
                {
                    _isJumpCharging = true;
                }
                else
                {
                    _jumpBufferCounter = PlatformerAttributes.jumpBufferTime;
                }
            }
            else if (context.canceled)
            {
                if (_isGrounded && _isJumpCharging && _jumpHoldCounter < PlatformerAttributes.maxJumpHoldTime)
                {
                    MarkBufferReadyToJump();
                }
            }
        }

        public void OnPlayerDespawned(IRespawnable player)
        {
            playerController.SpriteRenderer.enabled = false;
            playerController.Rigidbody2D.bodyType = RigidbodyType2D.Static;
        }

        public void OnPlayerRespawned(IRespawnable respawned)
        {
            playerController.SpriteRenderer.enabled = true;
            transform.position = _lastGroundedPosition;
            playerController.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            playerController.Rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}