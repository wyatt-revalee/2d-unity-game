using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Player player;
    private GameObject enemy;

    private float knockbackX; 
    private float knockbackY;
    private int attackDamage;

    void Start()
    {
        knockbackX = player.knockbackX;
        knockbackY = player.knockbackY;
        attackDamage = player.attackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.layer == 11)
            enemy = other.transform.parent.gameObject;
        else
            enemy = null;

        if (enemy != null)
        {
            var hit = enemy.GetComponent<IDamageable>();
            var knockback = enemy.GetComponent<IKnockbackable>();


            if (hit != null)
            {
                hit.Damage(attackDamage);
            }

            if (knockback != null)
            {
                knockback.Knockback(knockbackX, knockbackY, player.transform);
            }
        }
    }
}
