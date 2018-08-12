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
    /// MapData
    /// </summary>
    int[,] mapData;

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

}
