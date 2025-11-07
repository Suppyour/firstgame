using UnityEngine;
using Characters;

namespace Core
{
    public enum EnemyType
    {
        Normal,
        Fast,
        Heavy,
        Elite
    }

    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private GameObject normalEnemyPrefab;
        [SerializeField] private GameObject fastEnemyPrefab;
        [SerializeField] private GameObject heavyEnemyPrefab;

        public GameObject CreateEnemy(EnemyType type, Vector3 position)
        {
            GameObject prefab = GetPrefabByType(type);

            if (prefab != null)
            {
                return Instantiate(prefab, position, Quaternion.identity);
            }

            return null;
        }

        private GameObject GetPrefabByType(EnemyType type)
        {
            return type switch
            {
                EnemyType.Normal => normalEnemyPrefab,
                EnemyType.Fast => fastEnemyPrefab,
                EnemyType.Heavy => heavyEnemyPrefab,
                
                _ => normalEnemyPrefab
            };
        }
    }
}