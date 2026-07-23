using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public TextMeshProUGUI coinText;

    [Header("Interaction")]
    public KeyCode closeKey = KeyCode.E;
    public float interactDistance = 5f;

    playerController player;
    Transform playerTransform;
    bool isOpen = false;

    void Start()
    {
        player = FindAnyObjectByType<playerController>();
        if (player != null)
            playerTransform = player.transform;

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(closeKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseShop();
            }
            return;
        }

        if (playerTransform != null &&
            Vector3.Distance(playerTransform.position, transform.position) <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenShop();
            }
        }
    }

    public void OpenShop()
    {
        if (player == null) return;

        isOpen = true;
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        UpdateCoinDisplay();
    }

    public void CloseShop()
    {
        isOpen = false;
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public bool IsOpen => isOpen;

    public void BuyHealth() => player?.BuyHealthUpgrade();
    public void BuyStamina() => player?.BuyStaminaUpgrade();
    public void BuyAmmo() => player?.BuyAmmoUpgrade();

    public void UpdateCoinDisplay()
    {
        if (coinText != null && player != null)
            coinText.text = $"Coins: {player.coins}";
    }

    void OnGUI()
    {
        if (isOpen || playerTransform == null) return;

        float dist = Vector3.Distance(playerTransform.position, transform.position);
        if (dist <= interactDistance)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240, 50),
                      "Press E to Open Shop", style);
        }
    }
}