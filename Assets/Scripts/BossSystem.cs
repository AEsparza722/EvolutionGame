using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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
    float countDown;
    [SerializeField] float maxCountdown;
    [SerializeField] TMP_Text countDownText;
    float countDownTextScale = 1.2f;
    
    
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
            StartCoroutine(SpawnBoss());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(SpawnBoss());
        }
    }

    public void IncreaseScore(float coinsScore)
    {
        if (currentBoss == null)
        {
            currentScore += coinsScore;
            //Debug.Log(currentScore);
        }

    }

    IEnumerator SpawnBoss()
    {
        countDown = maxCountdown;
        currentScore = 0;
        countDownText.gameObject.SetActive(true);
        BossCountdownAnimation();
        while (countDown > 0)
        {
            countDown -= 0.10f;
            countDown = Mathf.Clamp(countDown, 0, countDown);
            countDownText.text = "BOSS INCOMING IN " + countDown.ToString("F1");
            yield return new WaitForSeconds(.10f);
            
        }

        countDownText.gameObject.SetActive(false);
        currentBoss = Instantiate(BossPrefab, new Vector2(Random.Range(-GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2), Random.Range(-GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2)), Quaternion.identity, transform.parent);
        bossController = currentBoss.GetComponent<BossController>();
        bossController.health *= levelMultiplier;
        bossController.damage *= levelMultiplier;
        bossController.speed *= levelMultiplier;        
        requiredScore *= 1.3f;
        yield return null;
    }

   
    void IncreaseBossLevel()
    {
        bossesKilled = 0;
        neededForNextLevel += 1;
        currentLevel++;
        BossPrefab = bossList[currentLevel - 1];
        levelMultiplier = 1f;

    }

    public void BossKilled()
    {
        BossController prefabBossController = bossList[currentLevel - 1].GetComponent<BossController>();

        bossesKilled++;
        levelMultiplier += .05f;

        if (bossesKilled == neededForNextLevel)
        {
            IncreaseBossLevel();
        }
    }

    void BossCountdownAnimation()
    {
        LeanTween.scale(countDownText.rectTransform, countDownTextScale * Vector3.one, .5f).setEaseInOutSine().setOnComplete(() => 
        {
            LeanTween.scale(countDownText.rectTransform, Vector3.one, .5f).setEaseInOutSine().setOnComplete(() =>
            {
                BossCountdownAnimation();
            });
        });
    }
}
