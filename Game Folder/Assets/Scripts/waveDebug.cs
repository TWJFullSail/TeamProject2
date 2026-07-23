using UnityEngine;

public class waveDebug : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemyAI enemy =
                FindObjectOfType<enemyAI>();

            if (enemy != null)
            {
                enemy.takeDamage(999999);						// removes one enemy for wave testing
            }
        }
#endif
    }
}