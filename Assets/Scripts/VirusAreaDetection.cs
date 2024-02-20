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
}
