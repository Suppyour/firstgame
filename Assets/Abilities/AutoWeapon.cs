using UnityEngine;
using System.Collections;
using Characters;

namespace Abilities
{
    public class AutoWeapon : MonoBehaviour
    {
        [SerializeField] protected WeaponData data;
        [SerializeField] protected float attackRange = 5f;
        [SerializeField] private LayerMask enemyLayerMask = 1 << 8; // ← ДОБАВЬТЕ ЭТО

        protected Transform player;

        private void Start()
        {
            player = transform;
            StartCoroutine(AttackRoutine());
        }

        protected virtual IEnumerator AttackRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(data.attackCooldown);
                Attack();
            }
        }

        protected virtual void Attack()
        {
            // ИСПОЛЬЗУЕМ enemyLayerMask вместо жесткого кода
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayerMask);

            Debug.Log($"Found {enemies.Length} enemies in range"); // ← ДЛЯ ОТЛАДКИ

            foreach (var enemy in enemies)
            {
                var enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(data.damage);
                    Debug.Log($"Dealt {data.damage} damage to enemy"); // ← ДЛЯ ОТЛАДКИ
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}