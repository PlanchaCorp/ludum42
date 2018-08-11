using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class IslandGenerator : MonoBehaviour
{


    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tile[] sandTile;

    private int perlinSizeX ;// Longueur du tableau
    private int perlinSizeY ;// Largeur du tableau 

    public int mapSize = 40;

    private float K;

    private float scale = 7;

    public float seed;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;



    // Use this for initialization
    void Start()
    {

        perlinSizeX = mapSize;// Longueur du tableau
        perlinSizeY = mapSize;// Largeur du tableau 
        K = mapSize / 2;
        seed = Random.Range(0, 999);
        GenerateIsland(GaussianMask(GeneratePerlin()));
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
            sandLayer.AddComponent<TilemapRenderer>();
            sandLayer.GetComponent<TilemapRenderer>().sortingOrder = l;
            sandLayer.name = "SandLayer" + l;
            sandLayer.transform.SetParent(groundTileMap.transform);
        }
        
        Vector3Int vectorPosition = Vector3Int.zero;
        for (int i = 0; i < perlinSizeX; i++)
        {
            for (int j = 0; j < perlinSizeY; j++)
            {
                vectorPosition.x = mapSize / 2 - i;
                vectorPosition.y = mapSize / 2 - j;
                groundTileMap.transform.Find("SandLayer"+ map[i,j]).GetComponent<Tilemap>().SetTile(vectorPosition, sandTile[map[i, j]]);
             

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

    private void ReadImage()
    {
        Texture2D image = (Texture2D)Resources.Load("texture");
        Debug.Log(image);

        // Iterate through it's pixels
        for (int i = 0; i < image.width; i++)
        {
            for (int j = 0; j < image.height; j++)
            {
                Color pixel = image.GetPixel(i, j);

                // if it's a white color then just debug...
                if (pixel == Color.white)
                {
                    Debug.Log("Im white");
                }
                else
                {
                    Debug.Log("Im black");
                }
            }
        }
    }


}
