using UnityEngine;
using UnityEngine.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int amountToSpawn;
    [SerializeField] float spawnRate;
    [SerializeField] int spawnDist;

    [SerializeField] bool useWaveManager;

    int spawnCount;
    float spawnTimer;

    bool startSpawning;

    void Start()
    {
        if (useWaveManager)
        {
            return;
        }


        if (gamemanager.instance == null || objectToSpawn == null || amountToSpawn <= 0)
        {
            enabled = false;
            return;
        }

        gamemanager.instance.updateGameGoal(amountToSpawn);						// registers the total enemies required to win
    }

    void Update()
    {

        if (useWaveManager)
        {
            return;
        }
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

        if (useWaveManager)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    public bool spawnEnemy(
    GameObject enemyPrefab,
    float healthMultiplier,
    float damageMultiplier)
    {
        if (enemyPrefab == null)
        {
            return false;
        }

        Vector3 ranPos = Random.insideUnitSphere * spawnDist;
        ranPos += transform.position;

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            ranPos,
            out hit,
            spawnDist,
            NavMesh.AllAreas))
        {
            return false;
        }

        GameObject enemyInstance =
            Instantiate(
                enemyPrefab,
                hit.position,
                Quaternion.Euler(0, Random.Range(0, 360), 0));

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
                damageMultiplier);                                      // applies wave difficulty
        }

        return true;
    }

    void spawn()
    {
        spawnTimer = 0;

        if (spawnEnemy(objectToSpawn, 1, 1))
        {
            spawnCount++;                                               // only counts successful spawns
        }
    }
}