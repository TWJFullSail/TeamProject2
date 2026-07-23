using UnityEngine;
using TMPro;
using System.Collections;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Style")]
    [SerializeField] private Color normalColor = new Color(1f, 0.9f, 0.3f); // Gold
    [SerializeField] private Color flashColor = Color.white;

    private PlayerCurrency playerCurrency;
    private Coroutine flashRoutine;

    void Awake()
    {
        playerCurrency = FindAnyObjectByType<PlayerCurrency>();

        if (coinText != null)
        {
            coinText.fontSize = 48;
            coinText.color = normalColor;
            coinText.alignment = TextAlignmentOptions.Right;
        }
    }

    void OnEnable()
    {
        if (playerCurrency != null)
        {
            playerCurrency.OnCoinsChanged.AddListener(UpdateCoinUI);
            UpdateCoinUI(playerCurrency.Coins);
        }
    }

    void OnDisable()
    {
        if (playerCurrency != null)
            playerCurrency.OnCoinsChanged.RemoveListener(UpdateCoinUI);
    }

    private void UpdateCoinUI(int amount)
    {
        if (coinText != null)
        {
            coinText.text = amount.ToString("000");

            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashEffect());
        }
    }

    private IEnumerator FlashEffect()
    {
        coinText.color = flashColor;
        yield return new WaitForSeconds(0.18f);
        coinText.color = normalColor;
    }
}