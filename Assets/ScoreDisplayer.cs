using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log(ScoreHandler.waveNumber);
        gameObject.GetComponent<TextMeshProUGUI>().text = "Score : " + ScoreHandler.scoreValue.ToString() + " Wave Survived : " + ScoreHandler.waveNumber.ToString();
	}
	

}
