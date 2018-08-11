using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHotbar : MonoBehaviour {

    [SerializeField] int switchStat;
    [SerializeField] GameObject switchSelector1;
    [SerializeField] GameObject switchSelector2;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("1"))
        {
            switchSelector1.SetActive(true);
            switchSelector2.SetActive(false);
            switchStat = 1;
        }
        if (Input.GetKey("2"))
        {
            switchSelector1.SetActive(false);
            switchSelector2.SetActive(true);
            switchStat = 2;
        }
    }
}
