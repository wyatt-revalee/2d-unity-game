using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ManageLevels : MonoBehaviour
{
    private static GameObject Instance;
    public static ManageLevels instance;
    private GameObject spawnPoints;
    private GameObject portalSpawns;
    private GameObject enemySpawns;
    private GameObject playerSpawn;
    private GameObject bossSpawn;
    public bool loading;
    public Animator sceneTransition;

    public GameObject portal;
    public GameObject player;

    public List<GameObject> levelSpawns;

    public int world;
    public int currentLevel;
    public int nextLevel;
    private string firstLevel = @"Level1\1-1";
    string assetPath;
    public bool isWebGL;
    public string bundlePath;
    public bool assetsLoaded;
    public bool levelsLoaded;
    public bool tilesLoaded;

    AssetBundle bundle;

    private void Start()
    {

        assetPath = Application.dataPath;


        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isWebGL = true;
            Debug.Log("Loading assets from web");
            StartCoroutine(LoadAssetBundle(bundlePath));
        }
        else
        {
            isWebGL = false;
            Debug.Log("Loading assets locally");
            CreateTileListsLocal();
            assetsLoaded = true;

            // isWebGL = true;
            // StartCoroutine(LoadAssetBundle(bundlePath));
            

        }



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

    //List of loaded levels
    List<List<UnityEngine.Object>> levelList = new List<List<UnityEngine.Object>>();
    List<UnityEngine.Object> shopLevels = new List<UnityEngine.Object>();
    List<UnityEngine.Object> world1 = new List<UnityEngine.Object>();
    List<UnityEngine.Object> world2 = new List<UnityEngine.Object>();
    List<UnityEngine.Object> world3= new List<UnityEngine.Object>();
    List<UnityEngine.Object> world4 = new List<UnityEngine.Object>();
    List<UnityEngine.Object> world5 = new List<UnityEngine.Object>();

    //Palette Dictionary of Lists
    public Dictionary<string, List<CustomTile>> palettes = new Dictionary<string, List<CustomTile>>();
    List<CustomTile> red = new List<CustomTile>();
    List<CustomTile> orange = new List<CustomTile>();
    List<CustomTile> yellow = new List<CustomTile>();
    List<CustomTile> green = new List<CustomTile>();
    List<CustomTile> blue = new List<CustomTile>();
    List<CustomTile> purple = new List<CustomTile>();
    List<CustomTile> white = new List<CustomTile>();

    //Dictionary for conversion of int to world name
    public Dictionary<int, string> intToColor = new Dictionary<int, string>()
    {
        {1, "Red"},
        {2, "Orange"},
        {3, "Yellow"},
        {4, "Green"},
        {5, "Blue"},
        {6, "Purple"},
        {7, "White"}
    };

    // Dictionary for assigning colors to objects respective to world
    public Dictionary<int, Color32> worldColors = new Dictionary<int, Color32>()
    {
        {0, new Color32(255, 0, 0, 255)},       // Red, Index 0 for scripts that are a tad too quick
        {1, new Color32(255, 0, 0, 255)},       // Red
        {2, new Color32(255, 125, 42, 255)},    // Orange
        {3, new Color32(255, 201, 0, 255)},       // Yellow
        {4, new Color32(0, 255, 0, 255)},       // Green
        {5, new Color32(0, 245, 255, 255)},       // Blue
        {6, new Color32(113, 0, 255, 255)},       // Purple
        {7, new Color32(255, 255, 255, 255)},       // White
    };

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
        
        if(sceneTransition == null)
            sceneTransition = GameObject.Find("SceneLoader").transform.GetChild(0).GetComponent<Animator>();
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
        string json = "";
        int lvl = levelName[5] - '0';
        int sublvl = levelName[9] - '0';
        if(lvl == 2)
            Debug.Log(sublvl);
        if(isWebGL)
            json = levelList[lvl][sublvl-1].ToString();
        else
            json = File.ReadAllText(assetPath+ @"\Levels\"+levelName+".json");
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
            espawn.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            espawn.transform.SetParent(enemySpawns.transform);
            espawn.transform.position = spawn;
        }

        playerSpawn = new GameObject("playerSpawn");
        playerSpawn.transform.SetParent(spawnPoints.transform);
        playerSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        playerSpawn.transform.position = levelData.playerSpawn;

        bossSpawn = new GameObject("bossSpawn");
        bossSpawn.transform.SetParent(spawnPoints.transform);
        bossSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        bossSpawn.transform.position = levelData.bossSpawn;

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
        string json = "";
        int lvl = levelName[5] - '0';
        int sublvl = levelName[9] - '0';
        if(lvl == 2)
            Debug.Log(sublvl);
        if(isWebGL)
            json = levelList[lvl][sublvl-1].ToString();
        else
            json = File.ReadAllText(assetPath+ @"\Levels\"+levelName+".json");
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
        yield return new WaitUntil(() => (assetsLoaded == true));
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

    public void CreateTileListsLocal()
    {
        //Add the initialized empty lists to the dict
        palettes.Add("Red", red);
        palettes.Add("Orange", orange);
        palettes.Add("Yellow", yellow);
        palettes.Add("Green", green);
        palettes.Add("Blue", blue);
        palettes.Add("Purple", purple);
        palettes.Add("White", white);

        //Loop through each folder of custom tiles (red, orange, ...)
        string[] dir = Directory.GetFiles(assetPath+ @"/Resources/CustomTiles/");
        foreach (string path in dir)
        {
            //Remove unnecessary stuff from path to get the key for the dict (so get the color from the filepath)
            string buffer = assetPath+ @"/Resources/CustomTiles/";
            // Debug.Log("BUFFER: " + buffer);
            // Debug.Log("BUFFER.LENGTH: " + buffer.Length);
            string file = path.Remove(0, buffer.Length);
            file = file.Remove(file.Length-5, 5);
            // Debug.Log("FILE: " + file);

            //Go through each customtile in the color, create it as a customtile using Resources.Load, and add it into the corresponding list of the dict
            string[] tilePalette = Directory.GetFiles(assetPath+ @"/Resources/CustomTiles/" + file, "*.asset");
            foreach (var tile in tilePalette)
            {
                //Get local path from full path, pass it into Resources.Load to grab file
                string tileSnip = tile.Remove(0, buffer.Length);
                tileSnip = tileSnip.Remove(tileSnip.Length-6, 6);
                // Debug.Log("SNIP: " + tileSnip);
                CustomTile ctile = Resources.Load<CustomTile>(@"CustomTiles/" + tileSnip);
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
        string json;
        if(isWebGL)
            json = levelList[0][0].ToString();
        else
            json = File.ReadAllText(assetPath+ @"\Levels\Shop.json");
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
            pspawn.transform.localScale = new Vector3(0.5f, 1f, 1f);
            pspawn.transform.SetParent(portalSpawns.transform);
            pspawn.transform.position = spawn;
        }

        playerSpawn = new GameObject("playerSpawn");
        playerSpawn.transform.SetParent(spawnPoints.transform);
        playerSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        playerSpawn.transform.position = levelData.playerSpawn;

        player.transform.position = playerSpawn.transform.position;
        CreatePortal();
        Debug.Log("Portal Created");
        
        loading = false;
        Debug.Log("Shop was loaded");
    }

    private IEnumerator LoadAssetBundle(string url)
    {
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Error while downloading asset bundle: " + www.error);
                    yield break;
                }
                
                bundle = DownloadHandlerAssetBundle.GetContent(www);
        }
        string[] assets = bundle.GetAllAssetNames();

        yield return new WaitUntil(() => (bundle != null));
        CreateTileListsWeb();
        CreateLevelLists();
    }

    public void CreateTileListsWeb()
    {
        //Add the initialized empty lists to the dict
        palettes.Add("Red", red);
        palettes.Add("Orange", orange);
        palettes.Add("Yellow", yellow);
        palettes.Add("Green", green);
        palettes.Add("Blue", blue);
        palettes.Add("Purple", purple);
        palettes.Add("White", white);

        CustomTile[] tiles = bundle.LoadAllAssets<CustomTile>();
        string[] names = bundle.GetAllAssetNames();
        
        foreach (var tile in tiles)
        {
            if(tile.name[0] == 'R')
            {
                palettes["Red"].Add(tile);
            }

            if(tile.name[0] == 'O')
            {
                palettes["Orange"].Add(tile);
            }

            if(tile.name[0] == 'Y')
            {
                palettes["Yellow"].Add(tile);
            }

            if(tile.name[0] == 'G')
            {
                palettes["Green"].Add(tile);
            }

            if(tile.name[0] == 'B')
            {
                palettes["Blue"].Add(tile);
            }

            if(tile.name[0] == 'P')
            {
                palettes["Purple"].Add(tile);
            }

            if(tile.name[0] == 'W')
            {
                palettes["White"].Add(tile);
            }
        }

        // foreach (string name in names)
        // {
        //     Debug.Log("Name:" + name);
        // }

        tilesLoaded = true;
        if(levelsLoaded && tilesLoaded){
            assetsLoaded = true;
            bundle.Unload(false);
        }


    }

    public void CreateLevelLists()
    {
        
        levelList.Add(shopLevels);
        levelList.Add(world1);
        levelList.Add(world2);
        levelList.Add(world3);
        levelList.Add(world4);
        levelList.Add(world5);

        string[] temp = bundle.GetAllAssetNames();
        List<string> files = new List<string>();

        foreach(string fileName in temp)
        {
            if(fileName.EndsWith(".json"))
            {
                files.Add(fileName);
            }
        
        }
        
        foreach(string file in files)
        {
            if(file[24] == 's')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[0].Add(level);
            }

            if(file[31] == '1')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[1].Add(level);
            }

            if(file[31] == '2')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[2].Add(level);
            }

            if(file[31] == '3')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[3].Add(level);
            }

            if(file[31] == '4')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[4].Add(level);
            }

            if(file[31] == '5')
            {
                UnityEngine.Object level = bundle.LoadAsset(file);
                levelList[5].Add(level);
            }
        }
        levelsLoaded = true;
        if(levelsLoaded && tilesLoaded){
            assetsLoaded = true;
            bundle.Unload(false);
        }
    }
}

public class LevelData
{
    public List<LayerData> layers = new List<LayerData>();
    public Vector3 playerSpawn;
    public Vector3 bossSpawn;
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

