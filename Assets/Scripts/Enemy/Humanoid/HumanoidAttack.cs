using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAttack : MonoBehaviour
{

    private GameObject playerEnemy;
    public Humanoid humanoid;
    private float knockbackX;
    private float knockbackY;
    private int attackDamage;

    void Start()
    {
        knockbackX = humanoid.knockbackX;
        knockbackY = humanoid.knockbackY;
        attackDamage = humanoid.attackDamage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 10)
            playerEnemy = other.transform.parent.gameObject;
        else
            playerEnemy = null;
        
        if(playerEnemy != null)
        {
            var knockback = playerEnemy.GetComponent<IKnockbackable>();
            var hit = playerEnemy.GetComponent<IDamageable>();

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
