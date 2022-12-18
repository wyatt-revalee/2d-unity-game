using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : MonoBehaviour
{
    private GameObject player;
    private Player playerscript;
    private int healCost = 5;

    void Start() {
        player = GameObject.Find("PlayerCharacter");
        playerscript = player.GetComponent<Player>();

    }

    public void HealPlayer() {
        // If player has enough money, and is not full health
        if(playerscript.coins > 5 && playerscript.currentHealth != playerscript.maxHealth) {
            //Heal player, take payment
            playerscript.Heal(healCost);
        }

    }
}