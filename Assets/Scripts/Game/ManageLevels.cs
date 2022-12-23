using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class ManageLevels : MonoBehaviour
{
    private static GameObject Instance;
    public static ManageLevels instance;
    private GameObject spawnPoints;
    private GameObject portalSpawns;
    private GameObject enemySpawns;
    private GameObject playerSpawn;

    public GameObject portal;
    public GameObject player;

    public List<GameObject> levelSpawns;

    public int currentLevel;
    public int nextLevel;
    private string firstLevel = @"Level1\1";

    private void Start()
    {
        GameObject[] managers = GameObject.FindGameObjectsWithTag("Manager");
        if(managers.Length > 1) Destroy(gameObject);
        Debug.Log("TEST");
        //set up the instance
        if (instance == null) instance = this;
        else Destroy(this);

        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        CreateParents();
        currentLevel = 0;
        nextLevel = 1;

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Tilemaps num in System.Enum.GetValues(typeof(Tilemaps)))
            {
                if (tilemap.name == num.ToString())
                {
                    if (!layers.ContainsKey((int)num)) layers.Add((int)num, tilemap);
                }
            }
        }

        LoadLevel(firstLevel);
    }

    public List<CustomTile> tiles = new List<CustomTile>();
    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();
    

    [Header("Sprites")]
    public Sprite portalSprite;
    public Sprite playerSprite;
    public Sprite enemySprite;



    public enum Tilemaps
    {
        Sky = 30,
        Background = 40,
        Ground = 50
    }

    private void Update()
    {
        GameObject[] levelSpawns = GameObject.FindGameObjectsWithTag("Level");
    }

    public void LoadLevel(string levelName)
    {

        if(SceneManager.GetActiveScene().buildIndex == 2)
            return;

        currentLevel++;
        nextLevel++;

        foreach (var spawn in levelSpawns) Destroy(spawn);
        //load the json file to a leveldata
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\"+levelName+".json");
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        CreateParents();

        foreach (var data in levelData.layers)
        {


            if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap)) break;

            //clear the tilemap
            tilemap.ClearAllTiles();


            //place the tiles
            for (int i = 0; i < data.tiles.Count; i++)
            {
                TileBase tile = tiles.Find(t => t.id == data.tiles[i]).tile;
                if (tile) tilemap.SetTile(new Vector3Int(data.poses_x[i], data.poses_y[i], 0), tile);
            }
        }

        int pspawnCount = 0;
        // Loop through all portal spawns and instantiate them
        foreach (var spawn in levelData.portalSpawns)
        {
            pspawnCount++;
            GameObject pspawn = new GameObject("spawn" + pspawnCount.ToString());
            SpriteRenderer spriteRen = pspawn.AddComponent<SpriteRenderer>();
            spriteRen.sprite = portalSprite;
            spriteRen.sortingLayerName = "Objects";
            pspawn.transform.localScale = new Vector3(0.5f, 1f, 1f);
            pspawn.transform.SetParent(portalSpawns.transform);
            pspawn.transform.position = spawn;
        }

        int espawnCount = 0;
        // Loop through all enemy spawns and instantiate them
        foreach (var spawn in levelData.enemySpawns)
        {
            espawnCount++;
            GameObject espawn = new GameObject("spawn" + espawnCount.ToString());
            SpriteRenderer spriteRen = espawn.AddComponent<SpriteRenderer>();
            spriteRen.sprite = enemySprite;
            spriteRen.sortingLayerName = "Objects";
            espawn.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            espawn.transform.SetParent(enemySpawns.transform);
            espawn.transform.position = spawn;
        }

        playerSpawn = new GameObject("playerSpawn");
        SpriteRenderer playSprite = playerSpawn.AddComponent<SpriteRenderer>();
        playSprite.sprite = playerSprite;
        playSprite.sortingLayerName = "Objects";
        playerSpawn.transform.SetParent(spawnPoints.transform);
        playerSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        playerSpawn.transform.position = levelData.playerSpawn;

        //debug
        Debug.Log("Level " + currentLevel.ToString() + " was loaded");
        player.transform.position = playerSpawn.transform.position;
        if(SceneManager.GetActiveScene().buildIndex != 2)
            CreatePortal();
    }

    void CreateParents()
    {
        spawnPoints = new GameObject("spawnPoints");
        portalSpawns = new GameObject("portalSpawns");
            portalSpawns.transform.SetParent(spawnPoints.transform);
            portalSpawns.tag = "Level";
        enemySpawns = new GameObject("enemySpawns");
            enemySpawns.transform.SetParent(spawnPoints.transform);
            enemySpawns.tag = "Level";

        levelSpawns = new List<GameObject>();
        levelSpawns.Add(spawnPoints);
    }

    void CreatePortal()
    {
        int randomChild = portalSpawns.transform.childCount;
        randomChild = UnityEngine.Random.Range(0, randomChild);
        Vector3 randomSpawn = portalSpawns.transform.GetChild(randomChild).transform.position;
        Instantiate(portal, randomSpawn, Quaternion.identity);
        
    }
}

public class LevelData
{
    public List<LayerData> layers = new List<LayerData>();
    public Vector3 playerSpawn;
    public List<Vector3> portalSpawns = new List<Vector3>();
    public List<Vector3> enemySpawns = new List<Vector3>();
}

[System.Serializable]
public class LayerData
{
    public int layer_id;
    public List<string> tiles = new List<string>();
    public List<int> poses_x = new List<int>();
    public List<int> poses_y = new List<int>();

    public LayerData (int id)
    {
        layer_id = id;
    }
}