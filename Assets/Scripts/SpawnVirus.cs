using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirus : MonoBehaviour
{
    [SerializeField] GameObject virusPrefab;
    [SerializeField] float destroySpawnerAfter = 5;


    private void Update()
    {
        Invoke("DestroySpawner", destroySpawnerAfter);
    }

    private void OnMouseDown()
    {
        Spawn();
    }
    void Spawn()
    {
        Instantiate(virusPrefab, transform.position, Quaternion.identity, VirusManager.instance.transform);
        Destroy(gameObject);        
    }

    void DestroySpawner()
    {
        Destroy(gameObject);
        //Debug.Log("Spawner expired");
    }

}
