using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIHotbar : MonoBehaviour {

    [SerializeField] int switchStat;
    [SerializeField] Image switchSelector1;
    [SerializeField] Image switchSelector2;
    [SerializeField] Image switchSelector3;
    [SerializeField] Sprite Sprite1 ;
    [SerializeField] Sprite Sprite2 ;
    [SerializeField] Slider BreathBar;
    [SerializeField] Slider DigBar;
    [SerializeField] GameObject IconDigBar;
    [SerializeField] GameObject IconBreathBar;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("1"))
        {
            switchSelector1.sprite = Sprite2;
            switchSelector2.sprite = Sprite1;
            switchSelector3.sprite = Sprite1;
            switchStat = 1;
        }
        if (Input.GetKey("2"))
        {
            switchSelector2.sprite = Sprite2;
            switchSelector1.sprite = Sprite1;
            switchSelector3.sprite = Sprite1;
            switchStat = 2;
        }
        if(Input.GetKey("3"))
        {
            switchSelector2.sprite = Sprite1;
            switchSelector1.sprite = Sprite1;
            switchSelector3.sprite = Sprite2;
            switchStat = 3;
        }
        if(DigBar.value > 0)
        {
            IconDigBar.SetActive(true);
        }
        else
        {
            IconDigBar.SetActive(false);
        }
        if (BreathBar.value > 0)
        {
            IconBreathBar.SetActive(true);
        }
        else
        {
            IconBreathBar.SetActive(false);
        }
    }
}
