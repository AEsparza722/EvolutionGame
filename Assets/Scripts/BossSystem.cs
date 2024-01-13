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
    [SerializeField] int neededForNextLevel;
    [SerializeField] int bossesKilled = 0;
    [SerializeField] float levelMultiplier;
    int currentLevel;
    [SerializeField] List<GameObject> bossList;
    
    
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

    private void Start()
    {
        currentLevel = 1;
        BossPrefab = bossList[currentLevel - 1];
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
        currentBoss = Instantiate(BossPrefab, new Vector2(Random.Range(-GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2), Random.Range(-GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2)), Quaternion.identity, transform.parent);
        bossController = currentBoss.GetComponent<BossController>();
        bossController.health *= levelMultiplier;
        bossController.damage *= levelMultiplier;
        bossController.speed *= levelMultiplier;

        currentScore = 0;

    }

    public void takeDamage(int damage)
    {
        bossController.health -= damage;
                
        //Debug.Log(bossController.health);
        if (bossController.health <= 0)
        {
            BossController prefabBossController = bossList[currentLevel - 1].GetComponent<BossController>();
            Destroy(currentBoss.gameObject);
            currentBoss = null;
            bossController=null; 
            bossesKilled++;
            levelMultiplier += .05f;
                       

            if (bossesKilled == neededForNextLevel)
            {
                IncreaseBossLevel();
            }
        }
    }

    void IncreaseBossLevel()
    {
        bossesKilled = 0;
        neededForNextLevel += 1;
        currentLevel++;
        BossPrefab = bossList[currentLevel - 1];
        levelMultiplier = 1f;

    }
}
