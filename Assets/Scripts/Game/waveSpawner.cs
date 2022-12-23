using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
 
public class waveSpawner : MonoBehaviour
{
   
   //General
    public List<Enemy> enemies = new List<Enemy>();
    public int spawnerWorldCheck;
    public int spawnerLevel;
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    private static GameObject Instance;
    public ManageLevels manageLevels;
    public GameObject portal;
    public bool shopping;
    public bool settingUp;
 
    //Spawning
    public Transform spawnParent;
    public List<Transform> spawnLocations = new List<Transform>();

    //Intervals and timers
    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        spawnerWorldCheck = 1;
        spawnerLevel = 0;
        currWave = 0;
        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex != 3)
            SceneManager.LoadScene(3);
    }
 
    // Update is called once per frame
    void FixedUpdate()
    {
        //If at main menu or game over menu, get rid of wave spawner
        if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            Destroy(gameObject);

        if(SceneManager.GetActiveScene().buildIndex == 2)
            shopping = true;
        else
            shopping = false;
        
        if(shopping == true) return;
        if(manageLevels.loading == true) return;

        if(manageLevels.world > spawnerWorldCheck)
        {
            spawnerWorldCheck++;
            spawnerLevel = 0;
        }

        if(spawnerLevel < manageLevels.currentLevel)
        {
            spawnerLevel++;
            currWave++;
            StartCoroutine(StartLevel());
        }
        if(settingUp == true) return;
        GameObject[] spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(spawnTimer <=0)
        {
            //spawn an enemy
            if(enemiesToSpawn.Count >0)
            {
                int randLocation = UnityEngine.Random.Range(0, spawnLocations.Count-1);
                // Debug.Log("spawnLocations.Count: " +  spawnLocations.Count.ToString());
                // Debug.Log("spawnLocations.Count", spawnLocations.Count);
                
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
 
        if(waveTimer<=0 && spawnedEnemies.Length <=0 && enemiesToSpawn.Count == 0)
        {
            portal.SetActive(true);
        }
    }
 
    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();
 
        spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only
    }
 
    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.
 
        // repeat... 
 
        //  -> if we have no points left, leave the loop
 
        List<GameObject> generatedEnemies = new List<GameObject>();
        while(waveValue>0 || generatedEnemies.Count <50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;
 
            if(waveValue-randEnemyCost>=0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if(waveValue<=0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    void getSpawns() 
        {
            spawnLocations.Clear();
            foreach (Transform spawnPoint in spawnParent){
                spawnLocations.Add(spawnPoint);
            }
        }

    void StartNewLevel() {
        portal.SetActive(false);
        getSpawns();
        GenerateWave();
        settingUp = false;
    }

    IEnumerator StartLevel()
    {
        settingUp = true;
        portal = GameObject.Find("Portal(Clone)");
        yield return new WaitUntil(() => portal == true);
        GameObject temp = GameObject.Find("spawnPoints");
        yield return new WaitUntil(() => temp == true);
        spawnParent = temp.transform.GetChild(1);
        StartNewLevel();

    }
  
}
 
[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}