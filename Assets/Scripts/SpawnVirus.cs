using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirus : MonoBehaviour
{
    [SerializeField] GameObject virusPrefab;
    [SerializeField] float destroySpawnerAfter = 5;


    private void OnEnable()
    {
        StartCoroutine(SpawnVirusSystem.instance.ReturnToPool(destroySpawnerAfter, gameObject));
    }

    private void OnMouseDown()
    {
        Spawn();
    }
    void Spawn()
    {
        Instantiate(virusPrefab, transform.position, Quaternion.identity, VirusManager.instance.transform);
        StartCoroutine(SpawnVirusSystem.instance.ReturnToPool(0, gameObject));
    }



}
