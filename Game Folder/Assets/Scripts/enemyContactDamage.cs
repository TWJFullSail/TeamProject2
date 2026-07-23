using UnityEngine;
using System.Collections;

public class enemyContactDamage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;

    int damageAmountOrig;
    bool isDamaging;

    void Awake()
    {
        damageAmountOrig = damageAmount;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDamaging)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    public void setDamageMultiplier(float multiplier)
    {
        multiplier = Mathf.Max(0.01f, multiplier);

        damageAmount = Mathf.Max(1, Mathf.RoundToInt(damageAmountOrig * multiplier));
    }

    IEnumerator damageOther(IDamage dmg)
    {
        isDamaging = true;

        dmg.takeDamage(damageAmount);

        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }
}