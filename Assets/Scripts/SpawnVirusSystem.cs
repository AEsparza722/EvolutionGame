using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVirusSystem : MonoBehaviour
{
    bool canCreateSpawner = true;
    [SerializeField] GameObject VirusSpawner;
    [SerializeField] float cooldown;
    

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
        Instantiate(VirusSpawner, new Vector2(Random.Range(-GameManager.instance.gameArea.x/2, GameManager.instance.gameArea.x/2), Random.Range(-GameManager.instance.gameArea.y/2, GameManager.instance.gameArea.y/2)), Quaternion.identity, transform.parent);
        yield return new WaitForSeconds(cooldown);
        canCreateSpawner = true;
    }

}
