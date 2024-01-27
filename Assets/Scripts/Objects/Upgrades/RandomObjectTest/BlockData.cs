using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockData : ScriptableObject
{    
    public string objectName;
    [Range(0f, 100f)] public float chance;
    [HideInInspector] public double weight;

}
