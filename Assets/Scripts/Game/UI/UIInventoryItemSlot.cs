using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIInventoryItemSlot : MonoBehaviour
{
    public Sprite itemImage;
    public TMP_Text itemName;
    public TMP_Text stackSize;

    // void Awake()
    // {
    //     itemImage = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
    //     itemName = this.gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
    //     stackSize = this.gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
    // }
}
