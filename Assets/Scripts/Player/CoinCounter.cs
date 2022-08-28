using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinCounter : MonoBehaviour
{

    public TMP_Text coinCount;
    public Player player;

    public void SetCoinCount(int coins) {
        coinCount = GetComponent<TMP_Text>();
        coinCount.text = "  x" + player.coins.ToString();
    }

}
