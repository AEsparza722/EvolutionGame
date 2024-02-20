using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMotherDetection : MonoBehaviour
{
    public List<GameObject> normalVirusDetected = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Virus"))
        {
            if (!normalVirusDetected.Contains(collision.gameObject))
            {
                normalVirusDetected.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Virus"))
        {
            normalVirusDetected.Remove(collision.gameObject);
        }
    }
}
