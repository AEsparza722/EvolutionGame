using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Virus", menuName = "New Virus")]
public class VirusData : ScriptableObject
{
    public string Name;
    public int Health;
    public Sprite Icon;
    public Mesh VirusMesh; //Added
    public float Speed;
    public int VirusLevel;
    public int Damage;
    public int Coins;
    public float AttackRadius;
  
}
