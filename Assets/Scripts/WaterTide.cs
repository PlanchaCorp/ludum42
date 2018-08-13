﻿﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTide : MonoBehaviour {
    public List<TileInfo> seaTiles = new List<TileInfo>();
    public enum TideState
    {
        RISING,
        FALLING,
        STILL
    }

    [SerializeField] private AnimatedTile waterTile;
    [SerializeField] private TideState state;
    [SerializeField] private int actualLayer;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject erosion;
    [SerializeField] private float risingTimer;
    [SerializeField] private GameObject mapManager;
    [SerializeField] private List<GameObject> pickUps;
    [SerializeField] private float bonusLuck = 0.01f;
    [SerializeField] private int startingLayer = 2;
    [SerializeField] public int maxLayer = 6;
    [SerializeField] public int minLayer = 1;
    [SerializeField] public float period = 10.0f;
    private bool gonnaRise = false;
    private float timeLeft;
    private TileInfo[,] terrainTilesInfo;
    private List<TileInfo> submergedTiles = new List<TileInfo>();
    private List<TileInfo> justUnderWater = new List<TileInfo>();

    private readonly string waterTag = "WaterTilemap";
    private readonly string managerTag = "Manager";
    // Use this for initialization
    void Start () {
        terrainTilesInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo();
        for(int i = 0; i<terrainTilesInfo.GetLength(0);i++ ){
           for(int j = 0; j<terrainTilesInfo.GetLength(1);j++ ){
                if(terrainTilesInfo[i,j].GetIsSea()){
                    seaTiles.Add(terrainTilesInfo[i,j]);
                }else if(terrainTilesInfo[i,j].GetHeight() > actualLayer - 1 && terrainTilesInfo[i,j].GetIsFlooded()){
                    justUnderWater.Add(terrainTilesInfo[i,j]);
                }  
           }
        }
        
        water = GameObject.FindGameObjectWithTag(waterTag);
        erosion = GameObject.FindGameObjectWithTag(managerTag);
        submergedTiles.AddRange(seaTiles);
        actualLayer = startingLayer;
        //Invoke("Unflood", 0.5f);
        Invoke("Flood", 0.5f);
        Invoke("ChangeTide", period);
	}
	// Update is called once per frame
	void Update () {
        CheatyStateSettings();
    }   

    private void ChangeTide()
    {
        switch(state)
        {
            case TideState.STILL:
                break;
            case TideState.FALLING:
                if (actualLayer > minLayer)
                {
                actualLayer--;
                for(int i = 0; i<terrainTilesInfo.GetLength(0);i++ ){
                    for(int j = 0; j<terrainTilesInfo.GetLength(1);j++ ){
                            if(terrainTilesInfo[i,j].ShallFlood())
                            {
                                RollTheBonusDice(i, j);
                                terrainTilesInfo[i,j].SetIsFlooded(false);
                            }  
                        }
                    }
                }
                break;
            case TideState.RISING:
                if (actualLayer < maxLayer)
                {
                    actualLayer++;
                    submergedTiles = new List<TileInfo>();
                    submergedTiles.AddRange(seaTiles);
                }
                break;
        }
        //UpdateTideLayer();
        erosion.GetComponent<ErosionManager>().Erode();
        Tide();

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

    public float GetRisingTimer()
    {
        return this.risingTimer;
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

    private void Tide()
    {
        timeLeft -= Time.deltaTime;
        if ( timeLeft <= 0 )
        {
            timeLeft = risingTimer;
            switch (state)
            {
                case TideState.STILL:
                    if (gonnaRise)
                    {
                        state = TideState.RISING;
                    } else
                    {
                        state = TideState.FALLING;
                    }
                    gonnaRise = !gonnaRise;
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

        terrainTilesInfo = mapManager.GetComponent<MapManager>().GetTerrainInfo(); 

        List<TileInfo> tileDone = new List<TileInfo>();
        List<TileInfo> tileModified = new List<TileInfo>();

        while(submergedTiles.Count() != 0){

            // Get the first Tile
            TileInfo tile = submergedTiles[0];
            submergedTiles.RemoveAt(0);

            // Check if the tile is not out of Map Bounds
            if (tile.GetCoordinates()[0] == 0 ||
                 tile.GetCoordinates()[0] == terrainTilesInfo.GetLength(0) - 1 ||
                 tile.GetCoordinates()[1] == 0 ||
                 tile.GetCoordinates()[1] == terrainTilesInfo.GetLength(1) - 1)
            {

            }else{

                // Get the neighbours of the tile
                Vector3Int[] neighbours = tile.GetNeighboursCoordinates();
                foreach(Vector3Int neighbour in neighbours){
                    TileInfo neighbourInfo = terrainTilesInfo[neighbour.x, neighbour.y];
                    // If neighbours hasn't already been done
                    if (!tileDone.Contains(neighbourInfo))
                    {
                        // Check if the neighbour is flooded
                        if(neighbourInfo.GetIsFlooded())
                        {
                            // Neighbour is Flooded =>
                            // Mark it as Done
                            submergedTiles.Add(neighbourInfo);
                            tileDone.Add(neighbourInfo);

                        } else if (neighbourInfo.ShallFlood(actualLayer))
                        {
                            neighbourInfo.SetIsFlooded(true);
                            tileDone.Add(neighbourInfo);
                            tileModified.Add(neighbourInfo);
                        }
                   }
                }
            }
        }
        submergedTiles = tileModified;
        Invoke("Flood", 0.5f);
    }
}
