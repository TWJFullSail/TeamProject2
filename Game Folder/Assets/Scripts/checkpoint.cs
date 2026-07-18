using UnityEngine;
using System.Collections;

public class checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gamemanager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gamemanager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(displayPopup());
        }
    }

    IEnumerator displayPopup()
    {
        gamemanager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        gamemanager.instance.checkpointPopup.SetActive(false);

    }
}
