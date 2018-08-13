using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour {

    private WaterTide waterTide;
    private TextMeshProUGUI score;

    // Use this for initialization
    void Start () {
        waterTide = GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>();
        score = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
	}
	
	// Update is called once per frame
	void Update () {
		if (waterTide.GetActualLayer() == waterTide.GetMaxLayer())
        {
            StartCoroutine(SetScoreText());
        }
	}

    /// <summary>
    /// Assign score text to the textmeshprougui
    /// </summary>
    private IEnumerator SetScoreText()
    {
        score.SetText("Score : " + CalculateAvailableTiles());
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Calculate the tiles that are dry and without walls
    /// </summary>
    /// <returns>Tiles count</returns>
    public int CalculateAvailableTiles()
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
