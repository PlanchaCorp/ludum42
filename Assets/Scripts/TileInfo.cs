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

    public TileInfo(Vector3Int vector){
        this.durability = 1.0f;
        this.isFlooded = false;
        this.x = vector.x;
        this.y = vector.y;
    }
     public TileInfo(Vector3Int vector, int height, bool flooded, bool isSea){
        this.durability = 1.0f;
        this.isFlooded = flooded;
        this.isSea = isSea;
		this.height = height;
        this.x = vector.x;
        this.y = vector.y;
         if(flooded){
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
        Vector3Int[] neighbours = new Vector3Int[6];
        neighbours[0] = new Vector3Int(this.x + 1, this.y - 1, 0);
        neighbours[1] = new Vector3Int(this.x + 1, this.y, 0);
        neighbours[2] = new Vector3Int(this.x, this.y + 1, 0);
        neighbours[3] = new Vector3Int(this.x - 1, this.y + 1, 0);
        neighbours[4] = new Vector3Int(this.x - 1, this.y, 0);
        neighbours[5] = new Vector3Int(this.x, this.y - 1, 0);
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
        if(value){
            Vector3Int vectorPosition = new Vector3Int(0,0,0);
            vectorPosition.x = TileInfo.mapSize / 2 - this.x;
            vectorPosition.y = TileInfo.mapSize / 2 - this.y;
            TileInfo.waterTileMap.SetTile(vectorPosition,waterTile);
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

}