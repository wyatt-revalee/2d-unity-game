// Made with help from https://www.youtube.com/watch?v=iU6mKyQjOYI

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{

    public abstract string GiveName();
    public abstract string GiveId();
    public virtual void Update(Player player, int stacks)
    {

    }

    public virtual void OnHit(Player player, IDamageable enemy, int stacks)
    {

    }
}

public class Coin : Item
{
    public override string GiveName()
    {
        return "Coin";
    }
    public override string GiveId()
    {
        return "coin";
    }
}

public class HealingItem : Item
{
    public override string GiveName()
    {
        return "Healing Item";
    }
    public override string GiveId()
    {
        return "healing_item";
    }

    public override void Update(Player player, int stacks)
    {
        player.currentHealth += 1*stacks;
        player.healthBar.SetHealth(player.currentHealth);
    }
}

public class FireDamageItem : Item
{
    public override string GiveName()
    {
        return "Fire Damage Item";
    }

    public override string GiveId()
    {
        return "fire_damage_item";
    }

    public override void OnHit(Player player, IDamageable enemy, int stacks)
    {
        enemy.Damage(1 * stacks);
    }
}