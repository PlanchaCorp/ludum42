﻿﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTide : MonoBehaviour {
    public enum TideState
    {
        RISING,
        FALLING,
        STILL
    }

    [SerializeField] private AnimatedTile waterTile;
    [SerializeField] private TideState state;
    [SerializeField] private GameObject erosion;
    [SerializeField] private GameObject mapManager;

    [SerializeField] private List<GameObject> pickUps;
    [SerializeField] private float bonusLuck = 0.01f;
    
    [SerializeField] private float period = 2.0f;
    private bool gonnaRise = false;
    [SerializeField] private int rising = 1;
    private int maxLayer = 6;
    private int minLayer = 1;
    private int startingLayer = 2;
    [SerializeField] private int actualLayer = 2;
    [SerializeField] private bool maxLayerReached = false;

    private TileInfo[,] terrainInfo;

    private List<TileInfo> submergedTiles = new List<TileInfo>();
    private List<TileInfo> justUnderWater = new List<TileInfo>();
    private List<TileInfo> seaTiles = new List<TileInfo>();

    // Use this for initialization
    void Start () {
        terrainInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo();
        for(int i = 0; i<terrainInfo.GetLength(0);i++ ){
           for(int j = 0; j<terrainInfo.GetLength(1);j++ ){
                if(terrainInfo[i,j].GetIsSea())
                {
                    seaTiles.Add(terrainInfo[i,j]);
                    submergedTiles.Add(terrainInfo[i, j]);
                }
                else if(terrainInfo[i,j].GetHeight() >= actualLayer &&
                        terrainInfo[i,j].GetIsFlooded())
                {
                    justUnderWater.Add(terrainInfo[i,j]);
                }  
           }
        }
        
        erosion = GameObject.FindGameObjectWithTag("Manager");
        actualLayer = startingLayer;

        //Invoke("Unflood", 0.5f);
        Invoke("Flood", 0.5f);
        Invoke("ApplyTide", period);
    }
	// Update is called once per frame
	void Update () {
        CheatyStateSettings();
    }   

    private void ApplyTide()
    {
        switch(state)
        {
            case TideState.STILL:
                break;
            case TideState.FALLING:
                if (actualLayer > minLayer)
                {
                    actualLayer--;
                    Fall();
                }
                break;
            case TideState.RISING:
                if (actualLayer < maxLayer)
                {
                    actualLayer++;
                }
                break;
        }

        submergedTiles.Clear();
        submergedTiles.AddRange(seaTiles);

        erosion.GetComponent<ErosionManager>().Erode();
        ChangeTideState();
        Flood();

        // Act as a clock, re invoke this method after 'period' seconds
        Invoke("ApplyTide", period);
    }

    private void RollTheBonusDice(int x, int y)
    {
        if (GameObject.FindGameObjectsWithTag("Bonus").Length < 10)
        {
            float dice = Random.Range(0f, 1f);
            if (dice <= bonusLuck)
            {
                int pickUpNumber = Mathf.FloorToInt(Random.Range(0, pickUps.Capacity));
                Vector3Int newTileVector = mapManager.GetComponent<MapManager>().DataToTilesCoordinates(new Vector3Int(x, y, 0));
                pickUps[pickUpNumber].transform.position = newTileVector;
                Instantiate(pickUps[pickUpNumber]);
            }
        }
    }
    
    public int GetActualLayer()
    {
        return actualLayer;
    }

    public int GetMaxLayer()
    {
        return maxLayer;
    }

    public bool WillRise()
    {
        return gonnaRise;
    }

    public bool IsStill()
    {
        return state == TideState.STILL;
    }

    public int GetLevel()
    {
        return actualLayer;
    }
    
    public void SetTideState(TideState state)
    {
        this.state = state;
    }

    public float GetPeriod()
    {
        return this.period;
    }

    private void CheatyStateSettings()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            state = TideState.FALLING;
            Debug.Log("Change to FALLING");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            state = TideState.STILL;
            Debug.Log("Change to STILL");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            state = TideState.RISING;
            Debug.Log("Change to RISING");
        }
    }

    private void ChangeTideState()
    {
        switch (state)
        {
            case TideState.STILL:
                if (rising == 3)
                {
                    rising = 0;
                    if (maxLayerReached)
                    {
                        state = TideState.RISING;
                    }
                    else
                    {
                        state = TideState.FALLING;
                    }
                } else
                {
                    if (maxLayerReached)
                    {
                        state = TideState.FALLING;
                    }
                    else
                    {
                        state = TideState.RISING;
                    }
                }
                rising++;
                break;
            default:
                state = TideState.STILL;
                break;
        }
        if ( actualLayer == maxLayer )
        {
            maxLayerReached = true;
            rising = 1;
        } else if (actualLayer == minLayer )
        {
            maxLayerReached = false;
            rising = 1;
        }
    }
  
    public void Flood(){

        terrainInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo(); 
        List<TileInfo> tileDone = new List<TileInfo>();
        List<TileInfo> tileModified = new List<TileInfo>();
        while(submergedTiles.Count() != 0){
            TileInfo tile = submergedTiles[0];
            submergedTiles.RemoveAt(0);
            if (tile.GetCoordinates()[0] == 0 ||
                 tile.GetCoordinates()[0] == terrainInfo.GetLength(0) - 1 ||
                 tile.GetCoordinates()[1] == 0 ||
                 tile.GetCoordinates()[1] == terrainInfo.GetLength(1) - 1)
            {

            }else{
                Vector3Int[] neighbours = tile.GetNeighboursCoordinates();
                foreach(Vector3Int neighbour in neighbours){
                    TileInfo n = terrainInfo[neighbour.x, neighbour.y];
                    if (!tileDone.Contains(n))
                    {
                        tileDone.Add(n);
                        if (n.GetIsFlooded())
                        {
                            submergedTiles.Add(n);
                        } else if(n.ShallFlood())
                        {
                            n.SetIsFlooded(true);
                            tileModified.Add(n);
                        }
                        n.DisplayWater();
                    }
                }
            }
        }
        submergedTiles = tileModified;
        Invoke("Flood", 0.5f);
    }

    private void Fall()
    {
        TileInfo[,] terrain = mapManager.GetComponent<MapManager>().GetTerrainInfo();
        for (int i = 0; i < terrain.GetLength(0); i++)
        {
            for (int j = 0; j < terrain.GetLength(1); j++)
            {
               if ( terrain[i, j].GetHeight() == actualLayer ||
                    terrain[i, j].GetHeight() == actualLayer + 1)
                {
                    terrain[i, j].SetIsFlooded(false);
                }
            }
        }
    }
}
