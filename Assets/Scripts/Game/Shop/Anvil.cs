using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Anvil : MonoBehaviour
{
    private GameObject player;
    private Player playerscript;
    public PlayerMovement playerMovement;
    public TMP_Text playerCurrency; 
    private int healCost = 5;
    public GameObject interactCanvas;
    public GameObject upgradeUI;

    // Player UI
    public Transform statusBar;
    public ManaBar manaBar;
    public HealthBar healthBar;


    void Start() {
        player = GameObject.Find("PlayerCharacter");
        playerscript = player.GetComponent<Player>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // Get UI Elements
        statusBar = GameObject.Find("UI").transform.GetChild(0);
        manaBar = statusBar.transform.GetChild(3).GetComponent<ManaBar>();
        healthBar = statusBar.transform.GetChild(0).GetComponent<HealthBar>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            if(upgradeUI.activeSelf)
            {
                upgradeUI.SetActive(false);
                interactCanvas.SetActive(true);
            }
    }

    public void InRange()
    {
        interactCanvas.SetActive(true);
    }

    public void OutOfRange()
    {
        interactCanvas.SetActive(false);
        upgradeUI.SetActive(false);
    }

    public void ShowUpgradeUI() {   
        GetPlayerCurrency();                    // Get player currency amount
        upgradeUI.SetActive(true);              // Turn on UI
        interactCanvas.SetActive(false);        // Turn off interact UI
        playerMovement.isPaused = true;         // Freeze player attacks 
        playerMovement.playerCanMove = false;   // Freeze player movement
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);              // Turn off UI
        interactCanvas.SetActive(true);        // Turn on interact UI
        playerMovement.isPaused = false;         // Unfreeze player attacks 
        playerMovement.playerCanMove = true;   // Unfreeze player movement
    }

    public void Upgrade(string upgradeID)
    {
        int cost = 5;
        if(playerscript.coins >= cost)
        {
            Invoke(upgradeID, 0f);
            playerscript.Purchase(cost);
            GetPlayerCurrency();
        }
    }

    // Upgrades

    public void Attack()
    {
        playerscript.baseDamage += 1;
        playerscript.heavyDamage = playerscript.baseDamage + 1;
        playerscript.meleeDamage = playerscript.baseDamage;
        playerscript.rangedDamage = playerscript.baseDamage - 1;
    }

    public void MaxHP()
    {
        playerscript.maxHealth += 2;
        playerscript.currentHealth = playerscript.maxHealth;
        healthBar.SetMaxHealth(playerscript.maxHealth);
    }

    public void MaxMana()
    {
        playerscript.maxMana += 20f;
        manaBar.SetMaxMana(playerscript.maxMana);
    }

    void HealPlayer()
    {
        // If player has enough money, and is not full health
        if(playerscript.coins > 5 && playerscript.currentHealth != playerscript.maxHealth) {
            //Heal player, take payment
            playerscript.Heal();
            playerscript.Purchase(healCost);
        }
    }

    void GetPlayerCurrency()
    {
        playerCurrency.text = "    " + playerscript.coins.ToString();
    }

    void DoNothing()    //Empty script for upgradeID to be assigned until runtime.
    {
        return;
    }
}
