using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] int gunRotateSpeed;

    int HPOrig;																// stores the prefab's original health

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    float shootTimer;
    float angleToPlayer;
    float roamTimer;
    float stoppingDistOrig;
    float waveDamageMultiplier = 1;

    bool playerInTrigger;
    bool hasReportedDeath;

    void Awake()
    {
        HPOrig = HP;
    }

    void Start()
    {
        if (model != null)
        {
            colorOrig = model.material.color;
        }

        startingPos = transform.position;

        if (agent != null)
        {
            stoppingDistOrig = agent.stoppingDistance;
        }
    }

    void Update()
    {
        if (!agentReady())													// prevents NavMesh calls before the agent is ready
        {
            return;
        }

        if (gamemanager.instance == null ||
            gamemanager.instance.player == null)
        {
            return;
        }

        if (playerInTrigger && canSeePlayer())
        {

        }
        else
        {
            checkRoam();
        }
    }

    public void setWaveStats(float healthMultiplier, float damageMultiplier)
    {
        healthMultiplier = Mathf.Max(0.01f, healthMultiplier);
        damageMultiplier = Mathf.Max(0.01f, damageMultiplier);

        HP = Mathf.Max(1,
            Mathf.RoundToInt(HPOrig * healthMultiplier));                   // applies the current wave's health increase

        waveDamageMultiplier = damageMultiplier;

        enemyContactDamage[] contactDamageScripts =
            GetComponentsInChildren<enemyContactDamage>(true);

        for (int i = 0; i < contactDamageScripts.Length; i++)
        {
            contactDamageScripts[i].setDamageMultiplier(
                waveDamageMultiplier);
        }

        damage[] damageScripts =
            GetComponentsInChildren<damage>(true);

        for (int i = 0; i < damageScripts.Length; i++)
        {
            damageScripts[i].setDamageMultiplier(
                waveDamageMultiplier);
        }
    }
    bool agentReady()
    {
        return agent != null &&
               agent.enabled &&
               agent.isOnNavMesh;
    }

    void checkRoam()
    {
        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;

            if (roamTimer >= roamPauseTime)
            {
                roam();
            }
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            ranPos, out hit, roamDist, NavMesh.AllAreas))					// confirms the random point is on a NavMesh
        {
            agent.SetDestination(hit.position);
        }
    }

    bool canSeePlayer()
    {
        shootTimer += Time.deltaTime;

        playerDir =
            gamemanager.instance.player.transform.position -
            transform.position;

        angleToPlayer =
            Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(
            transform.position,
            playerDir.normalized,
            out hit,
            playerDir.magnitude))
        {
            if (hit.collider.CompareTag("Player") &&
                angleToPlayer <= FOV)										// checks line of sight and field of view
            {
                agent.SetDestination(
                    gamemanager.instance.player.transform.position);

                rotateGun();
                faceTarget();

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                agent.stoppingDistance = stoppingDistOrig;

                return true;
            }
        }

        agent.stoppingDistance = 0;

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;

            if (agentReady())
            {
                agent.stoppingDistance = 0;
            }
        }
    }

    void shoot()
    {
        shootTimer = 0;

        if (bullet == null ||
            shootPos == null ||
            gunPivot == null)
        {
            return;
        }

        GameObject bulletInstance =
            Instantiate(
                bullet,
                shootPos.position,
                gunPivot.rotation);

        damage bulletDamage =
            bulletInstance.GetComponent<damage>();

        if (bulletDamage == null)
        {
            bulletDamage =
                bulletInstance.GetComponentInChildren<damage>();
        }

        if (bulletDamage != null)
        {
            bulletDamage.setDamageMultiplier(
                waveDamageMultiplier);										// applies the wave damage to the new projectile
        }
    }

    void rotateGun()
    {
        if (gunPivot == null)
        {
            return;
        }

        Quaternion rot = Quaternion.LookRotation(playerDir);

        gunPivot.rotation = Quaternion.Lerp(
            gunPivot.rotation,
            rot,
            gunRotateSpeed * Time.deltaTime);
    }

    void faceTarget()
    {
        Vector3 flatPlayerDir =
            new Vector3(playerDir.x, 0, playerDir.z);

        if (flatPlayerDir == Vector3.zero)
        {
            return;
        }

        Quaternion rot = Quaternion.LookRotation(flatPlayerDir);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rot,
            faceTargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        if (hasReportedDeath)												// prevents the same enemy from being counted twice
        {
            return;
        }

        HP -= amount;

        if (HP <= 0)
        {
            hasReportedDeath = true;

            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateGameGoal(-1);
            }

            Destroy(gameObject);

            return;
        }

        if (agentReady() &&
            gamemanager.instance != null &&
            gamemanager.instance.player != null)
        {
            agent.SetDestination(
                gamemanager.instance.player.transform.position);
        }

        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        if (model == null)
        {
            yield break;
        }

        model.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        model.material.color = colorOrig;
    }
}