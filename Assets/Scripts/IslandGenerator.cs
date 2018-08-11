using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandGenerator : MonoBehaviour {


    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tile sandTile;
    [SerializeField]
    private Texture2D heigtmap;
	// Use this for initialization
	void Start () {
        int[,] map = new int[,] { { 1, 0 ,1,1}, { 1, 0,0,0 }, { 1, 1,0,1 }, { 1, 1,0,1 } }; ;
        ReadImage();
        Vector3Int vectorPosition = Vector3Int.zero;
        for (int i = 0; i< 4; i++) {
            for(int j = 0; j < 4; j++)
            {
                if(map[i, j] == 1)
                {
                    vectorPosition.x = i;
                    vectorPosition.y = j;
                    Debug.Log(vectorPosition.x);
                    
                    groundTileMap.SetTile(vectorPosition, sandTile);
                }
               
            }
           
        }

	}

    private void ReadImage()
    {
        Texture2D image = heigtmap;
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
