using UnityEngine;

public class pickupWeapon : MonoBehaviour
{
    [SerializeField] weaponStats weapon;

    private void OnTriggerEnter(Collider other)
    {
        IPickupWeapon pic = other.GetComponent<IPickupWeapon>();
        if (pic != null)
        {
            pic.getWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}
