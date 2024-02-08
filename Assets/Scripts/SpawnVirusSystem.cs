using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirusSystem : MonoBehaviour
{
    public static SpawnVirusSystem instance;
    bool canCreateSpawner = true;
    [SerializeField] GameObject VirusSpawner;
    [SerializeField] float cooldown;
    List<GameObject> spawners = new List<GameObject>();
    [SerializeField] int objectsToSpawn;

        

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

        InstanceInitialSpawners();
    }
    private void Update()
    {
        if (canCreateSpawner && !GameManager.instance.isGameOver)
        {
            StartCoroutine(CreateSpawner());
        }
        if (GameManager.instance.isGameOver)
        {
            StopAllCoroutines();
        }
        
    }
    IEnumerator CreateSpawner()
    {
        canCreateSpawner = false;
        //float spawnPosX = Mathf.RoundToInt(Random.Range(-1f, 1f));
        //spawnPosX = spawnPosX == 0 ? -1 : spawnPosX;
        //float spawnPosY = Mathf.RoundToInt(Random.Range(-1f, 1f));
        //spawnPosY = spawnPosY == 0 ? 1 : spawnPosY;

        //Vector2 spawnPos = new Vector2
        //    (
        //    MotherController.instance.transform.position.x + spawnPosX * MotherController.instance.circleCollider.radius * 3f,
        //    MotherController.instance.transform.position.y + spawnPosY * MotherController.instance.circleCollider.radius * 3f
        //    );
        
        
        GameObject spawnerToInstantiate = GetSpawner();
        spawnerToInstantiate.transform.position = MotherController.instance.spawnObjects[0].transform.position;
        
        spawnerToInstantiate.SetActive(true);

        yield return new WaitForSeconds(cooldown);
        canCreateSpawner = true;
    }
    
    public IEnumerator ReturnToPool(float time, GameObject spawner)
    {
        yield return new WaitForSeconds(time);
        
        if (spawner != null)
        {
            spawner.SetActive(false);
        }
    }

    GameObject GetSpawner()
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            if (!spawners[i].activeSelf)
            {
                return spawners[i];
            }
            
        }
        return CreateNewSpawner();
    }

    GameObject CreateNewSpawner()
    {
        GameObject virusSpawerInstance = Instantiate(VirusSpawner, transform.position, Quaternion.identity, transform.parent);
        virusSpawerInstance.SetActive(false);
        spawners.Add(virusSpawerInstance);
        return virusSpawerInstance;
    }

    void InstanceInitialSpawners()
    {
        for (int i = 0; i < objectsToSpawn; ++i)
        {
            GameObject virusSpawerInstance = Instantiate(VirusSpawner, transform.position, Quaternion.identity, transform.parent);
            virusSpawerInstance.SetActive(false);
            spawners.Add(virusSpawerInstance);
        }
    }

}
