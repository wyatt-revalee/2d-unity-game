using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public Player player;
    private GameObject coin;
    public CoinCounter coinCounter;
    // Start is called before the first frame update
    void Start()
    {
        coinCounter.SetCoinCount(player.coins);
        
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.layer == 12)
        {
            Destroy(other.gameObject);
            player.coins += 1;
            coinCounter.SetCoinCount(player.coins);
        }
        
    }
}