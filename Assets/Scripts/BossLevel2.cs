using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel2 : BossLevel1
{
    bool canPushAttack = true;
    private void Awake()
    {
        InitializeBoss();
    }

    private void Update()
    {
        UpdateBoss();
        if (canPushAttack)
        {
            StartCoroutine(PushAttack());
        }
        
    }

    IEnumerator PushAttack()
    {
        canPushAttack = false;
        List<GameObject> detectedVirus = new List<GameObject>();
        detectedVirus = GetVirusInRange(attackRadius);

        for (int i = 0; i < detectedVirus.Count; i++)
        {
            detectedVirus[i].GetComponent<CharacterControler>().takeDamage(damage / 3, 20f);
        }

        yield return new WaitForSeconds(cooldownBasicAttack * 2);
        canPushAttack=true;

    }

    
}
