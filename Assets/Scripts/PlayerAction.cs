using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAction : MonoBehaviour {
    /// <summary>
    /// Range within which the player can interact
    /// </summary>
    public float actionRange = 3.5f;

    /// <summary>
    /// Tilemaps of the terrains
    /// </summary>
    private List<Tilemap> terrainTilemaps = new List<Tilemap>();
    private Grid grid;

    // Use this for initialization
    void Start()
    {
        GameObject[] terrainsObjects = GameObject.FindGameObjectsWithTag("TerrainTilemap");
        int i = 0;
        foreach (GameObject terrainObject in terrainsObjects)
        {
            terrainTilemaps.Add(terrainObject.GetComponent<Tilemap>());
            i++;
        }
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        InteractWithEnvironment();
    }

    /// <summary>
    /// Place an indicator on the tile hovered by the cursor
    /// </summary>
    private void HoverTile(Vector3 mousePosition)
    {
        foreach (Tilemap terrainTilemap in terrainTilemaps)
        {
            Vector3Int cellPosition = terrainTilemap.WorldToCell(mousePosition);
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            if (tile != null)
            {
                GameObject tileSelection = GameObject.FindGameObjectWithTag("TileSelection");
                tileSelection.transform.position = terrainTilemap.CellToLocal(cellPosition);
            }
        }
    }

    /// <summary>
    /// Handle click interaction
    /// </summary>
    private void InteractWithEnvironment()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        HoverTile(mouseWorldPosition);
        if (Input.GetMouseButtonDown(0))
        {
            InteractWithTerrain(mouseWorldPosition);
        }
    }

    /// <summary>
    /// Handle interaction with terrain
    /// </summary>
    /// <param name="mousePosition">Position where to interact</param>
    private void InteractWithTerrain(Vector3 mousePosition)
    {
        bool actionDone = false;
        foreach (Tilemap terrainTilemap in terrainTilemaps)
        {
            Vector3Int cellPosition = terrainTilemap.WorldToCell(mousePosition);
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            if (tile != null)
            {
                Vector3 tileDistanceVector = cellPosition - transform.position;
                float tileDistance = Mathf.Sqrt(Mathf.Pow(tileDistanceVector.x, 2) + Mathf.Pow(tileDistanceVector.y, 2));
                if (tileDistance <= actionRange)
                {
                    if (!actionDone)
                    {
                        Dig(cellPosition);
                        actionDone = true;
                    }
                }
                else
                {
                    gameObject.GetComponent<PlayerMovement>().SetMouseClick(mousePosition);
                }
            }
        }
    }

    /// <summary>
    /// Dig a case of sand, if possible
    /// </summary>
    /// <param name="diggingPosition">Position where to dig sand</param>
    private void Dig(Vector3Int diggingPosition)
    {
        Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
        int i = terrainTilemaps.Count - 1;
        int replacedTile = -1;
        while (i >= 0)
        {
            Tilemap terrainTilemap = terrainTilemaps[i];
            if (terrainTilemap.GetTile(diggingPosition) != null && i > 0)
            {
                terrainTilemap.SetTile(diggingPosition, null);
                replacedTile = i - 1;
            }
            if (replacedTile == i)
            {
                terrainTilemap.SetTile(diggingPosition, tiles[i]);
            }
            i--;
        }
    }

    /// <summary>
    /// Replenish a case of sand, if possible
    /// </summary>
    /// <param name="replenishPosition">Position where to replenish sand</param>
    private void Replenish(Vector3Int replenishPosition)
    {
        Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
        int i = 0;
        int replacedTile = -1;
        bool tileReplaced = false;
        while (i < terrainTilemaps.Count)
        {
            Tilemap terrainTilemap = terrainTilemaps[i];
            if (terrainTilemap.GetTile(replenishPosition) != null && i < terrainTilemaps.Count - 2)
            {
                Debug.Log("tileReplacement");
                terrainTilemap.SetTile(replenishPosition, null);
                replacedTile = i + 1;
            }
            if (replacedTile == i && !tileReplaced)
            {
                Debug.Log("tileReplaced");
                terrainTilemap.SetTile(replenishPosition, tiles[i]);
                tileReplaced = true;
            }
            i++;
        }
    }
}
