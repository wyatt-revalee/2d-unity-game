using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    public int iLevelToLoad;
    public string sLevelToLoad;
    public bool useIntegerToLoadLevel = false;

    private SpriteRenderer portalSprite;
    private Color portalColor;
    private Color highlightColor;


    void Start()
    {
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

    public void LoadScene()
    {
        if(useIntegerToLoadLevel)
        {
            SceneManager.LoadScene(iLevelToLoad);
        }
        else{
            SceneManager.LoadScene(sLevelToLoad);

        }
    }
}
