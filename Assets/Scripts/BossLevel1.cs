using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel1 : BossController
{
    
    private void Awake()
    {
        InitializeBoss();
    }

    private void Update()
    {
        UpdateBoss();
    }
}
