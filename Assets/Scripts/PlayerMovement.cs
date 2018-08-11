using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Speed of the player
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// Position of the last mouse click
    /// </summary>
    private Vector3 mouseClickPosition;
	
	// Update is called once per frame
	void Update () {
        UpdateMouseClick();
        UpdatePlayerPosition();
	}

    /// <summary>
    /// Check mouse clicks and set mouseClickPosition
    /// </summary>
    void UpdateMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            mouseClickPosition = mousePositionRay.origin;
        }
    }

    /// <summary>
    /// Move the player accordingly to the mouse click position
    /// </summary>
    void UpdatePlayerPosition()
    {

    }
}
