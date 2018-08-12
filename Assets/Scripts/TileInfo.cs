using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo{

    private int x, y, height;
    private bool isFlooded, isSea;
    private float durability;

    private static Tilemap waterTileMap;
    private static AnimatedTile waterTile;
    private static int mapSize;

    public enum WallSate
    {
        NOTHING,
        TOWER,
        WALL_HORIZONTAL,
        WALL_LEFT,
        WALL_RIGHT
    }
    private WallSate state;

    public TileInfo(Vector3Int vector){
        this.durability = 1.0f;
        this.isFlooded = false;
        this.x = vector.x;
        this.y = vector.y;
        this.state = WallSate.NOTHING;
    }
     public TileInfo(Vector3Int vector, int height, bool flooded, bool isSea){
        this.durability = 1.0f;
        this.isFlooded = flooded;
        this.isSea = isSea;
		this.height = height;
        this.x = vector.x;
        this.y = vector.y;
        this.state = WallSate.NOTHING;
         if(isSea){
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
        neighbours[0] = new Vector3Int(this.x, this.y+1,0);
        neighbours[1] = new Vector3Int(this.x, this.y-1,0);
        neighbours[2] = new Vector3Int(this.x-1, this.y,0);
        neighbours[3] = new Vector3Int(this.x+1, this.y,0);
        neighbours[4] = new Vector3Int(this.x-1+this.y%2,this.y+1,0);
        neighbours[5] = new Vector3Int(this.x-1+this.y%2,this.y-1,0);
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
        Vector3Int vectorPosition = new Vector3Int(0,0,0);
        vectorPosition.x = TileInfo.mapSize / 2 - this.x;
        vectorPosition.y = TileInfo.mapSize / 2 - this.y;
        if(value){
            TileInfo.waterTileMap.SetTile(vectorPosition,waterTile);
        }else{
            TileInfo.waterTileMap.SetTile(vectorPosition,null);
        }
    }
	public void Dig(){
		this.height -= 1;
	}
	public void Rep(){
		this.height += 1;
	}
    public void DecreaseDurability(float value)
    {
        this.durability -= value;
    }

    public float GetDurability()
    {
        return this.durability;
    }

    public void ResetDurability()
    {
        this.durability = 1.0f;
    }

    public void SetWallState( WallSate state)
    {
        this.state = state;
    }

    public WallSate GetWallState()
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