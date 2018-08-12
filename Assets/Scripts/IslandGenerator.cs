using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class IslandGenerator : MonoBehaviour
{


    [SerializeField]
    private Tilemap groundTileMap;


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

    private int perlinSizeX;// Longueur du tableau
    private int perlinSizeY;// Largeur du tableau 

    public int mapSize = 40;

    private float K;

    private float scale = 7;

    public float seed;

    public int[,] map;

    private int waterLevel;

    // The origin of the sampled area in the plane.
    private float xOrg;
    private float yOrg;

    public Tile[] GetSandTiles()
    {
        return sandTiles;
    }

    public int GetWaterLevel()
    {
        return waterLevel;
    }

    // Use this for initialization
    void Awake()
    {

        perlinSizeX = mapSize;// Longueur du tableau
        perlinSizeY = mapSize;// Largeur du tableau 
        K = 0.01f;
        seed = Random.Range(0, 999);
        map = GaussianMask(GeneratePerlin());
        GenerateIsland(map);
    }

    private void Start()
    {
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
        for (int l = 0; l < scale; l++)
        {
            GameObject sandLayer = new GameObject();
            sandLayer.AddComponent<Tilemap>();
            sandLayer.AddComponent<TilemapRenderer>().sortingLayerName = "Terrain";
            sandLayer.GetComponent<TilemapRenderer>().sortingOrder = l;
            sandLayer.GetComponent<Tilemap>().tileAnchor = new Vector3(0f, 0f);
            sandLayer.GetComponent<Tilemap>().orientation = Tilemap.Orientation.Custom;
            sandLayer.transform.position = new Vector3(0f, l / 10f, 0f);

            sandLayer.tag = "TerrainTilemap";

            sandLayer.name = "SandLayer" + l;
            sandLayer.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            sandLayer.transform.SetParent(groundTileMap.transform);

        }
        Vector3Int vectorPosition = Vector3Int.zero;
        
        for (int i = 0; i < perlinSizeX; i++)
        {
           
            for (int j = 0; j < perlinSizeY; j++)
            {
                vectorPosition.x = mapSize / 2 - i;
                vectorPosition.y = mapSize / 2 - j;

                float xCoord = (xOrg + i / (float) perlinSizeX)* scale;
                float yCoord = (yOrg + j / (float) perlinSizeY) * scale;
                
                Tile tile;
                Debug.Log(xCoord+" "+ yCoord +" "+Mathf.PerlinNoise((float)xCoord + seed, (float)yCoord + seed));
                if (Mathf.PerlinNoise(xCoord + seed, yCoord + seed) > 0.25f)
                {

                    tile = sandTiles[map[i, j]];


                }
                else
                {
                    tile = stoneTiles[map[i, j]];

                }
                groundTileMap.transform.Find("SandLayer" + map[i, j]).GetComponent<Tilemap>().SetTile(vectorPosition, tile);


              
            }
        }
    }

    public void SetWater(int level)
    {
        waterLevel = level;
        Vector3Int vectorPosition = Vector3Int.zero;
        for (int i = 0; i < perlinSizeX; i++)
        {
            for (int j = 0; j < perlinSizeY; j++)
            {
                vectorPosition.x = mapSize / 2 - i;
                vectorPosition.y = mapSize / 2 - j;
                if (map[i, j] <= level)
                {
                    waterTilemap.SetTile(vectorPosition, waterTile);
                }
                else
                {
                    waterTilemap.SetTile(vectorPosition, wall);
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
                float xCoord = xOrg + x / perlinSizeX * scale;
                float yCoord = yOrg + y / perlinSizeY * scale;
                map[(int)x, (int)y] = Mathf.PerlinNoise(seed + xCoord, seed + yCoord);

                x++;
            }
            y++;
        }

        return map;

    }

}
