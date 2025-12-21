using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public int money = 0;

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Money: +{amount} (Total: {money})");
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }

    public int GetMoney()
    {
        return money;
    }
}
