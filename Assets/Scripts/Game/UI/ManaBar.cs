using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{


    public Slider slider;
    public Image fill;
    public Player player;

    // public void FixedUpdate()
    // {
    //     if(player.currentMana < player.maxMana)
    //         player.currentMana += 0.01f;
    //     SetMana(player.currentMana);
    // }

    public void SetMaxMana(float mana) {
        slider.maxValue = mana;
        slider.value = mana;

    }


   public void SetMana(float mana) {
        slider.value = mana;

   }

}
