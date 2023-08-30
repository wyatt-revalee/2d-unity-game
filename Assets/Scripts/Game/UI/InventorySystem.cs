using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Code partially taken from tutorial by Game Dev Guide on Youtube (https://www.youtube.com/watch?v=SGz3sbZkfkg&t=159s)

public class InventorySystem : MonoBehaviour
{
    public static GameObject Instance;

    public Player player;
    public Dictionary<string, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory {get; private set; }
    public GameObject inventoryWrapper;
    public GameObject slotPrefab;
    public CoinCounter coinCounter;
    public GameObject gemSlotter;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<string, InventoryItem>();
        StartCoroutine(CallItemUpdate());
    }

    IEnumerator CallItemUpdate()
    {
        foreach (InventoryItem i in inventory)
        {
            i.data.item.UpdatePlayer(player, i.stackSize);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(CallItemUpdate());
    }

    public void CallItemOnHit(IDamageable enemy)
    {
        foreach (InventoryItem i in inventory)
        {
            i.data.item.OnHit(player, enemy, i.stackSize);
        }
    }

    public void CallDebuffOnHit(IDamageable enemy)
    {
        foreach (InventoryItem i in inventory)
        {
            if (i.data.item.IsBuff())
            {
                enemy.AddBuff(i.data);
            }
        }
    }



    public void Add(InventoryItemData referenceData)
    {   
        if(m_itemDictionary.TryGetValue(referenceData.id, out InventoryItem value))
        {
            value.AddToStack();
            if (referenceData.id == "coin")
            {
                coinCounter.SetCoinCount(value.stackSize);
                player.coinCount += 1;
            }
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData.id, newItem);

            if (referenceData.id == "coin")
            {
                coinCounter.SetCoinCount(1);
                player.coinCount = 1;
            }
        }
    }

    public void Remove(InventoryItemData referenceData)
    {
        if(m_itemDictionary.TryGetValue(referenceData.id, out InventoryItem value))
        {
            value.RemoveFromStack();

            if(value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData.id);
            }
        }
    }

    public void AddGem(int slotNum)
    {
        GemSlot gem = gemSlotter.transform.GetChild(slotNum).GetComponent<GemSlot>();
        gem.AddGem();
    }


    public void DrawInventory()
    {

        // Clear Inventory first
        foreach(Transform child in inventoryWrapper.transform){
            Destroy(child.gameObject);
        }
        // Debug.Log("Drawing Inventory");
        foreach(InventoryItem item in inventory)
        {
            AddInventorySlot(item);
            // Debug.Log(item.data.displayName);
        }
    }

    public void AddInventorySlot(InventoryItem item)
    {
        GameObject slot = Instantiate(slotPrefab); //Intialize new slot

        // Grab the image, name, and stack size of the item, set them to the UI
        var image = slot.transform.GetChild(0).gameObject.GetComponent<Image>();
        image.sprite = item.data.icon;

        var itemText = slot.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        itemText.text = item.data.displayName;

        var itemStack = slot.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
        itemStack.text = item.stackSize.ToString();

        // Add new slot to inventory
        slot.transform.SetParent(inventoryWrapper.transform);

    }

}

[System.Serializable]
public class InventoryItem
{
    public InventoryItemData data {get; private set; }
    public int stackSize { get; set; }
    public int itemBuffTime {get; set;}

    public InventoryItem(InventoryItemData source)
    {
        data = source;
        itemBuffTime = source.buffTime;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }

    public void DecBuffTime()
    {
        itemBuffTime--;
    }
}