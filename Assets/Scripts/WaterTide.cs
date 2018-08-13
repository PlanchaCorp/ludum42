﻿using System.Collections;
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
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject erosion;
    [SerializeField] private GameObject mapManager;
    [SerializeField] private List<GameObject> pickUps;
    [SerializeField] private float bonusLuck = 0.01f;
    [SerializeField] private int startingLayer = 1;
    [SerializeField] private int actualLayer;
    [SerializeField] public int maxLayer = 6;
    [SerializeField] public int minLayer = 1;
    [SerializeField] public float period = 2.0f;

    private bool gonnaRise = false;
    private int rising = 0;    
    private float timeLeft;
    private TileInfo[,] terrainInfo;
    private List<TileInfo> submergedTiles = new List<TileInfo>();
    private List<TileInfo> justUnderWater = new List<TileInfo>();

    public List<TileInfo> seaTiles = new List<TileInfo>();

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
        
        water = GameObject.FindGameObjectWithTag("WaterTilemap");
        erosion = GameObject.FindGameObjectWithTag("Manager");
        actualLayer = startingLayer;

        //Invoke("Unflood", 0.5f);
        Invoke("Flood", 0.5f);
        Invoke("ChangeTide", period);
	}
	// Update is called once per frame
	void Update () {
        ChangeTideState();
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
        Flood();

        // Act as a clock, re invoke this method after 'period' seconds
        Invoke("ChangeTide", period);
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

    public float GetTimeLeft()
    {
        return timeLeft;
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
        timeLeft -= Time.deltaTime;
        if ( timeLeft <= 0 )
        {
            timeLeft = period;
            switch (state)
            {
                case TideState.STILL:
                    if (rising < 2)
                    {
                        state = TideState.RISING;
                        rising++;
                    } else
                    {
                        state = TideState.FALLING;
                        rising = 0;
                    }
                   
                    break;
                case TideState.FALLING:
                    state = TideState.STILL;
                    break;
                case TideState.RISING:
                    state = TideState.STILL;
                    break;
            }
        } 
    }

   /* public void UnFlood(){
        
        List<TileInfo> notDone = new List<TileInfo>();
        
        while(justUnderWater.Count() != 0){
            TileInfo tile = justUnderWater[0];
            justUnderWater.RemoveAt(0);
            if (tile.GetCoordinates()[0] == 0 ||
                 tile.GetCoordinates()[0] == terrainTilesInfo.GetLength(0) - 1 ||
                 tile.GetCoordinates()[1] == 0 ||
                 tile.GetCoordinates()[1] == terrainTilesInfo.GetLength(1) - 1)
            {

            }else{
                Debug.Log("hs,dbf,sdnfbg");
                Vector3Int[] neighbours = tile.GetNeighboursCoordinates();
                int S = 0;
                foreach(Vector3Int neighbour in neighbours){
                    if(terrainTilesInfo[neighbour.x,neighbour.y].GetIsFlooded()){
                        if(terrainTilesInfo[neighbour.x,neighbour.y].GetHeight() > actualLayer - 1 ){
                            notDone.Add(terrainTilesInfo[neighbour.x,neighbour.y]);
                            S += 1;
                        }
                   }
                   
                }
                if(S < 6){
                    tile.SetIsFlooded(false);
                }

            }
        }
        justUnderWater.AddRange(notDone);
        Invoke("Unflood",0.5f);
    }
    */    
        
    public void  Flood(){

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
                            n.SetIsFlooded(true);
                        } else if(n.ShallFlood())
                        {
                            n.SetIsFlooded(true);
                            tileModified.Add(n);
                        } else
                        {
                            n.SetIsFlooded(false);
                        }
                    }
                }
            }
        }
        submergedTiles = tileModified;
        Invoke("Flood", 0.5f);
    }
}
