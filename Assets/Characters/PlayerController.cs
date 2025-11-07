using UnityEngine;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private PlayerStats stats;

        private Rigidbody2D rb;
        private Vector2 movement;

        private void Awake()
        {
            Instance = this;
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement = Vector2.ClampMagnitude(movement, 1f);
        }

        private void Move()
        {
            if (stats != null)
                rb.linearVelocity = movement * (moveSpeed * stats.MoveSpeedMultiplier);
            else
                rb.linearVelocity = movement * moveSpeed;
        }

    }
}