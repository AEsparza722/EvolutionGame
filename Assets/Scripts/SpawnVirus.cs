using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirus : MonoBehaviour
{
    [SerializeField] GameObject virusPrefab;


    private void Update()
    {
        
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

}
