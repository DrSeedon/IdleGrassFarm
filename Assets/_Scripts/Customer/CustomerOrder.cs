using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CustomerOrder
{
    public Dictionary<GrassType, int> requirements = new Dictionary<GrassType, int>();

    public void GenerateRandomOrder()
    {
        requirements.Clear();

        GrassType[] allTypes = (GrassType[])System.Enum.GetValues(typeof(GrassType));
        GrassType randomType = allTypes[Random.Range(0, allTypes.Length)];
        int stackCount = Random.Range(1, 4);

        requirements[randomType] = stackCount;
    }

    public bool IsFulfilled(Dictionary<GrassType, int> available)
    {
        foreach (var req in requirements)
        {
            if (!available.ContainsKey(req.Key) || available[req.Key] < req.Value)
            {
                return false;
            }
        }
        return true;
    }

    public int GetTotalStackCount()
    {
        return requirements.Values.Sum();
    }
}

public enum CustomerState
{
    Wandering,
    MovingToQueue,
    InQueue,
    Buying,
    Leaving
}
