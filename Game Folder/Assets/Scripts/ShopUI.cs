using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public TextMeshProUGUI coinText;

    [Header("Interaction")]
    public float interactDistance = 5f;

    playerController player;
    Transform playerTransform;
    public bool isOpen = false;          

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
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
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
        if (player == null || shopPanel == null) return;

        isOpen = true;
        shopPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gamemanager gm = gamemanager.instance;
        if (gm != null)
        {
            gm.statePause();
            gm.menuActive = shopPanel;       
        }

        UpdateCoinDisplay();
    }

    public void CloseShop()
    {
        if (shopPanel == null) return;

        isOpen = false;
        shopPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        if (gamemanager.instance != null)
        {
            gamemanager.instance.isPaused = false;
            Time.timeScale = gamemanager.instance.timeScaleOrig;   
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

           
            if (gamemanager.instance.menuActive == shopPanel)
            {
                gamemanager.instance.menuActive = null;
            }
        }
    }

    public void BuyHealth() => player?.BuyHealthUpgrade();
    public void BuyStamina() => player?.BuyStaminaUpgrade();
    public void BuyAmmo() => player?.BuyAmmoUpgrade();

    public void UpdateCoinDisplay()
    {
        if (coinText != null && player != null)
            coinText.text = $"Coins: {player.coins}";
    }
}