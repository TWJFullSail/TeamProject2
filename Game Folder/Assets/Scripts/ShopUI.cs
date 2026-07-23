using UnityEngine;
using TMPro;
using System.Collections;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 5f;

    private PlayerCurrency playerCurrency;
    private playerController playerController;
    public bool isOpen = false;

    void Start()
    {
        playerCurrency = FindAnyObjectByType<PlayerCurrency>();
        playerController = FindAnyObjectByType<playerController>();

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (playerCurrency != null)
            playerCurrency.OnCoinsChanged.AddListener(UpdateCoinDisplay);
    }

    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                CloseShop();
            return;
        }

        // Allow opening shop only if no menu is active or if it's the shop itself
        if (playerController != null)
        {
            float dist = Vector3.Distance(playerController.transform.position, transform.position);
            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                // Only open if no other menu is open or if shop is already the active one
                if (gamemanager.instance.menuActive == null || gamemanager.instance.menuActive == shopPanel)
                    OpenShop();
            }
        }
    }

    public void OpenShop()
    {
        if (playerCurrency == null || shopPanel == null) return;

        isOpen = true;
        shopPanel.SetActive(true);

        var gm = gamemanager.instance;
        gm.statePause();
        gm.menuActive = shopPanel;

        UpdateCoinDisplay();
    }

    public void CloseShop()
    {
        if (shopPanel == null) return;

        isOpen = false;
        shopPanel.SetActive(false);

        var gm = gamemanager.instance;
        gm.stateUnpause();
    }

    // ====================== PURCHASES ======================
    public void BuyHealth() => TryPurchase(playerController, "BuyHealthUpgrade");
    public void BuyStamina() => TryPurchase(playerController, "BuyStaminaUpgrade");
    public void BuyAmmo() => TryPurchase(playerController, "BuyAmmoUpgrade");

    private void TryPurchase(playerController controller, string methodName)
    {
        if (controller == null) return;

        switch (methodName)
        {
            case "BuyHealthUpgrade": controller.BuyHealthUpgrade(); break;
            case "BuyStaminaUpgrade": controller.BuyStaminaUpgrade(); break;
            case "BuyAmmoUpgrade": controller.BuyAmmoUpgrade(); break;
        }

        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay(int currentCoins = -1)
    {
        if (coinText != null)
        {
            int coins = currentCoins >= 0 ? currentCoins : (playerCurrency != null ? playerCurrency.Coins : 0);
            coinText.text = $"Coins: {coins}";
        }
    }

    public void ShowNotEnoughCoins(string itemName)
    {
        if (feedbackText != null)
        {
            feedbackText.text = $"Not enough coins for {itemName}!";
            feedbackText.color = Color.red;
            StartCoroutine(ClearFeedbackAfterDelay());
        }
    }

    private IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (feedbackText != null) feedbackText.text = "";
    }

    private void OnDestroy()
    {
        if (playerCurrency != null)
            playerCurrency.OnCoinsChanged.RemoveListener(UpdateCoinDisplay);
    }
}