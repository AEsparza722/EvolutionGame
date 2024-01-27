using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMoreUpgrade : MonoBehaviour
{
    [SerializeField] public List<RandomSpawnUpgradeData> amountList;
    double acumulatedWeights;
    System.Random random = new System.Random();
    public static SpawnMoreUpgrade instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void CalculateWeights()
    {
        acumulatedWeights = 0;

        foreach (RandomSpawnUpgradeData amount in amountList)
        {
            acumulatedWeights += amount.chance;
            amount.weight = acumulatedWeights;
        }
    }

    int GetRandomAmountIndex()
    {
        double randomTemp = random.NextDouble() * acumulatedWeights;

        for (int i = 0; i < amountList.Count; i++)
        {
            if (amountList[i].weight >= randomTemp)
            {
                return i;
            }
        }
        return 0;

    }

    public RandomSpawnUpgradeData GetRandomAmountData()
    {
        CalculateWeights();
        RandomSpawnUpgradeData block = amountList[GetRandomAmountIndex()];
        return block;
    }
}
