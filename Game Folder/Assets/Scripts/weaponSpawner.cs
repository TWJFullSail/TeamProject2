using System.Collections;
using UnityEngine;

public class weaponSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] weaponList;
    [SerializeField] GameObject weapon;
    [Range(1, 60)][SerializeField] float respawnDelay = 20f;
    private bool respawning = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon == null && !respawning)
        {
            StartCoroutine(RespawnWeapon());
        }
    }
    void SpawnWeapon()
    {
        int index = Random.Range(0, weaponList.Length);
        weapon = Instantiate(weaponList[index], transform.position, transform.rotation);
    }

    IEnumerator RespawnWeapon()
    {
        respawning = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnWeapon();
        respawning = false;
    }
}
