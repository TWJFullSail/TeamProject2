using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] GameObject enemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] int startingEnemies = 3;
    [SerializeField] float timeBetweenEnemies = 1f;
    [SerializeField] float timeBetweenWaves = 5f;

    int currentWave = 1;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            Debug.Log("Starting Wave " + currentWave);

            yield return StartCoroutine(SpawnWave());

            // Wait until every enemy has been defeated
            while (gamemanager.instance.gameGoalCount > 0)
            {
                yield return null;
            }

            Debug.Log("Wave Complete!");

            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;

            while (gamemanager.instance.gameGoalCount > 0)
            {
                Debug.Log("Enemies Remaining: " + gamemanager.instance.gameGoalCount);
                yield return null;
            }
        }
    }

    IEnumerator SpawnWave()
    {
        int enemyCount = startingEnemies + (currentWave - 1);

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(enemyPrefab, spawn.position, spawn.rotation);

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }
}