using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossLevel3 : BossLevel2
{
    [Header("Level 3")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float longRange;
    [SerializeField] float cooldownShoot;
    [SerializeField] float bulletSpeed;
    [SerializeField] List<GameObject> virusLongRange = new List<GameObject>();
    GameObject virusFocusLongRange;

    bool canShoot = true;

    private void Awake()
    {
        InitializeBoss();
    }

    private void Update()
    {
        UpdateBoss3();
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        GameObject bulletClone;
        virusLongRange.Clear();
        virusLongRange = GetVirusInRange(longRange);
        
        if (virusLongRange.Count > 0)
        {
            SortVirusList();

            virusFocusLongRange = virusLongRange[0];
            Vector3 direction = (virusFocusLongRange.transform.position - transform.position).normalized;

            bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bulletClone.GetComponent<Rigidbody2D>().AddForce(direction * bulletSpeed, ForceMode2D.Impulse);            
        }
        yield return new WaitForSeconds(cooldownShoot);
        canShoot = true;
    }


    void SortVirusList()
    {
        for (int i = 0; i < virusLongRange.Count - 1; i++)
        {           
            for (int j = 0; j < virusLongRange.Count - i - 1; j++)
            {
                float distance1;
                float distance2;

                distance1 = Vector3.Distance(virusLongRange[j].gameObject.transform.position, transform.position);
                distance2 = Vector3.Distance(virusLongRange[j+1].gameObject.transform.position, transform.position);

                if (distance1 < distance2)
                {
                    GameObject virusTemp = virusLongRange[j];
                    virusLongRange[j] = virusLongRange[j+1];
                    virusLongRange[j + 1] = virusTemp;
                }
            }
        }             
    }
      
    public void UpdateBoss3()
    {
        UpdateBoss();
        if (canShoot)
        {
            StartCoroutine(Shoot());
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, longRange);

    }
}
