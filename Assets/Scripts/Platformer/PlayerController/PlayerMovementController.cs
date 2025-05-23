using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace ZumoHead.Platforming
{
    /// <summary>
    /// Controls the movement behavior of the player character within the game.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing the player's movement mechanics,
    /// including interacting with input systems, applying physics, and ensuring
    /// proper handling of player movement within platforming environments.
    /// </remarks>
    /// <example>
    /// This class is designed to be attached as a component to the player character GameObject in Unity.
    /// All functionalities related to player movement should be handled within this component.
    /// </example>
    public sealed class PlayerMovementController : MonoBehaviour, IRespawnable
    {
        /// <summary>
        /// A reference to the SpriteRenderer component used to handle the visual appearance and transformations
        /// of the player character, including sprite flipping, scaling, and dynamic effects during gameplay.
        /// </summary>
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Configuration data for player movement attributes, defining key gameplay parameters such as movement speed,
        /// jump behavior, and related physics modifiers. This variable references a <c>PlatformerAttributes</c> ScriptableObject
        /// that holds the configurable settings for player movement.
        /// </summary>
        [SerializeField] private PlatformerAttributes movementConfig;

        /// <summary>
        /// Represents the `Rigidbody2D` component associated with the `PlayerMovementController`.
        /// Used to manage the player's physics-based behaviors such as movement, jumping, and interactions with forces.
        /// This variable is responsible for applying forces, checking velocities, and managing physical state during gameplay.
        /// </summary>
        [SerializeField] private new Rigidbody2D rigidbody2D;

        /// <summary>
        /// The layer mask used to define which layers are considered as "ground" in ground checks.
        /// </summary>
        /// <remarks>
        /// This variable is used in conjunction with physics operations to determine if the player character
        /// is standing on or interacting with the ground. It is primarily utilized by the ground checking logic
        /// to enable functionalities such as coyote time and proper jump handling.
        /// </remarks>
        [Space(5)]
        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;

        /// <summary>
        /// Specifies the offset position used for ground detection checks relative to the player's transform position.
        /// This offset ensures that the ground check is performed at the appropriate location around the player.
        /// Used alongside <see cref="groundCheckRadius"/> and <see cref="groundLayer"/> to determine if the player is grounded.
        /// </summary>
        [SerializeField] private Vector3 groundCheckOffset = new(0, 0.7f, 0);

        /// <summary>
        /// Represents the radius of the circular area used to check for ground collision in the ground check mechanism.
        /// This value determines the size of the detection sphere for detecting if the player is grounded.
        /// </summary>
        [SerializeField] private float groundCheckRadius = 0.15f;

        /// <summary>
        /// Represents the offset position from the player's transform used to perform roof collision detection.
        /// This vector defines the location where the system will evaluate potential obstructions above the player,
        /// ensuring accurate roof detection during gameplay.
        /// </summary>
        [Space(5)]
        [Header("Roof Check")]
        [SerializeField] private Vector3 roofCheckOffset = new(0, 0.7f, 0);

        /// <summary>
        /// The radius used to determine the size of the area for collision detection when checking for obstacles above the player (roof).
        /// This value is utilized in roof detection to determine if there is any object or ceiling directly above the player.
        /// </summary>
        [FormerlySerializedAs("roofCheckRadious")] [SerializeField]
        private float roofCheckRadius = 0.15f;

        /// <summary>
        /// Private field representing an instance of the InputSystemActions class.
        /// This variable is used to manage player input actions for movement and interactions
        /// within the PlayerMovementController class. It initializes, enables,
        /// and binds specific input actions such as movement and jumping.
        /// </summary>
        private InputSystemActions _inputSystemActions;

        /// <summary>
        /// Represents the color used to visually indicate the ground check status in the Unity Editor.
        /// This color is updated dynamically to reflect whether the player character is grounded.
        /// </summary>
        private Color _groundCheckColor = Color.cyan;

        /// <summary>
        /// A private field used to set the color of the visual indicator for roof collision checks during debugging.
        /// This color changes dynamically based on whether the roof is detected or not:
        /// - Green: Roof detected.
        /// - Red: No roof detected.
        /// </summary>
        private Color _roofCheckColor = Color.cyan;

        /// <summary>
        /// Tracks the remaining coyote time window during which the player can perform a jump even after leaving the ground.
        /// Coyote time provides a brief grace period to improve responsiveness for players.
        /// </summary>
        /// <remarks>
        /// This value is decremented over time and reset when the player is grounded.
        /// It is clamped between 0 and the `coyoteTime` value defined in the `PlatformerAttributes` configuration.
        /// </remarks>
        private float _coyoteTimeCounter;

        /// <summary>
        /// Indicates whether the player should apply the "hold jump" mechanic during the next jump action.
        /// When set to true, the jump force is influenced by the hold duration, utilizing the jump hold curve.
        /// This variable is controlled internally and reset after each jump to prevent unintended behavior.
        /// </summary>
        private bool _doHoldJump;

        /// <summary>
        /// Indicates whether the player character is currently on the ground.
        /// This boolean state is updated based on collision detection with the ground layer,
        /// and is essential for managing grounded-related mechanics like jumping and coyote time.
        /// </summary>
        private bool _isGrounded;

        /// <summary>
        /// Indicates whether the player is currently charging a jump.
        /// This state is triggered when the jump action is initiated and continues as long as
        /// the jump is being held, until the maximum jump hold time is reached or the jump is released.
        /// </summary>
        private bool _isJumpCharging;

        /// <summary>
        /// Tracks the remaining time within the jump buffer window, allowing the player
        /// input to "queue" a jump if the jump button is pressed shortly before landing
        /// or while grounded.
        /// </summary>
        /// <remarks>
        /// This counter is clamped between 0 and the maximum buffer time defined in
        /// <see cref="PlatformerAttributes.jumpBufferTime"/>. It decrements based on
        /// <see cref="Time.deltaTime"/> and is reset when specific jump conditions are met.
        /// </remarks>
        private float _jumpBufferCounter;

        /// <summary>
        /// Tracks the amount of time the jump button has been held down during a jump charging state.
        /// This variable is incremented while the player is grounded and currently charging a jump.
        /// It is clamped to a maximum value defined in the movement configuration.
        /// </summary>
        private float _jumpHoldCounter;

        /// <summary>
        /// Stores the original scale of the player visuals.
        /// </summary>
        /// <remarks>
        /// This variable holds the initial local scale of the SpriteRenderer's transform to be used
        /// as a reference for scaling transformations, such as animations or effects, and to reset
        /// it back to its original state after such effects have been completed.
        /// </remarks>
        private Vector3 _visualsOriginaScale;

        private Vector2 _lastGroundedPosition;

        /// <summary>
        /// Initializes necessary components and sets up default values
        /// for the PlayerMovementController.
        /// </summary>
        /// <remarks>
        /// This method is automatically invoked by Unity during the MonoBehaviour instantiation.
        /// - Creates and enables the InputSystemActions instance used for handling player input.
        /// - Stores the original local scale of the sprite renderer visuals for reference.
        /// </remarks>
        private void Awake()
        {
            _inputSystemActions = new InputSystemActions();
            _inputSystemActions.Enable();
            _visualsOriginaScale = spriteRenderer.transform.localScale;
        }

        /// <summary>
        /// Initializes or sets up event listeners for the PlayerMovementController when the object is enabled.
        /// </summary>
        /// <remarks>
        /// This method is automatically called when the MonoBehaviour is enabled.
        /// It subscribes to relevant input actions such as jump events.
        /// Ensure that proper cleanup is handled when disabling to prevent memory leaks or unintended behavior.
        /// </remarks>
        private void OnEnable()
        {
            _inputSystemActions.Player.Jump.performed += HandleJump;
            _inputSystemActions.Player.Jump.canceled += HandleJump;
        }

        /// The `Update` method is called once per frame in Unity's MonoBehaviour lifecycle.
        /// It serves as the primary update loop for the `PlayerMovementController` to process
        /// input and update player-related movement logic every frame.
        /// Specifically, it delegates the jump-related functionality to the `JumpUpdate` method,
        /// which handles aspects such as jump buffering, coyote time, jump force application, and
        /// input state management.
        /// This method is instrumental in ensuring smooth and responsive player controls in
        /// a platforming context.
        private void Update()
        {
            JumpUpdate();
        }

        /// <summary>
        /// Called on a fixed time interval to handle the physics-related operations for the player.
        /// This method performs necessary updates such as checking for ground collisions
        /// and managing player movement based on physics computations.
        /// </summary>
        private void FixedUpdate()
        {
            GroundCheck();
            HandleMovement();
        }

        /// <summary>
        /// Handles collision events for the GameObject this script is attached to.
        /// Applies a scaling shake effect to the sprite upon collision.
        /// </summary>
        /// <param name="other">Collision data containing information about the other collider involved in the contact.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            spriteRenderer.transform.DOShakeScale(0.1f, other.relativeVelocity * 0.1f, 24).SetRelative()
                .SetEase(Ease.InOutBounce)
                .OnComplete(() => spriteRenderer.transform.localScale = _visualsOriginaScale);
        }

        /// <summary>
        /// Called by Unity to allow the developer to draw custom gizmos in the Scene view
        /// only when the object is selected. This method is useful for visually debugging
        /// or designing specific components during the development phase.
        /// </summary>
        /// <remarks>
        /// This method in the context of the PlayerMovementController is used to render
        /// visual representations of the ground check and roof check spheres. The gizmos
        /// assist in confirming the placement and size of the respective collision check
        /// areas during editing.
        /// </remarks>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _groundCheckColor;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
            Gizmos.color = _roofCheckColor;
            Gizmos.DrawWireSphere(transform.position + roofCheckOffset, roofCheckRadius);
        }

        /// <summary>
        /// Processes and updates the player's jump mechanics, including jump buffering,
        /// coyote time counters, and jump forces based on configuration parameters.
        /// Handles the logic for charging and executing jumps while
        /// factoring in environmental conditions such as grounded state and ongoing momentum.
        /// </summary>
        /// <remarks>
        /// This method calculates jump timing mechanics, such as the coyote time (allowing jumps
        /// shortly after falling from a platform) and the jump buffer (allowing jumps
        /// triggered shortly before landing), to create a more responsive and user-friendly experience.
        /// Additionally, it accounts for jump hold behavior, enabling higher jumps
        /// by holding the jump button for a longer duration, as defined by the
        /// jump hold curve in the configuration.
        /// </remarks>
        private void JumpUpdate()
        {
            _jumpBufferCounter = Mathf.Clamp(_jumpBufferCounter - Time.deltaTime, 0, movementConfig.jumpBufferTime);
            _coyoteTimeCounter = Mathf.Clamp(_coyoteTimeCounter - Time.deltaTime, 0, movementConfig.coyoteTime);

            if (_isGrounded && _isJumpCharging)
            {
                _jumpHoldCounter += Time.deltaTime;
                _jumpHoldCounter = Mathf.Clamp(_jumpHoldCounter, 0, movementConfig.maxJumpHoldTime);

                if (Mathf.Approximately(_jumpHoldCounter, movementConfig.maxJumpHoldTime))
                {
                    MarkBufferReadyToJump();
                }
            }

            if (_isGrounded || _coyoteTimeCounter > 0)
            {
                if (_jumpBufferCounter > 0)
                {
                    var jumpForce = movementConfig.jumpForce * (_doHoldJump ? movementConfig.holdJumpCurve.Evaluate(_jumpHoldCounter) : 1);

                    jumpForce -= rigidbody2D.linearVelocityY;

                    rigidbody2D.AddForceY(jumpForce, ForceMode2D.Impulse);
                    _doHoldJump = false;
                    _coyoteTimeCounter = 0;
                    _jumpBufferCounter = 0;
                    _jumpHoldCounter = 0;
                }
            }
        }

        /// Marks the buffer ready to register a jump action.
        /// <param name="useHoldModifier">
        /// Determines whether the hold jump modifier should be applied.
        /// If true, the hold jump modifier is enabled; otherwise, it's disabled.
        /// Default value is true.
        /// </param>
        private void MarkBufferReadyToJump(bool useHoldModifier = true)
        {
            _jumpBufferCounter = movementConfig.jumpBufferTime;
            _doHoldJump = useHoldModifier;
            _isJumpCharging = false;
        }

        /// <summary>
        /// Performs a check to determine if the player character is grounded.
        /// </summary>
        /// <remarks>
        /// This method uses a 2D physics overlap circle at a specified offset and radius to check if the player is in contact
        /// with a surface defined by the ground layer mask. It also considers whether the player is touching a roof to ensure the
        /// grounded state is accurately determined.
        /// If the character is grounded, the coyote time counter is reset to the configured value in the movement configuration.
        /// If the player is charging a jump but is no longer grounded, the jump buffer is prepared without using the hold modifier.
        /// </remarks>
        private void GroundCheck()
        {
            var groundCheck = Physics2D.OverlapCircle(transform.position + groundCheckOffset, groundCheckRadius, groundLayer);
            var roofCheck = Physics2D.OverlapCircle(transform.position + roofCheckOffset, roofCheckRadius, groundLayer);

            _isGrounded = groundCheck && groundCheck != roofCheck && rigidbody2D.linearVelocityY <= 0;

            if (_isGrounded)
            {
                _coyoteTimeCounter = movementConfig.coyoteTime;
                _lastGroundedPosition = transform.position;
                _lastGroundedPosition.x -= rigidbody2D.linearVelocity.x / 2;
            }
            else if (_isJumpCharging)
            {
                MarkBufferReadyToJump(false);
            }

            _groundCheckColor = _isGrounded ? Color.green : Color.red;
            _roofCheckColor = roofCheck ? Color.green : Color.red;
        }

        /// <summary>
        /// Handles player movement input and applies horizontal motion to the Rigidbody2D component.
        /// Adjusts the player's sprite orientation based on movement direction
        /// and enforces a maximum horizontal speed constraint, which may vary depending on
        /// ground or air context.
        /// </summary>
        private void HandleMovement()
        {
            var movementAxis = _inputSystemActions.Player.Move.ReadValue<Vector2>();

            spriteRenderer.flipX = movementAxis.x switch
            {
                < 0 => true,
                > 0 => false,
                _ => spriteRenderer.flipX
            };


            if (Mathf.Abs(rigidbody2D.linearVelocityX) < movementConfig.maxSpeed * (_isGrounded ? 1 : movementConfig.movementApexJumpModifier.Evaluate(rigidbody2D.linearVelocityY)))
            {
                rigidbody2D.AddForceX(movementAxis.x * movementConfig.movementSpeed);
            }
        }

        /// <summary>
        /// Handles the player's jump input and triggers appropriate actions based on the input state
        /// (performed or canceled), while accounting for various jump mechanics such as buffering and hold time.
        /// </summary>
        /// <param name="context">The input action context that provides details about the jump input event.</param>
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
                    _jumpBufferCounter = movementConfig.jumpBufferTime;
                }
            }
            else if (context.canceled)
            {
                if (_isGrounded && _isJumpCharging && _jumpHoldCounter < movementConfig.maxJumpHoldTime)
                {
                    MarkBufferReadyToJump();
                }
            }
        }

        /// <summary>
        /// Method called when the MonoBehaviour becomes disabled or inactive.
        /// Unsubscribes actions related to the player's jump input from the input system events.
        /// This ensures that no unnecessary input processing occurs while the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            _inputSystemActions.Player.Jump.performed -= HandleJump;
            _inputSystemActions.Player.Jump.canceled -= HandleJump;
        }

        /// Cleans up resources and disables the input system actions when the component is being destroyed.
        /// This method is automatically invoked by Unity when the associated GameObject or Component is destroyed.
        private void OnDestroy()
        {
            _inputSystemActions.Disable();
        }

        public async UniTask Despawn()
        {
            _inputSystemActions.Disable();
            spriteRenderer.enabled = false;
            rigidbody2D.bodyType = RigidbodyType2D.Static;
            await UniTask.Delay(1500);
        }

        public async UniTask Respawn()
        {
            _inputSystemActions.Enable();
            spriteRenderer.enabled = true;
            rigidbody2D.linearVelocity = Vector2.zero;
            transform.position = _lastGroundedPosition;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}