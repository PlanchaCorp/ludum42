using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTide : MonoBehaviour {
    
    public enum TideState
    {
        RISING,
        FALLING,
        STILL
    }

    [SerializeField] private TideState state;
    [SerializeField] private int actualLayer;
    [SerializeField] private GameObject water;

    private int startingLayer = 2;
    public int maxLayer = 6;
    public int minLayer = 0;
    public float period = 5.0f;

    private readonly string waterTag = "WaterTilemap";

    // Use this for initialization
    void Start () {
        water = GameObject.FindGameObjectWithTag(waterTag);
        if (water != null)
        {
            startingLayer = water.GetComponent<TilemapRenderer>().sortingOrder;
            state = TideState.STILL;
            actualLayer = startingLayer;
            // Change the state of the tide with the current state selected (after 'period' seconds)
            Invoke("ChangeTide", period);
        } else
        {
            Debug.LogError("Could not get Water with Tag " + waterTag);
        }
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
                Debug.Log("Tide staying still.");
                break;
            case TideState.FALLING:
                Debug.Log("Tide is falling.");
                if (actualLayer > minLayer)
                {
                    actualLayer--;
                }
                break;
            case TideState.RISING:
                Debug.Log("Tide is rising, carefull!");
                if (actualLayer < maxLayer)
                {
                    actualLayer++;
                }
                break;
        }
        UpdateTideLayer();
        // Act as a clock, re invoke this method after 'period' seconds
        Invoke("ChangeTide", period);
    }

    private void UpdateTideLayer()
    {
        water.GetComponent<TilemapRenderer>().sortingOrder = actualLayer;
    }

    public void SetTideState(TideState state)
    {
        this.state = state;
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
}
