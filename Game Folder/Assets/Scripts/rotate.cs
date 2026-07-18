using UnityEngine;

public class rotate : MonoBehaviour
{
    [SerializeField] int speed;
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
