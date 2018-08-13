using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo{

    private int x, y, height;
    private bool isFlooded, isSea;
    private float damage;

    private static Tilemap waterTileMap;
    private static AnimatedTile waterTile;
    private static int mapSize;
    private MapManager manager;

    public enum WallState
    {
        NOTHING,
        TOWER,
        WALL_HORIZONTAL,
        WALL_LEFT,
        WALL_RIGHT
    }
    private WallState state;

    public TileInfo(Vector3Int vector, int height, bool flooded, bool isSea){
        this.damage = 1.0f;
        this.isFlooded = flooded;
        this.isSea = isSea;
		this.height = height;
        this.x = vector.x;
        this.y = vector.y;

        this.state = WallState.NOTHING;
        this.manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MapManager>();

        if (isSea || isFlooded){
            Vector3Int vectorPosition = new Vector3Int(0,0,0);
            vectorPosition.x = TileInfo.mapSize / 2 - this.x;
            vectorPosition.y = TileInfo.mapSize / 2 - this.y;
            TileInfo.waterTileMap.SetTile(vectorPosition,waterTile);
        }
    }

    public int GetHeight(){
        return this.height;
    }

    public static void SetTileMapWater(Tilemap wtm, int size){
        TileInfo.waterTileMap = wtm;
        TileInfo.mapSize = size;
    }

    public static void SetWaterTile(AnimatedTile Water){
        TileInfo.waterTile = Water;
    }

    public Vector3Int GetCoordinates()
    {
        return new Vector3Int(x, y, 0);
    }

    public Vector3Int[] GetNeighboursCoordinates()
    {
        Vector3Int pos = new Vector3Int(this.x, this.y, 0);
        Vector3Int[] neighbours = new Vector3Int[6];
        neighbours[0] = GetTopLeft(pos);
        neighbours[1] = GetTopRight(pos);
        neighbours[2] = GetBottomLeft(pos);
        neighbours[3] = GetBottomRight(pos);
        neighbours[4] = GetLeft(pos);
        neighbours[5] = GetRight(pos);
        return neighbours;
    }

    public bool GetIsFlooded(){
        return this.isFlooded;
    }

    public bool GetIsSea(){
        return this.isSea;
    }

    public void SetIsFlooded(bool value){
        this.isFlooded = value;
        this.isSea = false;
        DisplayWater();
    }

    public void DisplayWater()
    {
        Vector3Int vectorPosition = manager.DataToTilesCoordinates(new Vector3Int(this.x, this.y, 0));
        if (isFlooded)
        {
            waterTileMap.SetTile(vectorPosition, waterTile);
        }
        else
        {
            waterTileMap.SetTile(vectorPosition, null);
        }
    }

	public void Dig(){
		this.height -= 1;
        if (!isFlooded && height < GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>().GetLevel())
        {
            Refresh();
        }
    }

	public void Rep(){
		this.height += 1;
        if (height >= GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>().GetLevel())
        {
            SetIsFlooded(false);
        }
	}

    public void Refresh()
    {
        if (!isFlooded)
        {
            TileInfo[,] terrainInfo = manager.GetTerrainInfo();
            foreach (Vector3Int neighbourLocation in GetNeighboursCoordinates())
            {
                if (ShallFlood())
                {
                    SetIsFlooded(true);
                    terrainInfo[neighbourLocation.x, neighbourLocation.y].Refresh();
                }
            }
        }
    }

    public bool ShallFlood()
    {
        return ShallFlood(GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>().GetActualLayer());
    }

    public bool ShallFlood( int actualLayer )
    {
        if (!GetIsFlooded() &&
            GetHeight() < actualLayer &&
            GetWallState() == WallState.NOTHING )
        {
            return true;
        } else
        {
            return false;
        }         
    }

    public bool ShallGetEroded()
    {
        if (state != WallState.NOTHING)
        {
            if (damage >= 2)
            {
                return true;
            }
        } else if (damage >= 1)
        {
            return true;
        }
        return false;
    }

    public void DecreaseDurability(float value)
    {
        this.damage -= value;
    }

    public float GetDamageAmount()
    {
        return this.damage;
    }

    public void ResetDurability()
    {
        this.damage = 1.0f;
    }

    public void SetWallState( WallState state)
    {
        this.state = state;
    }

    public WallState GetWallState()
    {
        return state;
    }
    

    /// <summary>
    /// Return the Top Left position
    /// </summary>
    /// <param name="pos"></param>
    public static Vector3Int GetTopLeft(Vector3Int pos)
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
    public static Vector3Int GetTopRight(Vector3Int pos)
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
    public static Vector3Int GetBottomLeft(Vector3Int pos)
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
    public static Vector3Int GetBottomRight(Vector3Int pos)
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
    public static Vector3Int GetRight(Vector3Int pos)
    {
        return pos + new Vector3Int(1, 0, 0);
    }

    /// <summary>
    /// Return the Right
    /// </summary>
    /// <param name="pos"></param>
    public static Vector3Int GetLeft(Vector3Int pos)
    {
        return pos + new Vector3Int(-1, 0, 0);
    }

}