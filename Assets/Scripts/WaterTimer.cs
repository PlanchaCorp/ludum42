using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaterTimer : MonoBehaviour {

    private TextMeshProUGUI timer;
    private WaterTide waterTide;

    // Use this for initialization
    void Start () {
        timer = GetComponent<TextMeshProUGUI>();
        waterTide = GameObject.FindGameObjectWithTag("WaterTilemap").GetComponent<WaterTide>();
    }
	
	// Update is called once per frame
	void Update () {
        float time = waterTide.GetTimeLeft();
        if (!waterTide.WillRise())
        {
            if (waterTide.IsStill())
            {
                time += 20.0f;
            } else
            {
                time += 30.0f;
            }
        } else if (!waterTide.IsStill())
        {
            time += 10.0f;
        }

        timer.SetText("Next Wave : " + time.ToString("F1"));
	}
}
