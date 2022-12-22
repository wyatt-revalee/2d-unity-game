using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public string saveName;
    public GameObject spawnPoints;

    private void Awake()
    {
        //set up the instance
        if (instance == null) instance = this;
        else Destroy(this);

        spawnPoints = new GameObject("spawnPoints");

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
    }

    public List<CustomTile> tiles = new List<CustomTile>();
    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();

    [Header("Sprites")]
    public Sprite portalSprite;
    public Sprite playerSprite;
    public Sprite enemySprite;


    public LevelEditor levelEditor;

    public enum Tilemaps
    {
        Sky = 30,
        Background = 40,
        Ground = 50
    }

    private void Update()
    {
        //save level when pressing Ctrl + A
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) Savelevel();
        //load level when pressing Ctrl + L
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) LoadLevel();
    }

    void Savelevel()
    {
        //create a new leveldata
        LevelData levelData = new LevelData();

        //set up the layers in the leveldata
        foreach (var item in layers.Keys)
        {
            levelData.layers.Add(new LayerData(item));
        }

        foreach (var layerData in levelData.layers)
        {
            if (!layers.TryGetValue(layerData.layer_id, out Tilemap tilemap)) break;

            //get the bounds of the tilemap
            BoundsInt bounds = tilemap.cellBounds;

            //loop trougth the bounds of the tilemap
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    //get the tile on the position
                    TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                    //find the temp tile in the custom tiles list
                    CustomTile temptile = tiles.Find(t => t.tile == temp);

                    //if there's a customtile associated with the tile
                    if (temptile != null)
                    {
                        //add the values to the leveldata
                        layerData.tiles.Add(temptile.id);
                        layerData.poses_x.Add(x);
                        layerData.poses_y.Add(y);
                    }
                }
            }

        }

        foreach (var pspawn in levelEditor.portalSpawns)
        {
            levelData.portalSpawns.Add(pspawn.transform.position);
        }

        foreach (var espawn in levelEditor.enemySpawns)
        {
            levelData.enemySpawns.Add(espawn.transform.position);
        }

        levelData.playerSpawn = levelEditor.playerSpawn.transform.position;

        //save the data as a json
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\"+saveName+".json", json);

        //debug
        Debug.Log("Level was saved");
    }

    void LoadLevel()
    {
        //load the json file to a leveldata
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\"+saveName+".json");
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

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
            GameObject pspawn = new GameObject("portalSpawn" + pspawnCount.ToString());
            SpriteRenderer spriteRen = pspawn.AddComponent<SpriteRenderer>();
            spriteRen.sprite = portalSprite;
            pspawn.transform.localScale = new Vector3(0.5f, 1f, 1f);
            pspawn.transform.position = spawn;
        }

        int espawnCount = 0;
        // Loop through all enemy spawns and instantiate them
        foreach (var spawn in levelData.enemySpawns)
        {
            espawnCount++;
            GameObject espawn = new GameObject("enemySpawn" + espawnCount.ToString());
            SpriteRenderer spriteRen = espawn.AddComponent<SpriteRenderer>();
            spriteRen.sprite = enemySprite;
            espawn.transform.localScale = new Vector3(1f, 1f, 1f);
            espawn.transform.SetParent(spawnPoints.transform);
            espawn.transform.position = spawn;
        }

        //Next, createPlayerSpawn

        //Should probably create a parent "SpawnPoints" and make children
        // "portalSpawns", "enemySpawns", and "playerSpawn" as well as 
        // make subchildren containing the actual spawns for the first two.

        //debug
        Debug.Log("Level was loaded");
    }
}

// public class LevelData
// {
//     public List<LayerData> layers = new List<LayerData>();
//     public GameObject playerSpawn;
//     public List<GameObject> portalSpawns = new List<GameObject>();
//     public List<GameObject> enemySpawns = new List<GameObject>();
// }

// [System.Serializable]
// public class LayerData
// {
//     public int layer_id;
//     public List<string> tiles = new List<string>();
//     public List<int> poses_x = new List<int>();
//     public List<int> poses_y = new List<int>();

//     public LayerData (int id)
//     {
//         layer_id = id;
//     }
// }