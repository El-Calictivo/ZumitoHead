using System;
using DG.Tweening;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZumoHead.Platforming
{
    public sealed class PlatformerController : MonoBehaviour
    {
        [SerializeField] public References references;

        [Space(10)]
        [Header("Grounded")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private Vector3 groundCheckOffset = new(0, 0.7f, 0);

        [SerializeField] private float groundCheckRadius = 0.15f;

        [Space(10)]
        [Header("Jump")]
        [SerializeField] private float jumpForceBuffer = 0.3f;

        [SerializeField] private float jumpBuffer = 0.3f;

        [SerializeField] private float coyoteBuffer = 0.3f;


        private bool _isJumpButtonPressed;
        private bool _isGrounded;
        private bool _justLanded;
        private float _jumpForceBufferCounter;
        private float _jumpBufferCounter;
        private float _coyoteBufferCounter;

        private InputSystemActions _inputSystemActions;

        private void Awake()
        {
            _inputSystemActions = new InputSystemActions();
            _inputSystemActions.Enable();
        }

        private void Update()
        {
            JumpUpdate();
        }

        private void JumpUpdate()
        {
            _jumpBufferCounter -= Time.deltaTime;
            _jumpBufferCounter = Mathf.Clamp(_jumpBufferCounter, 0, jumpBuffer);

            _coyoteBufferCounter -= Time.deltaTime;
            _coyoteBufferCounter = Mathf.Clamp(_coyoteBufferCounter, 0, coyoteBuffer);

            if (_isJumpButtonPressed && _isGrounded)
            {
                _jumpForceBufferCounter += Time.deltaTime;
                _jumpForceBufferCounter = Mathf.Clamp(_jumpForceBufferCounter, 0, jumpForceBuffer);

                if (Mathf.Approximately(_jumpForceBufferCounter, jumpForceBuffer))
                {
                    references.rigidbody2D.AddForceY(references.platformerAttributes.jumpForce.Evaluate(_jumpForceBufferCounter), ForceMode2D.Impulse);
                    _coyoteBufferCounter = 0;
                    _jumpBufferCounter = 0;
                    _isJumpButtonPressed = false;
                    return;
                }
            }

            if (_isGrounded || _coyoteBufferCounter > 0)
            {
                if (_jumpBufferCounter > 0 && references.rigidbody2D.linearVelocityY <= 0)
                {
                    references.rigidbody2D.AddForceY(references.platformerAttributes.jumpForce.Evaluate(1), ForceMode2D.Impulse);
                    _coyoteBufferCounter = 0;
                    _jumpBufferCounter = 0;
                    _isJumpButtonPressed = false;
                }
            }
        }


        private void FixedUpdate()
        {
            GroundCheck();
            HandleMovement();
        }

        private void OnEnable()
        {
            _inputSystemActions.Player.Jump.performed += HandleJump;
            _inputSystemActions.Player.Jump.canceled += HandleJump;
        }

        private void OnDisable()
        {
            _inputSystemActions.Player.Jump.performed -= HandleJump;
            _inputSystemActions.Player.Jump.canceled -= HandleJump;
        }

        private void OnDestroy()
        {
            _inputSystemActions.Disable();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }

        private void GroundCheck()
        {
            var groundCheck = Physics2D.OverlapCircle(transform.position + groundCheckOffset, groundCheckRadius, groundLayer);
            float velocityY = references.rigidbody2D.linearVelocity.y;

            _justLanded = !_isGrounded && groundCheck;
            _isGrounded = groundCheck && references.rigidbody2D.linearVelocityY <= 0;

            if (_isGrounded)
            {
                _coyoteBufferCounter = coyoteBuffer;
            }

            if (_justLanded && references.rigidbody2D.linearVelocity.y < 0)
            {
                references.spriteRenderer.transform.DOShakeScale(0.15f, Mathf.Abs(references.rigidbody2D.linearVelocityY * 0.1f), 24)
                    .SetEase(Ease.InOutBounce);
            }
        }

        private void HandleMovement()
        {
            var movementAxis = _inputSystemActions.Player.Move.ReadValue<Vector2>();

            references.spriteRenderer.flipX = movementAxis.x switch
            {
                < 0 => true,
                > 0 => false,
                _ => references.spriteRenderer.flipX
            };

            if (Mathf.Abs(references.rigidbody2D.linearVelocityX) < references.platformerAttributes.maxSpeed)
            {
                references.rigidbody2D.AddForceX(movementAxis.x * references.platformerAttributes.movementSpeed);
            }
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isJumpButtonPressed = true;
                _jumpForceBufferCounter = 0;
            }
            else if (context.canceled && _jumpForceBufferCounter < jumpForceBuffer)
            {
                _jumpBufferCounter = jumpBuffer;
                _isJumpButtonPressed = false;
            }
        }


        [Serializable]
        public struct References
        {
            public SpriteRenderer spriteRenderer;
            public PlatformerAttributes platformerAttributes;
            public Rigidbody2D rigidbody2D;
        }
    }
}