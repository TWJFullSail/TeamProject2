using UnityEngine;
using System.Collections;
public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int coinsPerWave = 8;
    public float spawnRadius = 25f;
    public float minHeight = 0.5f;
    public float maxHeight = 1.5f;
    public LayerMask groundLayer;

    public void SpawnCoins()
    {
        for (int i = 0; i < coinsPerWave; i++)
        {
            Vector3 pos = GetRandomSpawnPosition();
            if (pos != Vector3.zero)
                Instantiate(coinPrefab, pos, Quaternion.identity);
        }
    }
    Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = transform.position + new Vector3(randomCircle.x, 20f, randomCircle.y);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 40f, groundLayer))
            {
                return hit.point + Vector3.up * Random.Range(minHeight, maxHeight);
            }
        }
        return Vector3.zero;
    }

    public void OnWaveStart()
    {
        SpawnCoins();
    }
}
