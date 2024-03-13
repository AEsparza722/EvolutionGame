using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public static EnemySystem instance;
    float currentTime;
    [SerializeField] float timeToSpawn;
    [SerializeField] GameObject motherVirus;
    [SerializeField] int levelVirus;
    int killedMothers;

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
        currentTime += Time.deltaTime;
        if (currentTime >= timeToSpawn)
        {
            SpawnMother();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnMother();
        }
    }

    void SpawnMother() 
    {        
        currentTime = 0;
        GameObject motherVirusInstance = Instantiate(motherVirus, new Vector2(Random.Range(-GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2), Random.Range(-GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2)),Quaternion.identity,transform);
        motherVirusInstance.GetComponent<EnemyMotherController>().currentLevel = levelVirus;
    }

    public void IncreaseKilledMothers()
    {
        killedMothers++;
        if(killedMothers >= 3) 
        {
            levelVirus++;
            killedMothers = 0;
        }
    }

}
