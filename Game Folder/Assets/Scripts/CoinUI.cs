using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;

    private PlayerCurrency playerCurrency;

    private void Awake()
    {
        playerCurrency = FindAnyObjectByType<PlayerCurrency>();

        if (playerCurrency == null)
        {
            Debug.LogError("CoinUI: PlayerCurrency not found in scene!");
        }
    }

    private void OnEnable()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnCoinsChanged.AddListener(UpdateCoinUI);

            UpdateCoinUI(playerCurrency.Coins);
        }
    }
    private void OnDisable()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnCoinsChanged.RemoveListener(UpdateCoinUI);
        }
    }

    private void UpdateCoinUI(int currentCoins)
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
}