using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    private SpriteRenderer portalSprite;
    private Color32 portalColor;
    private Color highlightColor;
    public GameObject sceneTransParent;
    public Animator sceneTransition;
    public int currentWorld;
    public GameObject interactCanvas;

    private GameObject manager;
    private ManageLevels manageLevels;
    public int nextLevelInt;
    public string nextLevel;
    public int currentLevelInt;
    public string currentLevel;

    public int randomMap;


    void Start()
    {
        sceneTransParent = GameObject.Find("SceneLoader");
        sceneTransition = sceneTransParent.transform.GetChild(0).GetComponent<Animator>();
        // randomMap = UnityEngine.Random.Range(1, 5);
        randomMap = 1;
        manager = GameObject.Find("Manager");
        manageLevels = manager.GetComponent<ManageLevels>();
        nextLevelInt = manageLevels.nextLevel;
        nextLevel = "Level" + nextLevelInt.ToString() + @"\" + randomMap.ToString();
        currentLevelInt = manageLevels.currentLevel;
        currentLevel = "Level" + currentLevelInt.ToString() + @"\" + randomMap.ToString();
        currentWorld = manageLevels.world;
        portalColor = manageLevels.worldColors[currentWorld];
        highlightColor = new Color32(System.Convert.ToByte(portalColor.r/2), System.Convert.ToByte(portalColor.g/2), System.Convert.ToByte(portalColor.b/2), 255);
        portalSprite = transform.GetComponent<SpriteRenderer>();
        portalSprite.color = portalColor;
    }

    public void Highlight()
    {
        interactCanvas.SetActive(true);
        portalSprite.color = highlightColor;
    }

    public void DeHighlight()
    {
        interactCanvas.SetActive(false);
        portalSprite.color = portalColor;
    }

    public void StartTransition()
    {
        StartCoroutine(LoadTransition());
    }

    public IEnumerator LoadTransition()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        LoadLevel();
    }

    public void LoadLevel()
    {

        //Load Shop
        if(nextLevelInt == 3 || nextLevelInt == 5)
        {
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                Debug.Log("Leaving shop");
                SceneManager.LoadScene(3);
                manageLevels.LeaveShopStarter(nextLevel);
                Destroy(gameObject);
                return;
            }
            else
            {
                SceneManager.LoadScene(2);
                manageLevels.ClearMap(currentLevel);
                manageLevels.EnterShopStarter();
                Destroy(gameObject);
                return;
            }
        }
        
        //Else, go to next level
        manageLevels.LoadLevel(nextLevel);
        Destroy(gameObject);
       
    }
}
