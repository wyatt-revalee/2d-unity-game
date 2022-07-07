using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Player player;

    float knockbackX; 
    float knockbackY;
    int attackDamage;

    void Start()
    {
        knockbackX = player.knockbackX;
        knockbackY = player.knockbackY;
        attackDamage = player.attackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        var hit = other.GetComponent<IDamageable>();
        var knockback = other.GetComponent<IKnockbackable>();


        if (hit != null)
        {
            // hit.Damage(attackDamage);
        }

        if (knockback != null)
        {
            knockback.Knockback(knockbackX, knockbackY, this.transform);
        }
    }
}
