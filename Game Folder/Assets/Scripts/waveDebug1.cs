using UnityEngine;

public class waveDebug : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemyAI enemy =
                FindFirstObjectByType<enemyAI>();

            if (enemy != null)
            {
                enemy.takeDamage(5);							// tests enemy health
                Debug.Log("Debug dealt 5 damage.");
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            enemyAI enemy =
                FindFirstObjectByType<enemyAI>();

            if (enemy != null)
            {
                enemy.takeDamage(999999);						// clears one enemy
                Debug.Log("Debug defeated one enemy.");
            }
        }
#endif
    }
}