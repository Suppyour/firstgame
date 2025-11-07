using UnityEngine;
using Core;

namespace Characters
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] protected EnemyData data;
        [SerializeField] protected int experienceValue = 10;

        protected Transform player;
        protected Rigidbody2D rb;
        protected int currentHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // Не ищем игрока здесь - Awake вызывается при создании префаба
        }

        private void Start()
        {
            // Ищем игрока при спауне каждого врага
            FindPlayer();

            if (data != null)
            {
                currentHealth = data.maxHealth;
                Debug.Log($"Enemy spawned with {currentHealth} HP");
            }
            else
            {
                Debug.LogError("EnemyData is not assigned!");
                currentHealth = 10;
            }
        }

        private void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log($"Player found at position: {player.position}");
            }
            else
            {
                Debug.LogError("Player not found! Enemy will not move.");

                // Альтернативный поиск по компоненту
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    player = playerController.transform;
                    Debug.Log("Player found via PlayerController component");
                }
            }
        }

        private void Update()
        {
            MoveTowardsPlayer();
        }

        protected virtual void MoveTowardsPlayer()
        {
            if (player == null)
            {
                // Пытаемся найти игрока снова каждые 60 кадров (≈1 секунда)
                if (Time.frameCount % 60 == 0)
                {
                    FindPlayer();
                }
                return;
            }

            // Проверяем что игрок все еще существует
            if (player == null || player.gameObject == null)
            {
                player = null;
                return;
            }

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * data.moveSpeed;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            currentHealth -= damage;
            Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // ПЕРЕДАВАЕМ ЧИСЛО (опыт), А НЕ ССЫЛКУ НА ОБЪЕКТ!
            int expToGive = data != null ? data.experienceValue : 10;

            Debug.Log($"Enemy died! Giving {expToGive} experience");

            // ВАЖНО: передаем ЧИСЛО, а не 'this'
            EventManager.Instance?.TriggerEvent(GameEventType.ExperienceGained, expToGive);
            EventManager.Instance?.TriggerEvent(GameEventType.EnemyDied, this); // это отдельное событие

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var playerStats = collision.gameObject.GetComponent<PlayerStats>();
                if (playerStats != null && data != null)
                {
                    playerStats.TakeDamage(data.damage);
                    Debug.Log($"Enemy dealt {data.damage} damage to player");
                }
            }
        }

        // Визуализация для отладки в редакторе
        private void OnDrawGizmosSelected()
        {
            if (player != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.position);

                // Показываем направление движения
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * 1f);
            }
        }

        // Защита от уничтожения при выходе за пределы
        private void OnBecameInvisible()
        {
            // Можно добавить логику уничтожения врагов за камерой
            // Destroy(gameObject);
        }
    }
}