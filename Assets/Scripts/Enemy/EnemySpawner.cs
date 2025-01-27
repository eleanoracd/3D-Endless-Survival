using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int randomWaypointCount = 3;
    [SerializeField] private float maxActiveTime = 30f;
    [SerializeField] private float playerRadius = 30f;
    [SerializeField] private Transform playerTransform;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(CheckDistantEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = enemyPool.GetObject(spawnPoint.position, Quaternion.identity);

            if (enemy != null)
            {
                SetEnemyWaypoints(enemy);
                StartCoroutine(TrackEnemyLifetime(enemy));
            }
        }
    }

    private void SetEnemyWaypoints(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            Transform[] randomWaypoints = GetRandomWaypoints();
            enemyAI.SetWaypoints(randomWaypoints);
        }
    }

    private Transform[] GetRandomWaypoints()
    {
        Transform[] shuffledWaypoints = (Transform[])waypoints.Clone();
        for (int i = 0; i < shuffledWaypoints.Length; i++)
        {
            int randomIndex = Random.Range(i, shuffledWaypoints.Length);
            (shuffledWaypoints[i], shuffledWaypoints[randomIndex]) = (shuffledWaypoints[randomIndex], shuffledWaypoints[i]);
        }

        Transform[] selectedWaypoints = new Transform[randomWaypointCount];
        for (int i = 0; i < randomWaypointCount; i++)
        {
            selectedWaypoints[i] = shuffledWaypoints[i];
        }
        return selectedWaypoints;
    }

    private IEnumerator CheckDistantEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            var activeEnemiesSnapshot = new List<GameObject>(enemyPool.ActiveObjects);

            foreach (GameObject enemy in activeEnemiesSnapshot)
            {
                if (enemy != null && enemy.activeSelf)
                {
                    float distanceToPlayer = Vector3.Distance(playerTransform.position, enemy.transform.position);

    
                    if (distanceToPlayer > playerRadius)
                    {
                        StartCoroutine(DelayedReturnToPool(enemy));
                    }
                }
            }
        }
    }

    private IEnumerator TrackEnemyLifetime(GameObject enemy)
    {
        float activeTime = 0f;

        while (enemy.activeSelf)
        {
            activeTime += Time.deltaTime;

            if (activeTime >= maxActiveTime)
            {
                enemyPool.ReturnObject(enemy);
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DelayedReturnToPool(GameObject enemy)
    {
        yield return new WaitForSeconds(10f);

        if (enemy.activeSelf)
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, enemy.transform.position);
            if (distanceToPlayer > playerRadius)
            {
                enemyPool.ReturnObject(enemy);
            }
        }
    }
}