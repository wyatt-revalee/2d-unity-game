using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code partially taken from tutorial by Game Dev Guide on Youtube (https://www.youtube.com/watch?v=SGz3sbZkfkg&t=159s)

public class Inventory : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private static GameObject Instance;

    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory {get; private set; }

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
        gameObject.SetActive(false);
    }

    public void Close() {
        //Close Inventory, resume game
        Time.timeScale = 1f;
        gameObject.transform.parent.gameObject.SetActive(false);
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