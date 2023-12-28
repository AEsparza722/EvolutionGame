using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : MonoBehaviour
{
    public static VirusManager instance;
    public List<VirusData> activeVirus;
    public List<VirusData> virusData;
    void Awake()
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

    public VirusData NextVirus(VirusData CurrentVirus)
    {
        CurrentVirus = virusData[CurrentVirus.VirusLevel];
        return CurrentVirus;

    }


}
