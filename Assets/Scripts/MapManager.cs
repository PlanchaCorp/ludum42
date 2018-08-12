using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// Tilemaps of the terrains
    /// </summary>
    private List<Tilemap> terrainTilemaps = new List<Tilemap>();
    private TileInfo[,] terrainInfo;

    /// <summary>
    /// MapData of the tiles Height
    /// </summary>
	[SerializeField] private GameObject grid;

    /// <summary>
    /// MapData of the tiles Height
    /// </summary>
	[SerializeField] private Tile tower;

    /// <summary>
    /// MapData of the tiles Height
    /// </summary>
	[SerializeField] private Tile wallLeft;

    /// <summary>
    /// MapData of the tiles Height
    /// </summary>
	[SerializeField] private Tile wallRight;

    /// <summary>
    /// MapData of the tiles Height
    /// </summary>
	[SerializeField] private Tile wallHorizontal;

    /// <summary>
    /// MapData
    /// </summary>
    int[,] mapData;

    private enum DIRECTION
    {
        TOP_LEFT,
        TOP_RIGHT,
        RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        LEFT
    }

    private void Start()
    {
        // Get the map data
        mapData = grid.GetComponent<IslandGenerator>().map;
        terrainInfo = new TileInfo[mapData.GetLength(0),mapData.GetLength(1)];
        for(int k = 0;k< mapData.GetLength(0);k++ ){
            for(int j = 0; j < mapData.GetLength(1); j++ ){
                Vector3Int vect = new Vector3Int(k, j, 0);
                if (mapData[k,j]<0){
                    terrainInfo[k,j] = new TileInfo(vect,true, true);
                }else{
                    terrainInfo[k,j] = new TileInfo(vect,false, false);
                }
            }
        }

        // Get the tile map
        GameObject[] terrainsObjects = GameObject.FindGameObjectsWithTag("TerrainTilemap");
        int i = 0;
        foreach (GameObject terrainObject in terrainsObjects)
        {
            terrainTilemaps.Add(terrainObject.GetComponent<Tilemap>());
            i++;
        }
    }

    public TileInfo[,] GetTerrainInfo(){
        return this.terrainInfo;
    }

    /// <summary>
    /// Get the TILE map
    /// </summary>
    /// <returns></returns>
    public List<Tilemap> GetTileMap()
    {
        return terrainTilemaps;
    }

    /// <summary>
    /// Get the DATA map
    /// </summary>
    /// <returns></returns>
    public int[,] GetMapDataCoordinates()
    {
        return this.mapData;
    }

    /// <summary>
    /// Values range from 0 to MapSize
    /// Return Values from -MapSize/2 to MapSize/2
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3Int DataToTilesCoordinates(Vector3Int v)
    {
        int size = (grid.GetComponent<IslandGenerator>().mapSize / 2) - 1;
        return v - new Vector3Int(size, size, 0);
    }

    /// <summary>
    /// Values range from -MapSize/2 to MapSize/2
    /// Return values from 0 to MapSize
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3Int TilesToDataCoordinates(Vector3Int v)
    {
        int size = (grid.GetComponent<IslandGenerator>().mapSize / 2) - 1;
        return v + new Vector3Int(size, size, 0);
    }

    /// <summary>
    /// On suppose la case a enlevé valide
    /// The DiggingPosition param shall range from 0 to MapSize - 1
    /// </summary>
    /// <param name="tileCoordinates"></param>
    public void Dig(Vector3Int diggingPosition)
    {
        // On modifie les données de la map
        mapData[diggingPosition.x, diggingPosition.y]--;

        // On affiche la nouvelle Tile
        Tile[] tiles = grid.GetComponent<IslandGenerator>().GetSandTiles();
        int i = terrainTilemaps.Count - 1;
        int replacedTile = -1;
        diggingPosition = DataToTilesCoordinates(diggingPosition);
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
        // Refreshing the water level
        IslandGenerator islandGenerator = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>();
        islandGenerator.SetWater(islandGenerator.GetWaterLevel());
    }

    /// <summary>
    /// Replenish a case of sand, if possible
    /// </summary>
    /// <param name="replenishPosition">Position where to replenish sand</param>
    public void Replenish(Vector3Int replenishPosition)
    {
        // On modifie les données de la map
        mapData[replenishPosition.x, replenishPosition.y]++;

        // On affiche la nouvelle Tile
        Tile[] tiles = grid.GetComponent<IslandGenerator>().GetSandTiles();
        int i = 0;
        int replacedTile = -1;
        bool tileReplaced = false;
        replenishPosition = DataToTilesCoordinates(replenishPosition);

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
    
    /// <summary>
    /// Build a sand castle
    /// </summary>
    /// <param name="buildingPosition"></param>
    public void Build(Vector3Int buildingPosition)
    {
        // On verifie si l'on est sur du sable et qu'il n'y a rien
        if ( !terrainInfo[buildingPosition.x, buildingPosition.y].GetIsFlooded() &&
             terrainInfo[buildingPosition.x, buildingPosition.y].GetWallState() == TileInfo.WallSate.NOTHING ) 
        {
            // On modifie les données
            terrainInfo[buildingPosition.x, buildingPosition.y].SetWallState(TileInfo.WallSate.TOWER);

            //Vector3Int newPos = GetLeft(buildingPosition);
            //terrainInfo[newPos.x, newPos.y].SetWallState(TileInfo.WallSate.TOWER);
            TryCreateWalls(buildingPosition);

            // On affiche un chateau            
            DisplayCastleSprite(buildingPosition);
            //DisplayCastleSprite(newPos);

        } else
        {
            Debug.LogWarning("Can't build on water");
            return;
        }
    }

    /// <summary>
    /// Check in every direction if there in less than 2 distant
    /// </summary>
    /// <param name="position"></param>
    private void TryCreateWalls(Vector3Int position)
    {
        int distanceLeft = 4;

        TryCreateWalls_rec(position, DIRECTION.TOP_LEFT, distanceLeft);
        TryCreateWalls_rec(position, DIRECTION.TOP_RIGHT, distanceLeft);
        TryCreateWalls_rec(position, DIRECTION.RIGHT, distanceLeft);
        TryCreateWalls_rec(position, DIRECTION.BOTTOM_RIGHT, distanceLeft);
        TryCreateWalls_rec(position, DIRECTION.BOTTOM_LEFT, distanceLeft);
        TryCreateWalls_rec(position, DIRECTION.LEFT, distanceLeft);
    }

    /// <summary>
    ///  Reccursive function to create walls
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="distanceLeft"></param>
    private bool TryCreateWalls_rec(Vector3Int position, DIRECTION dir, int distanceLeft)
    {
        if ( distanceLeft <= 0 )
        {
            return false;
        }

        Vector3Int direction;
        switch (dir)
        {
            case DIRECTION.TOP_LEFT:
                direction = GetTopLeft(position);
                break;
            case DIRECTION.TOP_RIGHT:
                direction = GetTopRight(position);
                break;
            case DIRECTION.RIGHT:
                direction = GetRight(position);
                break;
            case DIRECTION.BOTTOM_LEFT:
                direction = GetBottomLeft(position);
                break;
            case DIRECTION.BOTTOM_RIGHT:
                direction = GetBottomRight(position);
                break;
            case DIRECTION.LEFT:
                direction = GetLeft(position);
                break;
            default:
                direction = new Vector3Int(0, 0, 0);
                break;
        }

        position = direction;

        // Avoid Out of Bound
        if (position.x >= 0 &&
            position.x <= terrainInfo.GetLength(0) &&
            position.y >= 0 &&
            position.y <= terrainInfo.GetLength(1))
        {
            TileInfo currentTile = terrainInfo[position.x, position.y];

            // Check the next tile
            if ( TryCreateWalls_rec(position, dir, distanceLeft - 1) )
            {
                // Create a wall
                if ( dir == DIRECTION.TOP_LEFT || dir == DIRECTION.BOTTOM_RIGHT )
                {
                    currentTile.SetWallState(TileInfo.WallSate.WALL_RIGHT);
                } else if (dir == DIRECTION.TOP_RIGHT || dir == DIRECTION.BOTTOM_LEFT)
                {
                    currentTile.SetWallState(TileInfo.WallSate.WALL_LEFT);
                } else
                {
                    currentTile.SetWallState(TileInfo.WallSate.WALL_HORIZONTAL);
                }
                DisplayCastleSprite(position);
                return true;
            }

            // Check if there is a Wall a the current place
            if (currentTile.GetWallState() == TileInfo.WallSate.WALL_HORIZONTAL ||
                currentTile.GetWallState() == TileInfo.WallSate.WALL_RIGHT ||
                currentTile.GetWallState() == TileInfo.WallSate.WALL_LEFT)
            {
                return false;
            }
            // Check if there is a tower
            else if ( currentTile.GetWallState() == TileInfo.WallSate.TOWER )
            {
                return true;
            }
        } 
        return false;       
    }


    /// <summary>
    /// On suppose la position non nulle
    /// On affiche le sprite
    /// </summary>
    /// <param name="position"></param>
    public void DisplayCastleSprite(Vector3Int position)
    {
        Tilemap tilemap = GameObject.FindGameObjectWithTag("StructureTilemap").GetComponent<Tilemap>();
        Vector3Int tilePosition = DataToTilesCoordinates(position);
        switch ( terrainInfo[position.x, position.y].GetWallState() )
        {
            case TileInfo.WallSate.NOTHING:
                Debug.Log("Not a castle sprite");
                break;
            case TileInfo.WallSate.TOWER:
                tilemap.SetTile(tilePosition, tower);
                break;
            case TileInfo.WallSate.WALL_HORIZONTAL:
                tilemap.SetTile(tilePosition, wallHorizontal);
                break;
            case TileInfo.WallSate.WALL_LEFT:
                tilemap.SetTile(tilePosition, wallLeft);
                break;
            case TileInfo.WallSate.WALL_RIGHT:
                tilemap.SetTile(tilePosition, wallRight);
                break;
        }
    }

    /// <summary>
    /// Return the Top Left position
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetTopLeft(Vector3Int pos)
    {
        if ( pos.y%2 == 0 )
        {
            return pos + new Vector3Int(0, 1, 0);
        } else
        {
            return pos + new Vector3Int(-1, 1, 0);

        }
    }

    /// <summary>
    /// Return the Top Right position
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetTopRight(Vector3Int pos)
    {
        if (pos.y % 2 == 0)
        {
            return pos + new Vector3Int(1, 1, 0);
        }
        else
        {
            return pos + new Vector3Int(0, 1, 0);

        }
    }

    /// <summary>
    /// Return the Bottom Left position
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetBottomLeft(Vector3Int pos)
    {
        if (pos.y % 2 == 0)
        {
            return pos + new Vector3Int(0, -1, 0);
        }
        else
        {
            return pos + new Vector3Int(-1, -1, 0);

        }
    }

    /// <summary>
    /// Return the Bottom Right position
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetBottomRight(Vector3Int pos)
    {
        if (pos.y % 2 == 0)
        {
            return pos + new Vector3Int(1, -1, 0);
        }
        else
        {
            return pos + new Vector3Int(0, -1, 0);

        }
    }

    /// <summary>
    /// Return the Right
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetRight(Vector3Int pos)
    {
        return pos + new Vector3Int(1, 0, 0);
    }

    /// <summary>
    /// Return the Right
    /// </summary>
    /// <param name="pos"></param>
    public Vector3Int GetLeft(Vector3Int pos)
    {
        return pos + new Vector3Int(-1, 0, 0);
    }
}
