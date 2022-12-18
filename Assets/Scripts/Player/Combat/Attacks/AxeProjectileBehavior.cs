using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeProjectileBehavior : MonoBehaviour
{

    public Player player;
    public float Speed = 20f;
    private GameObject enemy;
    int attackDamage;
    public int critChance = 30;

    void Update()
    {
        transform.position += transform.right *Time.deltaTime * Speed;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 11 || other.gameObject.layer == 8)
            enemy = other.gameObject;
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
                knockback.Knockback(player.knockbackX, player.knockbackY, player.transform);
            }
        }
        Destroy(gameObject);
    }

}
