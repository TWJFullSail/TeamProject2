using UnityEngine;

public class pickupWeapon : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] weaponStats weapon;

    private void OnTriggerEnter(Collider other)
    {
        IPickupWeapon pic = other.GetComponent<IPickupWeapon>();
        if (pic != null)
        {
            pic.getWeaponStats(prefab, weapon);
            Destroy(gameObject);
        }
    }
}
