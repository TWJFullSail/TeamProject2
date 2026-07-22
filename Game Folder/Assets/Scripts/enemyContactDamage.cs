using UnityEngine;
using System.Collections;

public class enemyContactDamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;

    bool isDamaging;

    private void OnTriggerStay(Collider other)
    {
        if (isDamaging)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            IDamage dmg = other.GetComponentInParent<IDamage>();					// gets damage interface from the player

            if (dmg != null)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    IEnumerator damageOther(IDamage dmg)
    {
        isDamaging = true;

        dmg.takeDamage(damageAmount);

        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }
}