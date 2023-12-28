using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "New Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string Name;
    public float Cost;
    public int Level;
    public float CostMultiplier;
    public Sprite Icon;
    public string Description;
    public float CurrentEffect;
    public float MultiplierEffect;    
}
