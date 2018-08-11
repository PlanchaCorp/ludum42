using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class IslandGenerator : MonoBehaviour
{


    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tile sandTile;

    [SerializeField]
    private AnimatedTile waterTile;

    public int perlinSizeX = 64;// Longueur du tableau
    public int perlinSizeY = 64;// Largeur du tableau 


    public float K = 64;

    public float scale = 7;

    public float seed = Random.Range(0, 999);

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;



    // Use this for initialization
    void Start()
{

    

    GenerateIsland(generatePerlin());
}


    private float[,] GaussianMask()
    {
        int X0 = perlinSizeY;//2;
        int Y0 = perlinSizeY;//2;

         //Coefficient donnant la taille du masque (Plus K est grand, plus le masque est petit)
        float[,] mask = new float[perlinSizeX, perlinSizeY];
        for (int x = 0; x < perlinSizeX; x++)
        {
            for (int y = 0; y < perlinSizeY; y++)
            {
                mask[x, y] = Mathf.Exp(-((x - X0) * (x - X0) + (y - Y0) * (y - Y0) / K));
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
                vectorPosition.x = i;
                vectorPosition.y = j;
                if (map[i,j] > 2)
                {
                    groundTileMap.SetTile(vectorPosition, sandTile);
                }
                else {
                    groundTileMap.SetTile(vectorPosition, waterTile);
                }

            }
        }
    }
/// <summary>
/// Generate a perlin noise for a 64*64 map size
/// </summary>
/// <returns>an array with values between 0 and 7</returns>
public int[,] generatePerlin()
{
    int[,] map = new int[perlinSizeX, perlinSizeY];
          float y = 0.0F;

        while (y < perlinSizeX)
        {
            float x = 0.0F;
            while (x < perlinSizeY)
            {
                float xCoord = xOrg + x / perlinSizeX * scale;
                float yCoord = yOrg + y / perlinSizeY * scale;
                map[(int)x, (int)y] = Mathf.RoundToInt(Mathf.PerlinNoise(seed+xCoord,seed+ yCoord)*7);
                Debug.Log(map[(int)x, (int)y]);
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
