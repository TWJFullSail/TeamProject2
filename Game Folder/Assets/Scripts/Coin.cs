using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float lifetime = 30f;
    public AudioClip pickupSound;

    [Header("Spin")]
    public float spinSpeed = 180f;
    public Vector3 spinAxis = Vector3.up;

    private CoinSpawner spawner;

    void Start()
    {
        Destroy(gameObject, lifetime);
        spawner = FindAnyObjectByType<CoinSpawner>();
    }

    void Update()
    {
        transform.Rotate(spinAxis * spinSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCurrency currency = other.GetComponent<PlayerCurrency>();
            if (currency != null)
            {
                currency.AddCoins(value);

                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (spawner != null)
                spawner.OnCoinDestroyed();

            Destroy(gameObject);
        }
    }
}