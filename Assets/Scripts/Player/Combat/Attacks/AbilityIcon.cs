using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{

    public ManageLevels managerScript;
    public Slider slider;
    public Image fill;
    public float cooldown;
    public float currentCD;
    public bool abilityReady = true;


    void Update()
    {
        if(currentCD >= cooldown)
        {
            abilityReady = true;
        }
        else
        {
            currentCD = currentCD + Time.deltaTime;
            abilityReady = false;
        }

        slider.value = currentCD / cooldown;
        if(slider.value == slider.maxValue)
            fill.color = managerScript.worldColors[managerScript.world];;
    }

    public void StartCooldown()
    {
        currentCD = 0f;
        fill.color = new Color32(255, 255, 255, 123);
    }

}
