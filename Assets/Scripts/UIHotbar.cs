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
    [SerializeField] Texture2D digCursor;


    // Use this for initialization
    void Start ()
    {
        SelectShovel();
    }
	
	// Update is called once per frame
	void Update ()
    {
        int choice = switchStat;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            choice--;
            if (choice < 1)
            {
                choice = 3;
            }
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            choice++;
            if (choice > 3)
            {
                choice = 1;
            }
        }
        if (Input.GetKey("1"))
        {
            choice = 1;
        }
        if (Input.GetKey("2"))
        {
            choice = 2;
        }
        if(Input.GetKey("3"))
        {
            choice = 3;
        }
        switch (choice)
        {
            case 1:
                SelectShovel();
                break;
            case 2:
                SelectBucket();
                break;
            case 3:
            SelectHammer();
                break;
        }
        if (DigBar.value > 0)
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
        Cursor.SetCursor(digCursor, new Vector2(0, 4), CursorMode.Auto);
    }

    private void SelectHammer()
    {
        switchSelector2.sprite = Sprite1;
        switchSelector1.sprite = Sprite1;
        switchSelector3.sprite = Sprite2;
        switchStat = 3;
    }

    public int GetSwitch()
    {
        return switchStat;
    }
}
