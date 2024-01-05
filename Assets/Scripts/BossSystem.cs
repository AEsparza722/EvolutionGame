using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSystem : MonoBehaviour
{
    public static BossSystem instance;
    [SerializeField] float requiredScore;
    float currentScore;
    [SerializeField] GameObject BossPrefab;
    public GameObject currentBoss;
    BossController bossController;


    private void Awake()
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

    private void Update()
    {
        if(currentScore >= requiredScore)
        {
            SpawnBoss();
        } 
    }

    public void IncreaseScore(float coinsScore)
    {
        if (currentBoss == null)
        {
            currentScore += coinsScore;
            Debug.Log(currentScore);
        }

    }

    void SpawnBoss()
    {        
        currentBoss = Instantiate(BossPrefab, new Vector2(Random.Range(-4.45f, 4.45f), Random.Range(-8.4f, 8.4f)), Quaternion.identity, transform.parent);
        bossController = currentBoss.GetComponent<BossController>();
        currentScore = 0;
    }

    public void takeDamage(int damage)
    {
        bossController.health -= damage;
                
        //Debug.Log(bossController.health);
        if (bossController.health <= 0)
        {
            Destroy(currentBoss.gameObject);
            currentBoss = null;
            bossController=null; 
        }
    }
}
