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
        if (canCreateSpawner)
        {
            StartCoroutine(CreateSpawner());
        }
        
    }
    IEnumerator CreateSpawner()
    {
        canCreateSpawner = false;
        GameObject spawnerToInstantiate = GetSpawner();
        spawnerToInstantiate.transform.position = new Vector2(Random.Range(-GameManager.instance.gameArea.x / 2, GameManager.instance.gameArea.x / 2), Random.Range(-GameManager.instance.gameArea.y / 2, GameManager.instance.gameArea.y / 2));
        spawnerToInstantiate.SetActive(true);

       // Instantiate(VirusSpawner, new Vector2(Random.Range(-GameManager.instance.gameArea.x/2, GameManager.instance.gameArea.x/2), Random.Range(-GameManager.instance.gameArea.y/2, GameManager.instance.gameArea.y/2)), Quaternion.identity, transform.parent);
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
