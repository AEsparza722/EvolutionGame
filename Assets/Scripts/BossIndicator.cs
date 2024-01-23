using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    [SerializeField] GameObject target;
    bool isIndicatorActive = true;
    SpriteRenderer spriteRenderer;
    
    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isIndicatorActive)
        {
            Vector2 direction = target.transform.position - transform.position ;
            Vector2 targetViewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);

            if (targetViewportPosition.x > 0f && targetViewportPosition.x < 1f && targetViewportPosition.y > 0f && targetViewportPosition.y < 1f)
            {
                spriteRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = true;

                //Position
                Vector3 screenEgde = Camera.main.ViewportToWorldPoint(new Vector3(Mathf.Clamp(targetViewportPosition.x, 0.05f, 0.95f), Mathf.Clamp(targetViewportPosition.y, 0.1f, 0.9f), Camera.main.nearClipPlane));
                transform.position = screenEgde;

                //Rotation
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0,0,angle - 90);
            }
        }
    }

}
