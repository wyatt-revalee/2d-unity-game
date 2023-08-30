using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{

    public Item item;
    public Items itemDrop;

    // Start is called before the first frame update
    void Start()
    {
        item = AssignItem(itemDrop);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.parent.gameObject.GetComponent<Player>();
            player.inventory.Add(new InventoryItemData(item, item.GiveName(), this.GetComponent<SpriteRenderer>().sprite, item.GiveId(), item.IsBuff(), item.GiveBuffTime()));
            Destroy(gameObject);
        }
    }

    public Item AssignItem(Items itemToAssign)
    {
        switch (itemToAssign)
        {
            case Items.Coin:
                return new Coin();
            case Items.HealingItem:
                return new HealingItem();
            case Items.FireDamageItem:
                return new FireDamageItem();
            default:
                return new HealingItem();
        }
    }
}

public enum Items
{
    Coin,
    HealingItem,
    FireDamageItem
}