using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirusSystem : MonoBehaviour
{
    bool canCreateSpawner = true;
    [SerializeField] GameObject VirusSpawner;
    [SerializeField] int cooldown;

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
        Instantiate(VirusSpawner, new Vector2(Random.Range(-4.45f, 4.45f), Random.Range(-8.4f, 8.4f)), Quaternion.identity, transform.parent);
        yield return new WaitForSeconds(cooldown);
        canCreateSpawner = true;
    }

}
