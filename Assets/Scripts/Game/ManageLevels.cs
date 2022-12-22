using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

public class ManageLevels : MonoBehaviour
{
    public static ManageLevels instance;
    public Tilemap tilemap;
    public TileBase currentTiles;
    public int currentLevel;
    public List<CustomTile> tiles = new List<CustomTile>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();
    public static string levelPath = @"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\";

    public Portal portal;

    string[] mapFiles = Directory.GetFiles(levelPath, "*.json");

   void Start()
   {

    currentLevel = 1;

    for(int i = 0; i < mapFiles.Length; i++)
        Debug.Log(mapFiles[i]);
   }
   
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

     void LoadLevel()
    {
        string json = File.ReadAllText(@"C:\Users\Wyatt\UnityProjects\2D-Game\Assets\Levels\");
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        //loop through each layer
        foreach (var data in levelData.layers)
        {
            if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap)) break;

            //clear the tilemap
            tilemap.ClearAllTiles();

            //loop through all the tiles and place them
            for(int i = 0; i < data.tiles.Count; i++)
            {
                tilemap.SetTile(new Vector3Int(data.poses_x[i], data.poses_y[i], 0), tiles.Find(t => t.name == data.tiles[i]).tile);
            }
        }

        
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