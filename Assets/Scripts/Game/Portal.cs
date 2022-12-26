using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    private SpriteRenderer portalSprite;
    private Color portalColor;
    private Color highlightColor;
    public GameObject sceneTransParent;
    public Animator sceneTransition;

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
                Debug.Log("Loading shop");
                SceneManager.LoadScene(2);
                manageLevels.ClearMap(currentLevel);
                Destroy(gameObject);
                return;
            }
        }
        
        Debug.Log("Loading level: " + nextLevel);
        //Else, go to next level
        manageLevels.LoadLevel(nextLevel);
        Destroy(gameObject);
       
    }
}
