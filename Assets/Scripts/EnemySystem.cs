using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    float currentTime;
    [SerializeField] float timeToSpawn;
    [SerializeField] GameObject motherVirus;

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= timeToSpawn)
        {
            SpawnMother();
        }
    }

    void SpawnMother() 
    {
        currentTime = 0;
        Instantiate(motherVirus, new Vector2(Random.Range(-GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2), Random.Range(-GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2)),Quaternion.identity,transform);
    }
}
