using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour {

    private WaterTide waterTide;
    private TextMeshProUGUI score;
    private TextMeshProUGUI wave;
    public static float scoreValue = 0;
    public static int waveNumber = 0;
    private bool waveChecked = false;

    // Use this for initialization
    void Start () {
        waterTide = GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>();
        score = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        wave = GameObject.FindGameObjectWithTag("WaveNumber").GetComponent<TextMeshProUGUI>();
    }
	
	// Update is called once per frame
	void Update () {
		if (waterTide.GetActualLayer() == waterTide.GetMaxLayer())
        {
            if (!waveChecked)
            {
                waveNumber++;
                SetScoreText();
                SetWaveText();
                waveChecked = true;
            }
        } else
        {
            waveChecked = false;
        }
	}

    /// <summary>
    /// Assign score text to the textmeshprougui
    /// </summary>
    private void SetScoreText()
    {
        int newScore = CalculateAvailableTiles();
        if (scoreValue < newScore)
        {
            scoreValue = newScore;
            score.SetText("Score : " + newScore);
        }
    }

    /// <summary>
    /// Assign wave number to the textmeshprougui
    /// </summary>
    private void SetWaveText()
    {
        wave.SetText("Wave " + waveNumber);
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
