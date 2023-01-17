using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] Tilemap defaultTilemap;

    public List<GameObject> portalSpawns = new List<GameObject>();    
    public List<GameObject> enemySpawns = new List<GameObject>();    
    public GameObject playerSpawn;
    public GameObject bossSpawn;
 
    public Sprite enemySpawnSprite;
    public Sprite playerSpawnSprite;
    public Sprite portalSpawnSprite;
    public Sprite bossSpawnSprite;
    private Vector3 portalScale = new Vector3(3f, 6f, 1f);

    Tilemap currentTilemap
    {
        get
        {
            if (LevelManager.instance.layers.TryGetValue((int)LevelManager.instance.tiles[_selectedTileIndex].tilemap, out Tilemap tilemap))
            {
                return tilemap;
            }
            else
            {
                return defaultTilemap;
            }
        }
    }
    TileBase currentTile
    {
        get
        {
            return LevelManager.instance.tiles[_selectedTileIndex].tile;
        }
    }
    
    [SerializeField] Camera cam;

    private int _selectedTileIndex;

    int portSpawnCount = 0;
    int enemySpawnCount = 0;

    private void Update()
    {
        Vector3Int pos = currentTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButton(0))    PlaceTile(pos);

        if (Input.GetMouseButton(1))    DeleteTile(pos);
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedTileIndex++;
            if (_selectedTileIndex >= LevelManager.instance.tiles.Count) _selectedTileIndex = 0;
            Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        { 
            _selectedTileIndex--;
            if (_selectedTileIndex < 0) _selectedTileIndex = LevelManager.instance.tiles.Count -1;
            Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name);
        }

        
        if(Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
        {
            PlacePortalSpawn(pos);
        }

        if(Input.GetKeyDown(KeyCode.Z) && Input.GetMouseButton(0))
        {
            UndoLastPortalSpawn();
        }

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
        {
            PlaceEnemySpawn(pos);
        }

        if(Input.GetKey(KeyCode.Z) && Input.GetMouseButtonDown(1))
        {
            UndoLastEnemySpawn();
        }

        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
        {
            PlacePlayerSpawn(pos);
        }

        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.X))
        {
            PlaceBossSpawn(pos);
        }

    }

    void PlaceTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, currentTile);
    }

    void DeleteTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, null);
    }

    void PlacePortalSpawn(Vector3Int pos)
    {
        portSpawnCount++;
            GameObject newPortalSpawn = new GameObject(portSpawnCount.ToString());
            newPortalSpawn.transform.position = pos;
            SpriteRenderer spriteRen = newPortalSpawn.AddComponent<SpriteRenderer>();
            spriteRen.sprite = portalSpawnSprite;
            spriteRen.sortingLayerName = "Objects";
            newPortalSpawn.transform.localScale = portalScale;
            portalSpawns.Add(newPortalSpawn);
    }

    void UndoLastPortalSpawn()
    {   
        if(portSpawnCount > 0)
        {
            Destroy(portalSpawns[portSpawnCount-1]);
            portalSpawns.RemoveAt(portSpawnCount-1);
            portSpawnCount--;
        }
    }

    void PlaceEnemySpawn(Vector3Int pos)
    {
        enemySpawnCount++;
        GameObject newEnemySpawn = new GameObject(enemySpawnCount.ToString());
        newEnemySpawn.transform.position = pos;
        SpriteRenderer spriteRen = newEnemySpawn.AddComponent<SpriteRenderer>();
        spriteRen.sprite = enemySpawnSprite;
        spriteRen.sortingLayerName = "Objects";
        newEnemySpawn.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        enemySpawns.Add(newEnemySpawn);
    }

    void UndoLastEnemySpawn()
    {   
        if(enemySpawnCount > 0)
        {
            Destroy(enemySpawns[enemySpawnCount-1]);
            enemySpawns.RemoveAt(enemySpawnCount-1);
            enemySpawnCount--;
        }
    }

    void PlacePlayerSpawn(Vector3Int pos)
    {
        if(playerSpawn) Destroy(playerSpawn);
        playerSpawn = new GameObject("PlayerSpawn");
        playerSpawn.transform.position = pos;
        SpriteRenderer spriteRen = playerSpawn.AddComponent<SpriteRenderer>();
        spriteRen.sprite = playerSpawnSprite;
        spriteRen.sortingLayerName = "Objects";
        playerSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
        
    }

    void PlaceBossSpawn(Vector3Int pos)
    {
         if(bossSpawn) Destroy(bossSpawn);
        bossSpawn = new GameObject("BossSpawn");
        bossSpawn.transform.position = pos;
        SpriteRenderer spriteRen = bossSpawn.AddComponent<SpriteRenderer>();
        spriteRen.sprite = bossSpawnSprite;
        spriteRen.sortingLayerName = "Objects";
        bossSpawn.transform.localScale = new Vector3(5f, 5f, 1f);
    }
   
}
