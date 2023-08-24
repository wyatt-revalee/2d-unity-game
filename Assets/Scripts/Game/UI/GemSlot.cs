using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemSlot : MonoBehaviour
{
    public Image gemImage;

    // // Dictionary for assigning colors to objects respective to world
    public Dictionary<string, Color32> worldColors = new Dictionary<string, Color32>()
    {
        {"Red", new Color32(255, 0, 0, 255)},       // Red
        {"Orange", new Color32(255, 125, 42, 255)},    // Orange
        {"Yellow", new Color32(255, 201, 0, 255)},       // Yellow
        {"Green", new Color32(0, 255, 0, 255)},       // Green
        {"Blue", new Color32(0, 245, 255, 255)},       // Blue
        {"Purple", new Color32(113, 0, 255, 255)},       // Purple
    };

    public void AddGem() 
    {
        var colorName = this.gameObject.name.Substring(0, this.gameObject.name.Length-3);
        gemImage = this.gameObject.GetComponent<Image>();
        gemImage.color = worldColors[colorName];
    }
}
