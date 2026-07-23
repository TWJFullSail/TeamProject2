using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;
    public TextMeshProUGUI coinText;

    playerController player;

    void Start()
    {
        player = FindObjectOfType<playerController>();
        shopPanel.SetActive(false);
    }

    public bool IsOpen => shopPanel.activeSelf;

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void BuyHealth() => player.BuyHealthUpgrade();
    public void BuyStamina() => player.BuyStaminaUpgrade();
    public void BuyAmmo() => player.BuyAmmoUpgrade();
}