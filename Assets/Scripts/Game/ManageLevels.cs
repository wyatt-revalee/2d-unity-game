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
    public bool loading;
    public Animator sceneTransition;

    public GameObject portal;
    public GameObject player;

    public List<GameObject> levelSpawns;

    public int world;
    public int currentLevel;
    public int nextLevel;
    private string firstLevel = @"Level1\1";

    private void Start()
    {
        CreateTileLists();
        world = 0;
        loading = true;
        //set up the instance
        if(Instance == null)
        {
            Instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
        if (SceneManager.GetActiveScene().buildIndex == 4)
            SceneManager.LoadScene(3);

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

        StartCoroutine(StartManager());
    }

    public List<CustomTile> tiles = new List<CustomTile>();
    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();

    //Palette Dictionary of Lists
    public Dictionary<string, List<CustomTile>> palettes = new Dictionary<string, List<CustomTile>>();
    List<CustomTile> red = new List<CustomTile>();
    List<CustomTile> orange = new List<CustomTile>();
    List<CustomTile> yellow = new List<CustomTile>();
    List<CustomTile> green = new List<CustomTile>();
    List<CustomTile> blue = new List<CustomTile>();
    List<CustomTile> purple = new List<CustomTile>();

    //Dictionary for conversion of int to world name
    public Dictionary<int, string> intToColor = new Dictionary<int, string>()
    {
        {1, "Red"},
        {2, "Orange"},
        {3, "Yellow"},
        {4, "Green"},
        {5, "Blue"},
        {6, "Purple"},
    };

    // Dictionary for assigning colors to objects respective to world
    public Dictionary<int, Color32> worldColors = new Dictionary<int, Color32>()
    {
        {1, new Color32(255, 0, 0, 255)},       // Red
        {2, new Color32(255, 125, 42, 255)},    // Orange
        {3, new Color32(255, 0, 0, 255)},       // Yellow
        {4, new Color32(255, 0, 0, 255)},       // Green
        {5, new Color32(255, 0, 0, 255)},       // Blue
        {6, new Color32(255, 0, 0, 255)},       // Purple
    };

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
        if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            Destroy(gameObject);
        GameObject[] levelSpawns = GameObject.FindGameObjectsWithTag("Level");
    }

    public void LoadLevel(string levelName)
    {
        loading = true;
        Debug.Log("Loading: " + levelName.ToString());
        
        sceneTransition.SetTrigger("Start");

        if(currentLevel == 4)
        {
            currentLevel = nextLevel;
            nextLevel = 1;
        }
        else
        {
            currentLevel = nextLevel;
            nextLevel++;
        }
        if(currentLevel == 1)
            world++;

        string worldKey = intToColor[world];
        List<CustomTile> currWorld = palettes[worldKey];

        //load the json file to a leveldata
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\"+levelName+".json");
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);


        foreach (var data in levelData.layers)
        {

            if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap)) break;
            foreach (var spawn in levelSpawns) Destroy(spawn);
            //clear the tilemap
            tilemap.ClearAllTiles();

            //place the tiles
            for (int i = 0; i < data.tiles.Count; i++)
            {
                TileBase tile = currWorld.Find(t => t.id == data.tiles[i]).tile;
                if (tile) tilemap.SetTile(new Vector3Int(data.poses_x[i], data.poses_y[i], 0), tile);
            }
        }

        CreateParents();

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

        player.transform.position = playerSpawn.transform.position;
        
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            CreatePortal();
            Debug.Log("Portal Created");
        }
        
        loading = false;
        Debug.Log(levelName.ToString() + " was loaded");

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

    public void ClearMap(string levelName)
    {
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\"+levelName+".json");
       LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        foreach (var data in levelData.layers)
        {
            if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap)) break;
            //clear the tilemap
            tilemap.ClearAllTiles();
        }
    }

    public IEnumerator StartManager()
    {
        yield return new WaitUntil(() => (SceneManager.GetActiveScene().buildIndex == 3) == true);
        LoadLevel(firstLevel);
    }

    public void EnterShopStarter(){
        StartCoroutine(EnterShop());
    }

    public IEnumerator EnterShop()
    {
        yield return new WaitUntil(() => (SceneManager.GetActiveScene().buildIndex == 2) == true);
        LoadShop();
    }

    public void LeaveShopStarter(string levelname){
        StartCoroutine(LeaveShop(levelname));
    }

    public IEnumerator LeaveShop(string levelname)
    {
        yield return new WaitUntil(() => (SceneManager.GetActiveScene().buildIndex == 3) == true);
        LoadLevel(levelname);
    }

    public void CreateTileLists()
    {
        //Add the initialized empty lists to the dict
        palettes.Add("Red", red);
        palettes.Add("Orange", orange);
        palettes.Add("Yellow", yellow);
        palettes.Add("Green", green);
        palettes.Add("Blue", blue);
        palettes.Add("Purple", purple);

        //Loop through each folder of custom tiles (red, orange, ...)
        string[] dir = Directory.GetFiles(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Resources\CustomTiles\");
        foreach (string path in dir)
        {
            //Remove unnecessary stuff from path to get the key for the dict (so get the color from the filepath)
            string file = path.Substring(66, path.Length-66);
            file = file.Remove(file.Length - 5);

            //Go through each customtile in the color, create it as a customtile using Resources.Load, and add it into the corresponding list of the dict
            string[] tilePalette = Directory.GetFiles(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Resources\CustomTiles\" + file, "*.asset");
            foreach (var tile in tilePalette)
            {
                //Get local path from full path, pass it into Resources.Load to grab file
                string tileSnip = tile.Substring(65, tile.Length-71);
                CustomTile ctile = Resources.Load<CustomTile>(@"CustomTiles" + tileSnip);
                palettes[file].Add(ctile);
            }
        }
    }

    // This is similar to loadLevel but is just used to load the shop, nothing else.
    public void LoadShop()
    {
        loading = true;
        Debug.Log("Loading: Shop");
        
        sceneTransition.SetTrigger("Start");

        string worldKey = intToColor[world];
        List<CustomTile> currWorld = palettes[worldKey];

        //load the json file to a leveldata
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\Shop.json");
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);


        foreach (var data in levelData.layers)
        {

            if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap)) break;
            foreach (var spawn in levelSpawns) Destroy(spawn);
            //clear the tilemap
            tilemap.ClearAllTiles();

            //place the tiles
            for (int i = 0; i < data.tiles.Count; i++)
            {
                TileBase tile = currWorld.Find(t => t.id == data.tiles[i]).tile;
                Debug.Log(tile);
                if (tile) tilemap.SetTile(new Vector3Int(data.poses_x[i], data.poses_y[i], 0), tile);
            }
        }

        CreateParents();

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

        playerSpawn = new GameObject("playerSpawn");
        SpriteRenderer playSprite = playerSpawn.AddComponent<SpriteRenderer>();
        playSprite.sprite = playerSprite;
        playSprite.sortingLayerName = "Objects";
        playerSpawn.transform.SetParent(spawnPoints.transform);
        playerSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        playerSpawn.transform.position = levelData.playerSpawn;

        player.transform.position = playerSpawn.transform.position;
        CreatePortal();
        Debug.Log("Portal Created");
        
        loading = false;
        Debug.Log("Shop was loaded");
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

