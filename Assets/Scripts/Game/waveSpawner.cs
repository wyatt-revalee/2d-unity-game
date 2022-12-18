using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class waveSpawner : MonoBehaviour
{

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    public float waveValue;
    public GameObject waveFlashVisual;
    public GameObject waveUIVisual;
    public TMP_Text waveFlashText;
    public TMP_Text waveUIText;
    private string waveText;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    private static GameObject Instance;
    public GameObject portal;
    private int currentScene;
    private bool shopping;

    public Transform spawnParent;
    private List<Transform> spawnLocations = new List<Transform>();
    public int waveDuration;
    public float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    private float transitionTime;
    public bool transitioning;

    // Start is called before the first frame update
    void Start()
    {   

        getSpawns();

        currentScene = SceneManager.GetActiveScene().buildIndex;
        portal = GameObject.Find ("Portal");
        portal.SetActive(false);
        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
        transitioning = false;
        transitionTime = 5f;
        waveDuration = 20;
        waveValue = 10f;
        GenerateWave();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            Destroy(gameObject);
        
        if(SceneManager.GetActiveScene().buildIndex == 2)
            shopping = true;
        else if(SceneManager.GetActiveScene().buildIndex > 2)
            shopping = false;

        if(SceneManager.GetActiveScene().buildIndex > currentScene)
        {
            portal = GameObject.Find("Portal");
            portal.SetActive(false);
            Debug.Log("New portal found!");
        }

        waveFlashText.text = waveText;
        waveUIText.text = waveText;

        if(spawnTimer <= 0)
        {
            //spawn an enemy
            if(enemiesToSpawn.Count > 0)
            {
                int randLocation = UnityEngine.Random.Range(0, spawnLocations.Count);
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], spawnLocations[randLocation].position,Quaternion.identity); // spawn first enemy in our list
                enemiesToSpawn.RemoveAt(0); // and remove it
                spawnTimer = spawnInterval;
            }
            else
            {
                waveTimer = 0; // if no enemies remain, end wave
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }


        GameObject[] spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if(waveTimer<=0 && spawnedEnemies.Length <=0 && transitioning == false && shopping == false)
        {
            portal.SetActive(true);
            if(SceneManager.GetActiveScene().buildIndex > currentScene)
            {
                portal.SetActive(false);
                currentScene += 1;
                currWave += 1;
                waveDuration += 20;
                waveValue = 10 * currWave;
                GenerateWave();
            }
        }
        
    }

    void getSpawns() {
            foreach (Transform spawnPoint in spawnParent){
                spawnLocations.Add(spawnPoint);
            }
        }

    public void GenerateWave()
    {

        if(transitioning == false)
            StartCoroutine(WaveTransition());

    }

    public void GenerateEnemies()
    {

        //Create a temporary list of enemies to generate
        //
        // in a loop grab a random enemy
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.
        
        // repeat...
        
        // -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();
        while(waveValue > 0)
        {
            int randEnemyID = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyID].cost;

            if(waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyID].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if(waveValue <= 0)
            {
                break;
            }
        }

        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;

    }

    public IEnumerator WaveTransition()
    {
        if(currentScene == SceneManager.GetActiveScene().buildIndex)
        {
            transitioning = true;
            waveText = "Wave " + currWave.ToString();
            waveFlashVisual.SetActive(true);
            waveUIVisual.SetActive(false);
            yield return new WaitForSeconds(5);
            waveUIVisual.SetActive(true);
            waveFlashVisual.SetActive(false);
            yield return new WaitForSeconds(transitionTime);
            GenerateEnemies();

            spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
            waveTimer = waveDuration; // wave duration is read only
            transitioning = false;
        }
    }


}

    [System.Serializable]

public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}