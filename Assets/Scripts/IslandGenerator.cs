﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class IslandGenerator : MonoBehaviour
{
    
    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tilemap decorationTileMap;

    [SerializeField]
    private Tilemap waterTilemap;

    [SerializeField]
    private Tile[] sandTiles;

    [SerializeField]
    private Tile[] stoneTiles;

    [SerializeField]
    private Tile wall;

    [SerializeField]
    private AnimatedTile waterTile;

    [SerializeField]
    private List<Decoration> decorations;

    private int perlinSizeX;// Longueur du tableau
    private int perlinSizeY;// Largeur du tableau 

    public int mapSize = 40;

    private float K;

    private float scale = 7;

    public float seed;

    public int[,] map;

    public Tile[] GetSandTiles()
    {
        return sandTiles;
    }


    public List<Vector2Int> WaterLayerPostions;
    
    // Use this for initialization
    void Awake()
    {
        perlinSizeX = mapSize;// Longueur du tableau
        perlinSizeY = mapSize;// Largeur du tableau 
        K = 0.01f;
        seed = Random.Range(0, 999);
        map = GaussianMask(GeneratePerlin());
        GenerateIsland(map);
        WaterLayerPostions = new List<Vector2Int>();
       
    }

    public void PlacePickupOnMap()
    {
        for (int i = 0; i < 3; i++)
        {
            int rd = Random.Range(0, WaterLayerPostions.Count - 1);
            Vector2Int position = WaterLayerPostions[rd];
            WaterLayerPostions.RemoveAt(rd);
            //SetPickup(position.x, position.y);
        }
    }

    private int[,] GaussianMask(float[,] map)
    {
        int X0 = perlinSizeX / 2;//2;
        int Y0 = perlinSizeY / 2;//2;

        //Coefficient donnant la taille du masque (Plus K est grand, plus le masque est petit)
        int[,] mask = new int[perlinSizeX, perlinSizeY];
        for (int x = 0; x < perlinSizeX; x++)
        {
            for (int y = 0; y < perlinSizeY; y++)
            {
                mask[x, y] = Mathf.RoundToInt(map[x, y] * 7 * Mathf.Exp(-((x - X0) * (x - X0) + (y - Y0) * (y - Y0)) * K));
            }
        }
        return mask;
    }

    private void GenerateIsland(int[,] map)
    {
        Vector3Int vectorPosition = Vector3Int.zero;
        
        for (int i = 0; i < perlinSizeX; i++)
        {
           
            for (int j = 0; j < perlinSizeY; j++)
            {
                vectorPosition.x = mapSize / 2 - i;
                vectorPosition.y = mapSize / 2 - j;

                float xCoord = (i / (float) perlinSizeX)* scale;
                float yCoord = (j / (float) perlinSizeY) * scale;

                // Create a Stone or Sand
                Tile tile;
                if (Mathf.PerlinNoise(xCoord + seed, yCoord + seed) > 0.25f)
                {
                    tile = sandTiles[map[i, j]];
                }
                else
                {
                    tile = stoneTiles[map[i, j]];
                }
                groundTileMap.SetTile(vectorPosition, tile);


                // Create a decoration
                float rng = Random.Range(0, 20);
                if (rng < 1)
                {
                    List<Decoration> deco = decorations.Where(x => x.MinHeigth < map[i, j]).ToList();
                    if ( deco.Count > 0 )
                    {
                        decorationTileMap.SetTile(vectorPosition, deco[Random.Range(0, deco.Count - 1)].tile);
                    }
                }              
            }
        }
    }
   
    /// <summary>
    /// Generate a perlin noise for a 64*64 map size
    /// </summary>
    /// <returns>an array with values between 0 and 7</returns>
    public float[,] GeneratePerlin()
    {
        float[,] map = new float[perlinSizeX, perlinSizeY];
        float y = 0.0F;

        while (y < perlinSizeX)
        {
            float x = 0.0F;
            while (x < perlinSizeY)
            {
                float xCoord = x / perlinSizeX * scale;
                float yCoord = y / perlinSizeY * scale;
                map[(int)x, (int)y] = Mathf.PerlinNoise(seed + xCoord, seed + yCoord);

                x++;
            }
            y++;
        }

        return map;

    }

}
