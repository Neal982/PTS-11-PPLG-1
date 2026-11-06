using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField, Min(0)] private int initialSpawnCount = 1;
    [SerializeField, Min(1)] private int maxAliveEnemies = 5;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(8f, 5f);
    [SerializeField, Min(0.1f)] private float spawnInterval = 5f;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned.", this);
            enabled = false;
            return;
        }

        int spawnCount = Mathf.Min(initialSpawnCount, maxAliveEnemies);
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }

        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            RemoveDestroyedEnemies();
            if (activeEnemies.Count < maxAliveEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector2 localPosition = new Vector2(
            Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
            Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f)
        );
        Vector3 spawnPosition = transform.TransformPoint(localPosition);

        Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(spawnedEnemy);
    }

    private void RemoveDestroyedEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    private void OnValidate()
    {
        initialSpawnCount = Mathf.Max(0, initialSpawnCount);
        maxAliveEnemies = Mathf.Max(1, maxAliveEnemies);
        spawnAreaSize.x = Mathf.Max(0.1f, spawnAreaSize.x);
        spawnAreaSize.y = Mathf.Max(0.1f, spawnAreaSize.y);
        spawnInterval = Mathf.Max(0.1f, spawnInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));
        Gizmos.matrix = previousMatrix;
    }
}
