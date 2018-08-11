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
    void Start ()
    {
        SelectShovel();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("1"))
        {
            SelectShovel();
        }
        if (Input.GetKey("2"))
        {
            SelectBucket();
        }
        if(Input.GetKey("3"))
        {
            SelectHammer();
        }
        if(DigBar.value > 0)
        {
            IconDigBar.SetActive(true);
        }
        else
        {
            IconDigBar.SetActive(false);
        }
        if (BreathBar.value < 100)
        {
            IconBreathBar.SetActive(true);
        }
        else
        {
            IconBreathBar.SetActive(false);
        }
    }

    private void SelectShovel()
    {
        switchSelector1.sprite = Sprite2;
        switchSelector2.sprite = Sprite1;
        switchSelector3.sprite = Sprite1;
        switchStat = 1;
    }

    private void SelectBucket()
    {
        switchSelector2.sprite = Sprite2;
        switchSelector1.sprite = Sprite1;
        switchSelector3.sprite = Sprite1;
        switchStat = 2;
    }

    private void SelectHammer()
    {
        switchSelector2.sprite = Sprite1;
        switchSelector1.sprite = Sprite1;
        switchSelector3.sprite = Sprite2;
        switchStat = 3;
    }
}
