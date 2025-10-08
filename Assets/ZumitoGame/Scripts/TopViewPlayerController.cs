using UnityEngine;
using ZumitoGame.InputActions;

namespace ZumitoGame
{
    public class TopViewPlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;

        [SerializeField] private new Rigidbody2D rigidbody;

        [SerializeField] private SpriteRenderer spriteRenderer;

        public LevelSelectionInputActions LevelSelectionInputActions { private set; get; }

        private void Awake()
        {
            LevelSelectionInputActions = new LevelSelectionInputActions();
        }

        private void OnEnable()
        {
            LevelSelectionInputActions.Enable();
        }

        private void FixedUpdate()
        {
            var movementAxis = LevelSelectionInputActions.Player.Move.ReadValue<Vector2>();
            rigidbody.linearVelocity = movementAxis * speed;

            if (movementAxis != Vector2.zero)
            {
                spriteRenderer.flipX = movementAxis.x switch
                {
                    < 0 => true,
                    > 0 => false,
                    _ => spriteRenderer.flipX
                };
            }
        }

        private void OnDisable()
        {
            LevelSelectionInputActions.Disable();
        }
    }
}