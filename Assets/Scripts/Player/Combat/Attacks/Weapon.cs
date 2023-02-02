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
    private int critChance;

    void Start()
    {
        knockbackX = player.knockbackX;
        knockbackY = player.knockbackY;
        critChance = 30;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.layer == 11 || other.gameObject.layer == 15)
            enemy = other.transform.parent.gameObject;
        else
            enemy = null;

        if (enemy != null)
        {
            var hit = enemy.GetComponent<IDamageable>();
            var knockback = enemy.GetComponent<IKnockbackable>();


            if (hit != null)
            {
                attackDamage = player.attackDamage;
                bool isCritical = UnityEngine.Random.Range(0, 100) < critChance;
                if(isCritical)
                    attackDamage *= 2;
                hit.Damage(attackDamage);
                DamagePopup.Create(enemy.transform.position, attackDamage, isCritical);
            }

            if (knockback != null)
            {
                knockback.Knockback(knockbackX, knockbackY, player.transform);
            }
        }
    }
}
