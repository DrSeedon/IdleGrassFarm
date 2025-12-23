using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public int redCurrency = 0;
    public int yellowCurrency = 0;

    public void AddRedCurrency(int amount)
    {
        redCurrency += amount;
    }

    public void AddYellowCurrency(int amount)
    {
        yellowCurrency += amount;
    }

    public bool SpendCurrency(int red, int yellow)
    {
        if (redCurrency >= red && yellowCurrency >= yellow)
        {
            redCurrency -= red;
            yellowCurrency -= yellow;
            return true;
        }
        return false;
    }

    public int GetRedCurrency() => redCurrency;
    public int GetYellowCurrency() => yellowCurrency;
}
