using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class IslandGenerator : MonoBehaviour
{


    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tile[] sandTiles;

    [SerializeField]
    private Tile[] stoneTiles;

    private int perlinSizeX ;// Longueur du tableau
    private int perlinSizeY ;// Largeur du tableau 

    public int mapSize = 40;

    private float K;

    private float scale = 7;

    public float seed;

    public int[,] map;

    // The origin of the sampled area in the plane.
    private float xOrg;
    private float yOrg;

    public Tile[] GetSandTiles()
    {
        return sandTiles;
    }

    // Use this for initialization
    void Awake()
    {
        Debug.Log("awake");
        perlinSizeX = mapSize;// Longueur du tableau
        perlinSizeY = mapSize;// Largeur du tableau 
        K = mapSize / 2;
        seed = Random.Range(0, 999);
        map = GaussianMask(GeneratePerlin());
    }

    private void Start()
    {
        Debug.Log("start");
        GenerateIsland(map);
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
                mask[x, y] = Mathf.Max(Mathf.RoundToInt(map[x, y] * (-Mathf.Sqrt((x - X0) * (x - X0) + (y - Y0) * (y - Y0)) / K + 1) * 7), 0);
            }

        }
        return mask;
    }

    private void GenerateIsland(int[,] map)
    {
        for (int l =0; l< scale; l++) {
            GameObject sandLayer = new GameObject();
            sandLayer.AddComponent<Tilemap>();
            sandLayer.AddComponent<TilemapRenderer>().sortingLayerName = "Terrain";
            sandLayer.GetComponent<TilemapRenderer>().sortingOrder = l;
            sandLayer.GetComponent<Tilemap>().tileAnchor = Vector3.zero;
            sandLayer.tag = "TerrainTilemap";
            
            sandLayer.name = "SandLayer" + l;
            sandLayer.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
            sandLayer.transform.SetParent(groundTileMap.transform);
            
        }

        

   

        Vector3Int vectorPosition = Vector3Int.zero;
        for (int i = 0; i < perlinSizeX; i++)
        {
            for (int j = 0; j < perlinSizeY; j++)
            {
                vectorPosition.x = mapSize/2-  i;
                vectorPosition.y = mapSize / 2 - j;
                if (Mathf.PerlinNoise(i + seed, j + seed) < 0.25f)
                {
                    groundTileMap.transform.Find("SandLayer" + map[i, j]).GetComponent<Tilemap>().SetTile(vectorPosition, stoneTiles[map[i, j]]);
                }
                else
                {
                    groundTileMap.transform.Find("SandLayer" + map[i, j]).GetComponent<Tilemap>().SetTile(vectorPosition, sandTiles[map[i, j]]);
                }
                
                if(map[i, j] < 3)
                {

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
