using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatHitBox : MonoBehaviour
{

    private GameObject enemy;
    public Bat bat;
    private float knockbackX;
    private float knockbackY;
    private int attackDamage;

    void Start()
    {
        knockbackX = bat.knockbackX;
        knockbackY = bat.knockbackY;
        attackDamage = bat.attackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 10)
            enemy = other.transform.parent.gameObject;
        else
            enemy = null;
        
        if(enemy != null)
        {
            var knockback = enemy.GetComponent<IKnockbackable>();
            var hit = enemy.GetComponent<IDamageable>();

            if (hit != null)
            {
                hit.Damage(attackDamage);
            }

            if (knockback != null)
            {
                knockback.Knockback(knockbackX, knockbackY, this.transform);
            }
        }
        
    }
}
