using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;
    public InventorySystem inventory;

    public void OnHandlePickupItem()
    {
        inventory.Add(referenceItem);
        // Destroy(gameObject); Not necessary since other scripts do this
    }
}
