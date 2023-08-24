using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float playerHealth;

    [Header("Movement")]
    public float playerSpeed;

    [Header("Combat")]
    public float playerMeleeDamage;
    public float playerMagicDamage;

    public float playerMeleeSpeed;
    public float playerCastSpeed;
    public float playerRangedSpeed;

    [Header("Health and Mana")]
    public float playerMaxHealth;
    public float playerMaxMana;

    public float playerHealthRegen;
    public float playerManaRegen;

}
