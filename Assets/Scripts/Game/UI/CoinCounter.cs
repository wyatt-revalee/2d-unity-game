using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    public Player player;
    public TMP_Text coinCount;
    public InventorySystem inventory;
    public InventoryItemData coin;

    public void SetCoinCount(int coins) {
        coinCount = GetComponent<TMP_Text>();
        coinCount.text = " x" + coins.ToString();

    }

}
