using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public gunStats gun;
    public Camera playerCamera;

    public AudioSource audioSource;

    int currentAmmo;
    float shootTimer;


    void Start()
    {
        currentAmmo = gun.ammoMax;
    }


    void Update()
    {
        shootTimer += Time.deltaTime;


        if (Input.GetButton("Fire1") && shootTimer >= gun.shootRate)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo");
            return;
        }


        shootTimer = 0;
        currentAmmo--;


        PlayShootSound();


        RaycastHit hit;


        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            gun.shootDist))
        {

            Debug.Log("Hit: " + hit.collider.name);


            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (damageable != null)
            {
                damageable.takeDamage(gun.shootDamage);
            }


            if (gun.hitEffect != null)
            {
                Instantiate(
                    gun.hitEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
            }
        }
    }


    void PlayShootSound()
    {
        if (gun.shootSound.Length > 0)
        {
            AudioClip clip = gun.shootSound[
                Random.Range(0, gun.shootSound.Length)
            ];

            audioSource.PlayOneShot(
                clip,
                gun.shootSoundVol
            );
        }
    }
}