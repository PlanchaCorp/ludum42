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
                time += waterTide.GetPeriod() * 2;
            } else
            {
                time += waterTide.GetPeriod() * 3;
            }
        } else if (!waterTide.IsStill())
        {
            time += waterTide.GetPeriod();
        }

        timer.SetText("Next Wave : " + time.ToString("F1"));
	}
}
