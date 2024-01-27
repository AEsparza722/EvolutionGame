using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RandomSpawnUpgradeData : ScriptableObject
{
    public int quantity;
    [Range(0f, 100f)] public float chance;
    [HideInInspector] public double weight;


}