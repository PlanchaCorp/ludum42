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
    /// Sprites of the breath bar
    /// </summary>
    [SerializeField]
    private Sprite[] breathBarSprites;

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
    /// Max amount of sand in inventory
    /// </summary>
    [SerializeField]
    private int maxSandInInventory = 10;

    /// <summary>
    /// Amount of time needed to die from drowning
    /// </summary>
    [SerializeField]
    private float drowningTime = 5f;

    /// <summary>
    /// Tilemaps of the terrains
    /// </summary>
    private List<Tilemap> terrainTilemaps = new List<Tilemap>();

    /// <summary>
    /// Tilemap of the water
    /// </summary>
    private Tilemap waterTilemap;

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

    /// <summary>
    /// Amount of sand in inventory
    /// </summary>
    private int sandInInventory = 0;

    /// <summary>
    /// Current time from which the player began drowning
    /// </summary>
    private float currentDrowningTime = 0;

    /// <summary>
    /// Active boost of respiration
    /// </summary>
    private bool respirationBoost = false;

    /// <summary>
    /// Active boost of construction
    /// </summary>
    private bool diggingBoost = false;

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
        waterTilemap = GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<Tilemap>();
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
        CheckWater();
    }

    /// <summary>
    /// Forbid digging while hotbar not equal one
    /// </summary>
    private void CheckHotbar()
    {
        int switchHotbar = uiHotbarCanvas.GetComponent<UIHotbar>().GetSwitch();
        if (switchHotbar != 1)
        {
            StopDigging();
        }
    }

    /// <summary>
    /// Check that player is in water and activates breath bar
    /// </summary>
    private void CheckWater()
    {
        bool isDrowning = false;
        foreach (Tilemap terrainTilemap in terrainTilemaps)
        {
            Vector3Int cellPosition = terrainTilemap.WorldToCell(gameObject.transform.position);
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            if (tile != null)
            {
                isDrowning = terrainTilemap.GetComponent<TilemapRenderer>().sortingOrder < waterTilemap.GetComponent<TilemapRenderer>().sortingOrder;
            }
        }
        SpriteRenderer breathBar = GameObject.FindGameObjectWithTag("BreathBar").GetComponent<SpriteRenderer>();
        if (isDrowning)
        {
            breathBar.enabled = true;
            currentDrowningTime += (respirationBoost) ? Time.deltaTime * 0.5f : Time.deltaTime;
            int breathBarCurrentSprite = Mathf.FloorToInt(currentDrowningTime * breathBarSprites.Length / drowningTime);
            if (breathBarCurrentSprite >= 0 && breathBarCurrentSprite < breathBarSprites.Length)
            {
                breathBar.sprite = breathBarSprites[breathBarCurrentSprite];
            }
            if (currentDrowningTime > drowningTime)
            {
                Debug.Log("You just drowned !");
            }
        } else
        {
            currentDrowningTime -= Time.deltaTime * 2;
            if (currentDrowningTime < 0)
            {
                breathBar.enabled = false;
                currentDrowningTime = 0;
            } else
            {
                int breathBarCurrentSprite = Mathf.FloorToInt(currentDrowningTime * breathBarSprites.Length / drowningTime);
                if (breathBarCurrentSprite >= 0 && breathBarCurrentSprite < breathBarSprites.Length)
                {
                    breathBar.sprite = breathBarSprites[breathBarCurrentSprite];
                }
            }
        }
    }

    /// <summary>
    /// Place an indicator on the tile hovered by the cursor
    /// </summary>
    private void HoverTile(Vector3 mousePosition)
    {
        GameObject tileSelection = GameObject.FindGameObjectWithTag("TileSelection");
        bool tileMatched = false;
        foreach (Tilemap terrainTilemap in terrainTilemaps)
        {
            Vector3Int cellPosition = terrainTilemap.WorldToCell(mousePosition);
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            if (tile != null)
            {
                tileMatched = true;
                Vector3 localCellPosition = terrainTilemap.CellToLocal(cellPosition);
                if (tileSelection.transform.position != localCellPosition)
                {
                    StopDigging();
                }
                tileSelection.transform.position = localCellPosition;
                tileSelection.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if (!tileMatched)
        {
            tileSelection.GetComponent<SpriteRenderer>().enabled = false;
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
                Vector3 tileDistanceVector = mousePosition - transform.position;
                float tileDistance = Mathf.Sqrt(Mathf.Pow(tileDistanceVector.x, 2) + Mathf.Pow(tileDistanceVector.y, 2));
                if (tileDistance <= actionRange)
                {
                    if (!actionDone)
                    {
                        switch (uiHotbarCanvas.GetComponent<UIHotbar>().GetSwitch())
                        {
                            case 1:
                                if (terrainTilemap.GetComponent<TilemapRenderer>().sortingOrder >= waterTilemap.GetComponent<TilemapRenderer>().sortingOrder
                                    && sandInInventory < maxSandInInventory && terrainTilemap.GetComponent<TilemapRenderer>().sortingOrder > 0)
                                {
                                    Dig(cellPosition);
                                }
                                break;
                            case 2:
                                if (sandInInventory > 0)
                                {
                                    TryReplenish(cellPosition);
                                }
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
            gameObject.GetComponent<Animator>().SetBool("isDigging", false);
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        } else
        {
            digBar.enabled = true;
            diggingTime -= (diggingBoost) ? Time.deltaTime * 1.5f : Time.deltaTime;
            int digBarCurrentSprite = Mathf.FloorToInt(diggingTime * digBarSprites.Length / maxDiggingTime);
            if (digBarCurrentSprite >= 0)
            {
                digBar.sprite = digBarSprites[digBarCurrentSprite];
            }
            gameObject.GetComponent<Animator>().SetBool("isDigging", true);
            if (diggingPosition.x < transform.position.x)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
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
        gameObject.GetComponent<Animator>().SetBool("isDigging", false);
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
        diggingTime = maxDiggingTime;
    }

    /// <summary>
    /// Dig a case of sand, if possible
    /// </summary>
    /// <param name="diggingPosition">Position where to dig sand</param>
    private void FinishDigging(Vector3Int diggingPosition)
    {
        Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
        MapManager mapManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MapManager>();
        mapManager.Dig(mapManager.TilesToDataCoordinates(diggingPosition));
        sandInInventory++;
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
        /*Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
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
        }*/
        MapManager mapManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MapManager>();
        mapManager.Replenish(mapManager.TilesToDataCoordinates(replenishPosition));
        sandInInventory--;
    }

    /// <summary>
    /// Activate or deactivate respiration boost
    /// </summary>
    /// <param name="value">Activation or deactivation</param>
    public void SetRespirationBoost(bool value)
    {
        this.respirationBoost = value;
    }

    /// <summary>
    /// Activate or deactivate digging boost
    /// </summary>
    /// <param name="value">Activation or deactivation</param>
    public void SetDiggingBoost(bool value)
    {
        this.diggingBoost = value;
    }
}
