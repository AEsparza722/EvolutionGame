using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel2 : BossLevel1
{
    [Header("Level 2")]
    bool canPushAttack = true;
    private void Awake()
    {
        InitializeBoss();
    }

    private void Update()
    {
        UpdateBoss();       
        
    }

    IEnumerator PushAttack()
    {
        canPushAttack = false;
        List<GameObject> detectedVirus = new List<GameObject>();
        detectedVirus = GetVirusInRange(attackRadius);

        for (int i = 0; i < detectedVirus.Count; i++)
        {
            detectedVirus[i].GetComponent<CharacterControler>().takeDamage(damage / 3f, 20f);
        }

        yield return new WaitForSeconds(cooldownBasicAttack * 2);
        canPushAttack=true;

    }

    public override void UpdateBoss()
    {
        base.UpdateBoss();
        if (canPushAttack)
        {
            StartCoroutine(PushAttack());
        }
    }


}
