using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAction : MonoBehaviour {
    /// <summary>
    /// Range within which the player can interact
    /// </summary>
    [SerializeField]
    private float actionRange = 3.5f;

    /// <summary>
    /// Sprites of the digging bar
    /// </summary>
    [SerializeField]
    private Sprite[] digBarSprites;

    /// <summary>
    /// Time for digging a sand tile
    /// </summary>
    [SerializeField]
    private float maxDiggingTime = 0.3f;

    /// <summary>
    /// Cooldown for replenishing sand
    /// </summary>
    [SerializeField]
    private float maxReplenishCooldown = 0.05f;

    /// <summary>
    /// Canvas with hotbar
    /// </summary>
    [SerializeField]
    private Canvas uiHotbarCanvas;

    /// <summary>
    /// Tilemaps of the terrains
    /// </summary>
    private List<Tilemap> terrainTilemaps = new List<Tilemap>();

    /// <summary>
    /// Grid object containing all tilesets
    /// </summary>
    private Grid grid;

    /// <summary>
    /// Current dig time
    /// </summary>
    private float diggingTime = 0;

    /// <summary>
    /// Current replenishing cooldown elapsed (0 = can replenish)
    /// </summary>
    private float replenishTime = 0;

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
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        HoverTile(mouseWorldPosition);
        if (Input.GetMouseButton(0))
        {
            InteractWithTerrain(mouseWorldPosition);
        }
        else
        {
            StopDigging();
        }
        CheckHotbar();
    }

    private void CheckHotbar()
    {
        int switchHotbar = uiHotbarCanvas.GetComponent<UIHotbar>().GetSwitch();
        if (switchHotbar != 1)
        {
            StopDigging();
        }
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
                Vector3 localCellPosition = terrainTilemap.CellToLocal(cellPosition);
                
                if (tileSelection.transform.position != localCellPosition)
                {
                    StopDigging();
                }
                tileSelection.transform.position = localCellPosition;
            }
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
                        switch (uiHotbarCanvas.GetComponent<UIHotbar>().GetSwitch())
                        {
                            case 1:
                                Dig(cellPosition);
                                break;
                            case 2:
                                TryReplenish(cellPosition);
                                break;
                        }
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
    /// Digging action, handling animation and action
    /// </summary>
    /// <param name="diggingPosition">Tile position of the sand tile to dig</param>
    private void Dig(Vector3Int diggingPosition)
    {
        SpriteRenderer digBar = GameObject.FindGameObjectWithTag("DigBar").GetComponent<SpriteRenderer>();
        if (diggingTime < 0)
        {
            digBar.enabled = false;
            diggingTime = maxDiggingTime;
            FinishDigging(diggingPosition);
        } else
        {
            digBar.enabled = true;
            diggingTime -= Time.deltaTime;
            int digBarCurrentSprite = Mathf.FloorToInt(diggingTime * digBarSprites.Length / maxDiggingTime);
            if (digBarCurrentSprite >= 0)
            {
                digBar.sprite = digBarSprites[digBarCurrentSprite];
            }
        }
    }

    /// <summary>
    /// Stop digging right now
    /// </summary>
    private void StopDigging()
    {
        SpriteRenderer digBar = GameObject.FindGameObjectWithTag("DigBar").GetComponent<SpriteRenderer>();
        digBar.enabled = false;
        diggingTime = maxDiggingTime;
    }

    /// <summary>
    /// Dig a case of sand, if possible
    /// </summary>
    /// <param name="diggingPosition">Position where to dig sand</param>
    private void FinishDigging(Vector3Int diggingPosition)
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
    /// Try to replenish if cooldown available
    /// </summary>
    /// <param name="diggingPosition">Tile position of the sand tile to replenish</param>
    private void TryReplenish(Vector3Int replenishPosition)
    {
        if (replenishTime <= 0)
        {
            Replenish(replenishPosition);
            replenishTime = maxReplenishCooldown;
        } else
        {
            replenishTime -= Time.deltaTime;
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
                terrainTilemap.SetTile(replenishPosition, null);
                replacedTile = i + 1;
            }
            if (replacedTile == i && !tileReplaced)
            {
                terrainTilemap.SetTile(replenishPosition, tiles[i]);
                tileReplaced = true;
            }
            i++;
        }
    }
}
