using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunInv = new List<gunStats>();
    [SerializeField] GameObject gunModel;

    [SerializeField] gunStats startingGun;

    int jumpCount;
    int HPOrig;
    int gunInvPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOrig = HP;

        if (startingGun != null)
        {
            getGunStats(startingGun);
        }

        spawnPlayer();
    }
    void Update()
    {
        if (!gamemanager.instance.isPaused)
        {
            movement();
        }

        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        shootTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && gunInv.Count > 0 && gunInv[gunInvPos].ammoCur > 0 && shootTimer > gunInv[gunInvPos].shootRate)
        {

            shoot();
        }

        if (gunInv.Count > 0)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunInv[gunInvPos].shootDist, Color.red);
        }

        selectGun();
        reload();
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunInv[gunInvPos].ammoCur--;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunInv[gunInvPos].shootDist, ~ignoreLayer))
        {

            //Debug.Log(hit.collider.name);

            Instantiate(gunInv[gunInvPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(gunInv[gunInvPos].shootDamage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();

        }
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunInv.Count > 0)
        {
            gunInv[gunInvPos].ammoCur = gunInv[gunInvPos].ammoMax;
        }
    }

    public void updatePlayerUI()
    {
        if (gamemanager.instance.playerHPBar != null)
        {
            gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        }
    }

    IEnumerator flashDamage()
    {
        if (gamemanager.instance.playerDamageScreen != null)
        {
            gamemanager.instance.playerDamageScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            gamemanager.instance.playerDamageScreen.SetActive(false);
        }
    }

    public void getGunStats(gunStats gun)
    {

        gunInv.Add(gun);
        gunInvPos = gunInv.Count - 1;

        changeGun();

    }

    void changeGun()
    {
        foreach (Transform child in gunModel.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(gunInv[gunInvPos].gunModel, gunModel.transform);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunInvPos < gunInv.Count - 1)
        {
            gunInvPos++;
            changeGun();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunInvPos > 0)
        {
            gunInvPos--;
            changeGun();
        }
    }

    public void spawnPlayer()
    {
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;
        Physics.SyncTransforms();
        HP = HPOrig;
        updatePlayerUI();
    }
}