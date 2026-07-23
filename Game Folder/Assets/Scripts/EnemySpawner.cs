using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] int startingEnemies = 3;
    [SerializeField] float timeBetweenEnemies = 1f;
    [SerializeField] float timeBetweenWaves = 5f;

    [SerializeField] int totalWaves = 3;
    [SerializeField] float healthIncreasePerWave = 0.25f;
    [SerializeField] float damageIncreasePerWave = 0.15f;

    int currentWave = 1;

    void Start()
    {
        if (enemyPrefabs == null ||
        enemyPrefabs.Length == 0 ||
        spawnPoints == null ||
        spawnPoints.Length == 0)
        {
            Debug.LogError(
                "EnemySpawner needs enemy prefabs and spawn points.");

            enabled = false;
            return;
        }

        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (currentWave <= totalWaves)
        {
            int enemyCount = startingEnemies + (currentWave - 1);
            bool finalWave = currentWave == totalWaves;

            gamemanager.instance.startWave(
                enemyCount,
                finalWave);                                                 // registers the current wave goal

            Debug.Log(
                "Starting Wave " + currentWave +
                " with " + enemyCount + " enemies.");

            yield return StartCoroutine(SpawnWave(enemyCount));

            // Wait until every enemy has been defeated
            while (gamemanager.instance.gameGoalCount > 0)
            {
                yield return null;
            }

            Debug.Log("Wave " + currentWave + " Complete!");

            if (finalWave)
            {
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;

        }
    }

    IEnumerator SpawnWave(int enemyCount)
    {
        float healthMultiplier =
            1f + ((currentWave - 1) * healthIncreasePerWave);

        float damageMultiplier =
            1f + ((currentWave - 1) * damageIncreasePerWave);

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject selectedEnemy =
                getEnemyForWave();

            if (selectedEnemy == null)
            {
                Debug.LogError(
                    "No valid enemy prefab was found.");

                yield break;
            }

            GameObject enemyInstance =
                Instantiate(
                    selectedEnemy,
                    spawn.position,
                    spawn.rotation);

            enemyAI enemyScript =
                enemyInstance.GetComponent<enemyAI>();

            if (enemyScript == null)
            {
                enemyScript =
                    enemyInstance.GetComponentInChildren<enemyAI>();
            }

            if (enemyScript != null)
            {
                enemyScript.setWaveStats(
                    healthMultiplier,
                    damageMultiplier);                                      // applies the wave difficulty
            }

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    GameObject getEnemyForWave()
    {
        int unlockedEnemyTypes =
            Mathf.Clamp(
                currentWave,
                1,
                enemyPrefabs.Length);

        int startIndex =
            Random.Range(0, unlockedEnemyTypes);

        for (int i = 0; i < unlockedEnemyTypes; i++)
        {
            int index =
                (startIndex + i) % unlockedEnemyTypes;

            if (enemyPrefabs[index] != null)
            {
                return enemyPrefabs[index];
            }
        }

        return null;
    }
}