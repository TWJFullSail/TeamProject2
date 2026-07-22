using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour
{
    public float speed;
    public float duration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyProjectile());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
