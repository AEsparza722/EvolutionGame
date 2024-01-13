using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel4 : BossLevel3
{
    [Header("Level 4")]
    bool canPoison = true;
    [SerializeField] float cooldownPoison = 5;
    
    private void Awake()
    {
        InitializeBoss();
    }

    private void Update()
    {
        UpdateBoss4();
    }

    IEnumerator PoisonAttack()
    {
        canPoison = false;
        List <GameObject> virusDetected = GetVirusInRange(attackRadius);

        if(virusDetected.Count > 1)
        {
            StartCoroutine(virusDetected[0].GetComponent<CharacterControler>().takeDamageOverTime(damage / 5, 10, 0.5f));
            StartCoroutine(virusDetected[1].GetComponent<CharacterControler>().takeDamageOverTime(damage / 5, 10, 0.5f));
            Debug.Log("bien");
        }
        else if(virusDetected.Count > 0)
        {
            StartCoroutine(virusDetected[0].GetComponent<CharacterControler>().takeDamageOverTime(damage / 5, 10, 0.5f)); 
            Debug.Log("bien");
        }
        else
        {
            Debug.Log("perrilla");
        }
        

        yield return new WaitForSeconds(cooldownPoison);
        canPoison = true;
    }



    void UpdateBoss4()
    {
        UpdateBoss3();
        if (canPoison)
        {
            StartCoroutine(PoisonAttack());
        }
    }
}
