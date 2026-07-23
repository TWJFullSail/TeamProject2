
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
        if (shopUI == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= interactDistance && !shopUI.isOpen)
        {
            GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 80, 160, 40),
                      "Press Tab to open Shop");
        }
    }
}
