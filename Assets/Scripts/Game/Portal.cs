using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    private SpriteRenderer portalSprite;
    private Color portalColor;
    private Color highlightColor;

    private GameObject manager;
    private ManageLevels manageLevels;
    public int nextLevelInt;
    public string nextLevel;
    public int randomMap;


    void Start()
    {
        // randomMap = UnityEngine.Random.Range(1, 5);
        randomMap = 1;
        manager = GameObject.Find("Manager");
        manageLevels = manager.GetComponent<ManageLevels>();
        nextLevelInt = manageLevels.nextLevel;
        nextLevel = "Level" + nextLevelInt.ToString() + randomMap.ToString();
        highlightColor = new Color(0.5f, 0, 0);
        portalColor = new Color(1, 0, 0);
        portalSprite = transform.GetComponent<SpriteRenderer>();
        portalSprite.color = portalColor;
    }

    public void Highlight()
    {
        portalSprite.color = highlightColor;
    }

    public void DeHighlight()
    {
        portalSprite.color = portalColor;
    }

    public void LoadLevel()
    {
        //Load Shop
        if(nextLevelInt == 3 || nextLevelInt == 5)
        {
            if(SceneManager.GetActiveScene().buildIndex == 2)
                SceneManager.LoadScene(3);
            else
                SceneManager.LoadScene(2);
        }
        

        //Else, go to next level
        manageLevels.LoadLevel(nextLevel);
       
    }
}
