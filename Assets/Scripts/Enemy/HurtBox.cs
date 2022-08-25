using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{

    private GameObject enemy;
    public Slime slime;
    private float knockbackX;
    private float knockbackY;
    private int attackDamage;

    void Start()
    {
        knockbackX = slime.knockbackX;
        knockbackY = slime.knockbackY;
        attackDamage = slime.attackDamage;
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
