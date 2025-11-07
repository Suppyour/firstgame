using UnityEngine;
using Core;
using System.Collections;

namespace Characters
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float initialSpawnRate = 2f;
        [SerializeField] private float spawnRateIncrease = 0.1f;

        private float spawnTimer;
        private float currentSpawnRate;
        private EnemyFactory enemyFactory;

        private void Start()
        {
            enemyFactory = FindFirstObjectByType<EnemyFactory>();
            currentSpawnRate = initialSpawnRate;

            // опнбепъел х янгдюел яоюбм-онимрш еякх мсфмн
            if (spawnPoints.Length == 0 || spawnPoints[0] == null)
            {
                CreateSpawnPoints();
            }

            Debug.Log($"EnemySpawner ready. SpawnPoints: {spawnPoints.Length}, Factory: {enemyFactory != null}");

            EventManager.Instance?.StartListening(GameEventType.GameStart, OnGameStart);
        }

        private void Update()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= currentSpawnRate)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            // опнбепйю мю NULL дкъ йюфднцн яоюбм-онимрю
            if (spawnPoints.Length == 0 || !HasValidSpawnPoints())
            {
                Debug.LogWarning("No valid spawn points!");
                return;
            }

            if (enemyFactory == null)
            {
                Debug.LogError("EnemyFactory not found!");
                return;
            }

            // мюундхл оепбши бюкхдмши яоюбм-онимр
            Transform spawnPoint = GetRandomValidSpawnPoint();
            if (spawnPoint == null)
            {
                Debug.LogWarning("No valid spawn point found!");
                return;
            }

            EnemyType enemyType = GetRandomEnemyType();
            var enemy = enemyFactory.CreateEnemy(enemyType, spawnPoint.position);

            if (enemy == null)
            {
                Debug.LogError($"Failed to spawn {enemyType} enemy!");
                return;
            }

            Debug.Log($"Spawned {enemyType} at {spawnPoint.position}");
            EventManager.Instance?.TriggerEvent(GameEventType.EnemySpawned, enemy);
        }

        // опнбепйю врн бяе яоюбм-онимрш бюкхдмш
        private bool HasValidSpawnPoints()
        {
            foreach (var point in spawnPoints)
            {
                if (point != null) return true;
            }
            return false;
        }

        // онксвемхе яксвюимнцн бюкхдмнцн яоюбм-онимрю
        private Transform GetRandomValidSpawnPoint()
        {
            // янахпюел рнкэйн бюкхдмше онимрш
            var validPoints = new System.Collections.Generic.List<Transform>();
            foreach (var point in spawnPoints)
            {
                if (point != null) validPoints.Add(point);
            }

            if (validPoints.Count == 0) return null;
            return validPoints[Random.Range(0, validPoints.Count)];
        }

        private void CreateSpawnPoints()
        {
            Debug.Log("Creating spawn points...");

            Vector3[] positions = {
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 0),
                new Vector3(0, 8, 0),
                new Vector3(0, -8, 0)
            };

            spawnPoints = new Transform[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject point = new GameObject($"SpawnPoint_{i}");
                point.transform.position = positions[i];
                point.transform.SetParent(transform); // ДЕКЮЕЛ ДНВЕПМХЛХ
                spawnPoints[i] = point.transform;
            }

            Debug.Log($"Created {spawnPoints.Length} spawn points");
        }

        private EnemyType GetRandomEnemyType()
        {
            float random = Random.value;
            if (random < 0.6f) return EnemyType.Normal;
            if (random < 0.8f) return EnemyType.Fast;
            if (random < 0.95f) return EnemyType.Heavy;
            return EnemyType.Elite;
        }

        private void OnGameStart(object data)
        {
            Debug.Log("Game started - beginning difficulty progression");
            StartCoroutine(IncreaseDifficulty());
        }

        private IEnumerator IncreaseDifficulty()
        {
            while (true)
            {
                yield return new WaitForSeconds(30f);
                currentSpawnRate = Mathf.Max(0.5f, currentSpawnRate - spawnRateIncrease);
                Debug.Log($"Difficulty increased! Spawn rate: {currentSpawnRate:F2}s");
            }
        }

        // гюыхрю нр смхврнфемхъ - ме дюел смхврнфхрэ яоюбмеп
        private void OnDestroy()
        {
            Debug.Log("EnemySpawner destroyed");
        }
    }
}