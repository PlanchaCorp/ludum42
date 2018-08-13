using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour {

    private WaterTide waterTide;

	// Use this for initialization
	void Start () {
        waterTide = GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>();
	}
	
	// Update is called once per frame
	void Update () {
		if (waterTide.GetActualLayer() == waterTide.GetMaxLayer())
        {
            Debug.Log(calculateAvailableTiles());
        }
	}

    /// <summary>
    /// Calculate the tiles that are dry and without walls
    /// </summary>
    /// <returns>Tiles count</returns>
    public int calculateAvailableTiles()
    {
        int dryCount = 0;
        TileInfo[,] terrainInfo = GameObject.FindGameObjectWithTag("Manager").GetComponent<MapManager>().GetTerrainInfo();
        for (int x = 0; x < terrainInfo.GetLength(0); x += 1)
        {
            for (int y = 0; y < terrainInfo.GetLength(1); y += 1)
            {
                if (!terrainInfo[x, y].GetIsFlooded() && terrainInfo[x, y].GetWallState() == TileInfo.WallState.NOTHING)
                {
                    dryCount++;
                }
            }
        }
        return dryCount;
    }
}
