using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeCounter : MonoBehaviour
{

    public TMP_Text lifeCount;
    public SpriteRenderer playerEyes;
    public Image lifeIcon;

    public void SetLives(int lives) {
        lifeCount = GetComponent<TMP_Text>();
        lifeCount.text = lives.ToString();
        if(lives == 1) {
            lifeCount.color = new Color32(255, 0, 0, 255);
            playerEyes.color = new Color32(255, 0, 0, 255);
            lifeIcon.color = new Color32(255, 0, 0, 255);
        }
    }

}
