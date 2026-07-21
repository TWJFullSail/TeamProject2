using UnityEngine;

public class pickupGun : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickupGun pic = other.GetComponent<IPickupGun>();
        if (pic != null)
        {
            gun.ammoCur = gun.ammoMax;
            pic.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
