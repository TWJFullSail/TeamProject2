using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float lifetime = 30f;
    public AudioClip pickupSound;
    void Start()
    {
        Destroy(gameObject, lifetime);
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

                Destroy(gameObject);

            }
        }
    }
   
}
