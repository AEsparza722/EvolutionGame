using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetectionRadius : MonoBehaviour
{
    public List<GameObject> virusDetected = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Virus") || collision.transform.CompareTag("Protect"))
        {
            if (!virusDetected.Contains(collision.gameObject))
            {
                virusDetected.Add(collision.gameObject);
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Virus") || collision.transform.CompareTag("Protect"))
        {
            virusDetected.Remove(collision.gameObject);
        }
    }
}

