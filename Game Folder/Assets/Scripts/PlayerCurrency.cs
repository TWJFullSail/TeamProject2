using UnityEngine;
using UnityEngine.Events;

public class PlayerCurrency : MonoBehaviour
{
    public int coins = 0;
    public UnityEvent<int> OnCoinsChanged;

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            OnCoinsChanged?.Invoke(coins);
            return true;
        }
        return false;
    }
}