using UnityEngine;

public class audioManager : MonoBehaviour
{

    public static audioManager instance;
    public AudioSource audPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }
}
