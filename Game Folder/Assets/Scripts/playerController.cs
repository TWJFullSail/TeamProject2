using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static weaponStats;

public class playerController : MonoBehaviour, IDamage, IPickupGun
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Range(0, 500)][SerializeField] int HP;
    [Range(0, 100)][SerializeField] int Stamina;
    [Range(1, 20)][SerializeField] int StaminaRate;
    [Range(1, 50)][SerializeField] int speed;
    [Range(1, 10)][SerializeField] int sprintMod;
    [Range(1, 50)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(10, 100)][SerializeField] int gravity;
    
    [SerializeField] List<gunStats> gunInv = new List<gunStats>();
    gunStats currentGun;
    [SerializeField] GameObject gunModel;

    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    //Old idea for "Bullet Heaven multiple weapon system abandoned
    /*
    [SerializeField] List<GameObject> weaponInv = new List<GameObject>();    
    [SerializeField] Transform aura;
    [SerializeField] Transform arms;
    [SerializeField] Transform rangeWeapon;
    [SerializeField] Transform passive;
    HashSet<stanceType> stanceInv = new HashSet<stanceType>();
    [SerializeField] List<weaponStats> statInv = new List<weaponStats>();
    int stanceInvPos;
    /**/

    int jumpCount;
    int HPOrig;
    int StaminaOrig;    
    int gunInvPos = 0;
    float shootTimer;

    bool isSprinting;
    bool isPlayingSteps;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOrig = HP;
        StaminaOrig = Stamina;
        spawnPlayer();
    }
    void Update()
    {
        if (!gamemanager.instance.isPaused)
        {
            movement();            
        }
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;

            if (moveDir.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(playSteps());
            }
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

        sprint();
        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;        

        shootTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && gunInv.Count > 0 && currentGun.ammoCur > 0 && shootTimer > currentGun.shootRate)
        {
            shoot();
        }

        selectGun();
        reload();
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint") && Stamina > 0) {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint")) {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;

        audioManager.instance.audPlayer.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
            if (Stamina > 0)
            {
                Stamina--;
                updatePlayerStamina();
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            if (Stamina < StaminaOrig)
            {
                Stamina += StaminaRate;
                updatePlayerStamina();
            }            
        }
        isPlayingSteps = false;
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && Stamina >= 10)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
            Stamina -= 10;
            updatePlayerStamina();
        }
    }

    void shoot()
    {
        shootTimer = 0;
        currentGun.ammoCur--;
        audioManager.instance.audPlayer.PlayOneShot(currentGun.shootSound[Random.Range(0, currentGun.shootSound.Length)], currentGun.shootSoundVol);
        updateAmmoUI();

        GameObject projectile = currentGun.projectile;
        if (projectile != null)
        {
            Instantiate(projectile, gunModel.transform.position + projectile.transform.position, Quaternion.identity);
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currentGun.shootDist, ~ignoreLayer))
            {
                //Debug.Log(hit.collider.name);
                if (currentGun.hitEffect != null)
                {
                    Instantiate(currentGun.hitEffect, hit.point, Quaternion.identity);
                }

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(currentGun.shootDamage);
                }
            }
        }
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && currentGun != null && currentGun.ammoCur < currentGun.clipSize && currentGun.ammoTotal > 0)
        {
            int missing = currentGun.clipSize - currentGun.ammoCur;
            if (currentGun.ammoTotal > missing)
            {
                currentGun.ammoTotal -= missing;
                currentGun.ammoCur += missing;
            }
            else
            {
                currentGun.ammoCur = currentGun.ammoTotal;
                currentGun.ammoTotal = 0;
            }
            updateAmmoUI();
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        audioManager.instance.audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        updatePlayerHP();
        StartCoroutine(flashDamage());

        if(HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        updatePlayerHP();
        updatePlayerStamina();
        updateAmmoUI();        
    }

    public void updatePlayerHP()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gamemanager.instance.playerHPText.text = HP.ToString("F00") + " / " + HPOrig.ToString("F00");
    }

    public void updatePlayerStamina()
    {
        gamemanager.instance.playerStaminaBar.fillAmount = (float)Stamina / StaminaOrig;
        gamemanager.instance.playerStaminaText.text = Stamina.ToString("F00") + " / " + StaminaOrig.ToString("F00");
    }

    public void updateAmmoUI()
    {
        gamemanager.instance.ammoText.text = currentGun.ammoCur.ToString("F0") + " / " + currentGun.clipSize.ToString("F0");
        int maxAmmo = currentGun.clipSize * (currentGun.clipMax - 1);
        gamemanager.instance.totalAmmoText.text = currentGun.ammoTotal.ToString("F0") + " / " + maxAmmo.ToString("F0");
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
        bool isNew = true;
        foreach (gunStats gs in gunInv)
        {
            if (gs.gunModel == gun.gunModel)
            {
                isNew = false;
                int maxAmmo = gun.clipSize * (gun.clipMax - 1);
                gs.ammoTotal += gun.clipSize;

                if (gs.ammoTotal > maxAmmo)
                {
                    gs.ammoTotal = maxAmmo;
                }
                updateAmmoUI();
                break;
            }
        }

        if (isNew)
        {
            gun.ammoCur = gun.clipSize;
            gunInv.Add(gun);
            gunInvPos = gunInv.Count - 1;

            changeGun();
        }
    }

    void changeGun()
    {
        currentGun = gunInv[gunInvPos];
        gunModel.GetComponent<MeshFilter>().sharedMesh = currentGun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = currentGun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        updateAmmoUI();
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunInvPos < gunInv.Count - 1)
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
        Stamina = StaminaOrig;
        currentGun = gunInv[gunInvPos];
        currentGun.ammoCur = currentGun.clipSize;
        updatePlayerUI();
    }

    /*

    public void getWeaponStats(GameObject prefab, weaponStats weapon)
    {
        weaponInv.Add(prefab);
        statInv.Add(weapon);

        int originalCount = stanceInv.Count;
        stanceInv.Add(weapon.stance);        
        
        if (originalCount < stanceInv.Count)
        {
            stanceInvPos = stanceInv.Count - 1;            
        }

        changeWeapon();
    }

    stanceType GetCurrentStance()
    {
        if (stanceInv.Count == 0) return default;

        List<stanceType> stanceList = stanceInv.ToList<stanceType>();

        if (stanceInvPos < 0 || stanceInvPos >= stanceList.Count)
            return default;

        return stanceList[stanceInvPos];
    }

    void changeWeapon()
    {
        foreach (Transform child in aura)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in arms)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in rangeWeapon)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in passive)
        {
            Destroy(child.gameObject);
        }

        stanceType currentStance = GetCurrentStance();

        for (int i = 0; i < weaponInv.Count; i++)
        {
            weaponStats ws = statInv[i];
            if (ws.stance == currentStance)
            {
                GameObject prefab = weaponInv[i];
                switch ((int)ws.type)
                {
                    case 0:
                        Instantiate(prefab, aura.position, aura.rotation, aura);
                        break;
                    case 1:
                        Instantiate(prefab, arms.position, arms.rotation, arms);
                        break;
                    case 2:
                        Instantiate(prefab, rangeWeapon.position, rangeWeapon.rotation, rangeWeapon);
                        break;
                    case 3:
                        Instantiate(prefab, passive.position, passive.rotation, passive);
                        break;
                }
            }
        }
    }

    void changeStance()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && stanceInvPos < stanceInv.Count - 1)
        {
            stanceInvPos++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && stanceInvPos > 0)
        {
            stanceInvPos--;
            changeWeapon();
        }
    }
    /**/
}