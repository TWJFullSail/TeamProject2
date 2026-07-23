
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public ShopUI shopUI;
    Transform player;
    bool playerInRange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        playerInRange = dist <= interactDistance;

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            shopUI.OpenShop();
        }
    }
    void OnGUI()
    {
        if (playerInRange && !shopUI.IsOpen)
        {
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 50, 120, 30), "Press E to Shop");
        }
    }
}
