using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(int damage);

    void AddBuff(InventoryItemData itemData);

    void RemoveBuff(InventoryItemData itemData);
}
