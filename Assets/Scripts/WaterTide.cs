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
    [SerializeField] private GameObject erosion;
    [SerializeField] private float risingTimer;

    private int startingLayer = 2;
    public int maxLayer = 6;
    public int minLayer = 0;
    public float period = 2.0f;
    private bool gonnaRise = false;
    private float timeLeft;

    private readonly string waterTag = "WaterTilemap";
    private readonly string managerTag = "Manager";

    // Use this for initialization
    void Start () {
        water = GameObject.FindGameObjectWithTag(waterTag);
        erosion = GameObject.FindGameObjectWithTag(managerTag);
        if (water != null)
        {
            startingLayer = water.GetComponent<TilemapRenderer>().sortingOrder;
            state = TideState.STILL;
            risingTimer = 4.0f;
            timeLeft = risingTimer;
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
        Tide();
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
                }
                break;
            case TideState.RISING:
                if (actualLayer < maxLayer)
                {
                    actualLayer++;
                }
                break;
        }
        UpdateTideLayer();
        erosion.GetComponent<ErosionManager>().Erode();

        // Act as a clock, re invoke this method after 'period' seconds
        Invoke("ChangeTide", period);
    }

    private void UpdateTideLayer()
    {
        water.GetComponent<TilemapRenderer>().sortingOrder = actualLayer;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public bool WillRise()
    {
        return gonnaRise;
    }

    public bool IsStill()
    {
        return state == TideState.STILL;
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
            Debug.Log("Change the water state.");
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
}
