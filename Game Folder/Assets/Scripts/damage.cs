using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { bullet, stationary, AOE, Rotate, DOT }

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmt;
    [SerializeField] float damageRate;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    void Start()
    {
        if (type == damageType.bullet)
        {
            if (rb != null)
            {
                rb.angularVelocity = transform.forward * bulletSpeed;
            }
            else
            {
                Debug.LogWarning($"{name}: Rigidbody not assigned on bullet.");
            }

            Destroy(gameObject, bulletDestroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.transform.root == transform.root)
        {

            return;
        }
        
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmt);
        }

        if(type == damageType.bullet)
        {
            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
            
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmt);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
