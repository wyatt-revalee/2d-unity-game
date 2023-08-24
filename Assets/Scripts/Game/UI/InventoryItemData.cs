using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

// Taken from tutorial by Game Dev Guide on Youtube (https://www.youtube.com/watch?v=SGz3sbZkfkg&t=159s)
// [CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData
{
   public Item item;
   public string displayName;
   public Sprite icon;
   public string id;

   public InventoryItemData(Item newItem, string newName, Sprite newSprite, string NewId)
   {
      item = newItem;
      displayName = newName;
      icon = newSprite;
      id = NewId;
   }
}
