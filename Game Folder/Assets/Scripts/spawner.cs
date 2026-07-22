using UnityEngine;
using UnityEngine.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int amountToSpawn;
    [SerializeField] float spawnRate;
    [SerializeField] int spawnDist;

    int spawnCount;
    float spawnTimer;

    bool startSpawning;

    void Start()
    {
        if (gamemanager.instance == null || objectToSpawn == null || amountToSpawn <= 0)
        {
            enabled = false;
            return;
        }

        gamemanager.instance.updateGameGoal(amountToSpawn);						// registers the total enemies required to win
    }

    void Update()
    {
        if (!startSpawning || gamemanager.instance == null || gamemanager.instance.isPaused)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnCount < amountToSpawn && spawnTimer >= spawnRate)
        {
            spawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void spawn()
    {
        spawnTimer = 0;

        Vector3 ranPos = Random.insideUnitSphere * spawnDist;
        ranPos += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(ranPos, out hit, spawnDist, NavMesh.AllAreas))
        {
            Instantiate(objectToSpawn, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));

            spawnCount++;														// only counts successful spawns
        }
    }
}