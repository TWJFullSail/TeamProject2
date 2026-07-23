using UnityEngine;

using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Prefab")]
    public GameObject coinPrefab;

    [Header("Continuous Spawn Settings")]
    public float spawnInterval = 2.2f;
    public bool spawnOnWaveStart = true;
    public int coinsPerWaveBurst = 8;

    [Header("Spawn Area")]
    public float spawnRadius = 25f;
    public float minHeight = 0.5f;
    public float maxHeight = 1.5f;
    public LayerMask groundLayer;

    [Header("Limits")]
    public int maxCoinsAlive = 35;

    private int currentCoinsAlive = 0;

    private void Start()
    {
        StartCoroutine(ContinuousSpawn());
    }

    private IEnumerator ContinuousSpawn()
    {
        while (true)
        {
            if (currentCoinsAlive < maxCoinsAlive && coinPrefab != null)
            {
                Vector3 pos = GetRandomSpawnPosition();
                if (pos != Vector3.zero)
                {
                    Instantiate(coinPrefab, pos, Quaternion.identity);
                    currentCoinsAlive++;
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnWaveStart()
    {
        if (!spawnOnWaveStart || coinPrefab == null) return;

        for (int i = 0; i < coinsPerWaveBurst; i++)
        {
            Vector3 pos = GetRandomSpawnPosition();
            if (pos != Vector3.zero)
            {
                Instantiate(coinPrefab, pos, Quaternion.identity);
                currentCoinsAlive++;
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < 25; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = transform.position + new Vector3(randomCircle.x, 25f, randomCircle.y);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 50f, groundLayer))
            {
                return hit.point + Vector3.up * Random.Range(minHeight, maxHeight);
            }
        }
        return Vector3.zero;
    }

    public void OnCoinDestroyed()
    {
        if (currentCoinsAlive > 0)
            currentCoinsAlive--;
    }
}