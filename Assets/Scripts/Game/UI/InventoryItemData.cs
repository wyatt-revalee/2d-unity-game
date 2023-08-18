using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Taken from tutorial by Game Dev Guide on Youtube (https://www.youtube.com/watch?v=SGz3sbZkfkg&t=159s)
[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
   public string id;
   public string displayName;
   public Sprite icon;
}
