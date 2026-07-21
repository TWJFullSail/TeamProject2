using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour
{
    [SerializeField] LineRenderer laserLine;

    [SerializeField] GameObject hitEffect;
    [SerializeField] Transform laserStartPos;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int laserDistMax;

    bool isDamaging;

    void Update()
    {
        createLaser();
    }

    void createLaser()
    {
        RaycastHit hit;

        laserLine.SetPosition(0, laserStartPos.position);

        if (Physics.Raycast(laserStartPos.position, laserStartPos.forward, out hit, laserDistMax))
        {

            //laserLine.SetPosition(0, laserStartPos.position);
            laserLine.SetPosition(1, hit.point);
            hitEffect.SetActive(true);
            hitEffect.transform.position = hit.point;

            IDamage dmg = hit.collider.GetComponent<IDamage>();                                                 // checks if the hit object can take damage
            if (dmg != null && !isDamaging)
            {
                StartCoroutine(damageTime(dmg));
            }
            else
            {
                //laserLine.SetPosition(0, laserStartPos.position);
                laserLine.SetPosition(1, laserStartPos.position + laserStartPos.forward * laserDistMax);
                hitEffect.SetActive(false);
            }

        }
    }

    IEnumerator damageTime(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

}
