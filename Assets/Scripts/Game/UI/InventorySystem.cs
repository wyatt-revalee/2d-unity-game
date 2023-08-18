using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Code partially taken from tutorial by Game Dev Guide on Youtube (https://www.youtube.com/watch?v=SGz3sbZkfkg&t=159s)

public class InventorySystem : MonoBehaviour
{
    private static GameObject Instance;

    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory {get; private set; }
    public GameObject inventoryWrapper;
    public GameObject slotPrefab;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public void Add(InventoryItemData referenceData)
    {
        if(m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }
    }

    public void Remove(InventoryItemData referenceData)
    {
        if(m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if(value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
    }

    void Start()
    {
        gameObject.SetActive(true);
        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void DrawInventory()
    {

        // Clear Inventory first
        foreach(Transform child in inventoryWrapper.transform){
            Destroy(child.gameObject);
        }
        Debug.Log("Drawing Inventory");
        foreach(InventoryItem item in inventory)
        {
            AddInventorySlot(item);
            // Debug.Log(item.data.displayName);
        }
    }

    public void AddInventorySlot(InventoryItem item)
    {
        GameObject slot = Instantiate(slotPrefab); //Intialize new slot

        // Grab the image of the slot, set to the item's icon
        var image = slot.transform.GetChild(0).gameObject.GetComponent<Image>();
        image.sprite = item.data.icon;

        var itemText = slot.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        itemText.text = item.data.displayName;

        var itemStack = slot.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
        itemStack.text = item.stackSize.ToString();

        slot.transform.SetParent(inventoryWrapper.transform);




        // itemImage = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        //     itemName = this.gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        //     stackSize = this.gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
    }

}

[System.Serializable]
public class InventoryItem
{
    public InventoryItemData data {get; private set; }
    public int stackSize { get; private set; }

    public InventoryItem(InventoryItemData source)
    {
        data = source;
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
}