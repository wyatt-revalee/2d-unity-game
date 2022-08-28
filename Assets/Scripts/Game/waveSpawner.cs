using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    public float waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform spawnLocation;
    public int waveDuration;
    public float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        waveDuration = 20;
        waveValue = 10f;
        GenerateWave();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spawnTimer <= 0)
        {
            //spawn an enemy
            if(enemiesToSpawn.Count > 0)
            {
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], spawnLocation.position,Quaternion.identity); // spawn first enemy in our list
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
        

        if(waveTimer<=0 && spawnedEnemies.Length <=0)
        {
             currWave += 1;
             waveDuration += 20;
             waveValue *= 1.5f;

             GenerateWave();
        }
        
    }

    public void GenerateWave()
    {
        GenerateEnemies();

        waveValue = 10 * currWave;

        spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only

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


}

    [System.Serializable]

public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}