using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusAreaDetection : MonoBehaviour
{
    public List<GameObject> bossDetected = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("EnemyMother") || collision.transform.CompareTag("EnemyVirus"))
        {
            if (!bossDetected.Contains(collision.gameObject))
            {
                bossDetected.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("EnemyMother") || collision.transform.CompareTag("EnemyVirus"))
        {
            bossDetected.Remove(collision.gameObject);
        }
    }

    public GameObject GetNearbyEnemy()
    {
        for (int i = 0; i < bossDetected.Count - 1; i++)
        {
            for (int j = 0; j < bossDetected.Count - i - 1; j++)
            {
                float distance1;
                float distance2;

                distance1 = Vector3.Distance(bossDetected[j].gameObject.transform.position, transform.position);
                distance2 = Vector3.Distance(bossDetected[j + 1].gameObject.transform.position, transform.position);

                if (distance1 > distance2)
                {
                    GameObject virusTemp = bossDetected[j];
                    bossDetected[j] = bossDetected[j + 1];
                    bossDetected[j + 1] = virusTemp;
                }
            }
        }
        return bossDetected[0];
    }
}
