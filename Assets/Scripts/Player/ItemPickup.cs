using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public GameObject player;
    public Player playerScript;
    public PlayerMovement movementScript;
    private GameObject coin;
    public CoinCounter coinCounter;
    public Animator playerAnimator;
    private GameObject gem;
    public InventorySystem inventory;
    // Start is called before the first frame update
    void Start()
    {
        coinCounter.SetCoinCount(playerScript.coins);
        
        
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D item)
    {

        if(item.gameObject.layer == 12)
        {
            var itemPickup = item.GetComponent<Item>();
            var itemData = itemPickup.referenceItem;
            inventory.Add(itemData);
            Destroy(item.gameObject);
        }

        if(item.gameObject.layer == 14)
            GemPickup(item.gameObject);
        
    }

    void GemPickup(GameObject gemItem)
    {
        gem = gemItem;
        var gemCol = gemItem.GetComponent<Collider2D>();
        var rb = gemItem.GetComponent<Rigidbody2D>();
        gemCol.enabled = false;
        rb.gravityScale = 0;
        gemItem.transform.position = new Vector2(player.transform.position.x, player.transform.position.y+1.1f);
        StartCoroutine(HoldPickup());
        Debug.Log("GEM GET!");
    }

    IEnumerator HoldPickup()
    {
        playerAnimator.SetBool("IsPickup", true);
        movementScript.playerCanMove = false;
        movementScript.rigidbody2d.velocity = new Vector2(0, 0);
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(2.5f);
        playerAnimator.SetBool("IsPickup", false);
        Destroy(gem);
        movementScript.playerCanMove = true;
        Time.timeScale = 1f;
    }
}
