using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{

    //Sistema random por pesos

    [SerializeField] List<BlockData> objectList;
    double acumulatedWeights;
    System.Random random = new System.Random();

    private void Start()
    {
        for (int i = 0; i < 100; i++) 
        {
            Debug.Log(GetRandomBlockData().objectName);
        }        
    }
    void CalculateWeights()
    {
        acumulatedWeights = 0;

        foreach (BlockData block in objectList)
        {
            acumulatedWeights += block.chance;
            block.weight = acumulatedWeights;
        }
    }

    int GetRandomBlockIndex()
    {
        double randomTemp = random.NextDouble() * acumulatedWeights;

        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].weight >= randomTemp)
            {
                return i;
            }
        }
        return 0;
      
    }

    BlockData GetRandomBlockData()
    {
        
        CalculateWeights();
        BlockData block = objectList[GetRandomBlockIndex()];
        return block;
    }
}
