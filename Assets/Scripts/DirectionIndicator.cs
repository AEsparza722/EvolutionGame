using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DirectionIndicator : MonoBehaviour
{
    GameObject currentBoss;
    [SerializeField] GameObject directionIndicator;
    [SerializeField] float maxDistance;
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = directionIndicator.GetComponent<RectTransform>();
        
    }
    private void Update()
    {
        currentBoss = BossSystem.instance.currentBoss;
        
        if (currentBoss != null)
        {
            Vector2 bossDirection = (transform.position - currentBoss.transform.position).normalized;
            directionIndicator.SetActive(true);
            rectTransform.anchoredPosition = -bossDirection * maxDistance;

            float angle = Mathf.Atan2(bossDirection.y, bossDirection.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle + 90);

        }
    }

}
