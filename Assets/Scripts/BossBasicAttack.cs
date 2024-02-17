using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBasicAttack : MonoBehaviour
{
    Rigidbody2D rb;
    float maxDistance;
    bool isAttacking;
    float currentDistance = 0;
    Transform initialPosition;
    Transform targetPosition;
    float damage;
    float force;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (targetPosition != null)
        {
            if (isAttacking && IsMaxDistance())
            {
                rb.velocity = Vector2.zero;

                isAttacking = false;
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Transform initialPosition, Transform targetPosition, float maxDistance, float force, float damage)
    {
        isAttacking = true;
        currentDistance = 0;
        Vector2 direction = (targetPosition.position - initialPosition.position).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        this.initialPosition = initialPosition;
        this.targetPosition = targetPosition;
        this.maxDistance = maxDistance;
        this.damage = damage;       
    }

    bool IsMaxDistance()
    {
        currentDistance = Vector2.Distance(initialPosition.position, targetPosition.position);     
        if (currentDistance >= maxDistance) return true; else return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetPosition != null)
        {
            if (collision.gameObject == targetPosition.gameObject)
            {
                collision.gameObject.GetComponent<IDamageable>().takeDamage(damage, 10f);
            }
        }
    }
}
