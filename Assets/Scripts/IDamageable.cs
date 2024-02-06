using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float health { get; set; }

    public void takeDamage(float damage, float force);
    public IEnumerator takeDamageOverTime(float damage, float times, float seconds);
    public IEnumerator KnockBack(float force);
    public IEnumerator ChangeColorDamage();
}
