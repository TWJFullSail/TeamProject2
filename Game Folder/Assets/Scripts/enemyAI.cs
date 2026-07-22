using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

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

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    float shootTimer;
    float angleToPlayer;
    float roamTimer;
    float stoppingDistOrig;

    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"{name} | Agent assigned: {agent != null}");

        if (agent != null)
        {
            Debug.Log($"{name} | Enabled: {agent.enabled}");
            Debug.Log($"{name} | On NavMesh: {agent.isOnNavMesh}");
        }

        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && canSeePlayer())
        {

        }
        else
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if(agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;

            if(roamTimer >= roamPauseTime)
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
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;

        }
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        } 
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }


    bool canSeePlayer()
    {
        shootTimer += Time.deltaTime;

        CharacterController cc = gamemanager.instance.player.GetComponent<CharacterController>();

        Vector3 aimPoint = cc.bounds.center;

        playerDir = aimPoint - shootPos.position;

        if (playerDir.sqrMagnitude < 0.01f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, playerDir.normalized, out hit, playerDir.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.root == transform.root)
                return false;

            if (hit.collider.CompareTag("Player"))
            {
                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(gamemanager.instance.player.transform.position);
                }
                else
                {
                    Debug.LogError($"{name}: NavMeshAgent is not on a NavMesh!");
                }
                rotateGun();
                faceTarget();

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                agent.stoppingDistance = stoppingDistOrig > 0 ? stoppingDistOrig : 8f;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void rotateGun()
    {
        if (playerDir.sqrMagnitude < 0.01f) return;

        Quaternion rot = Quaternion.LookRotation(playerDir);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, gunRotateSpeed * Time.deltaTime);
    }

    void shoot()
    {
        shootTimer = 0;
        if (bullet != null)
        {
            GameObject newBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(playerDir));

            Collider bulletCol = newBullet.GetComponent<Collider>();
            if (bulletCol != null)
            {
                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(bulletCol, col);
                }
            }
        }
    }
}
