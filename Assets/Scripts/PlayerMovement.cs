using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Speed of the player
    /// </summary>
    public float speed = 2.5f;

    /// <summary>
    /// Position of the last mouse click
    /// </summary>
    private Vector3 mouseClickPosition;
    /// <summary>
    /// Boolean asserting that player is currently moving
    /// </summary>
    private bool isMoving = false;
	
	// Update is called once per frame
	void Update () {
        UpdateMouseClick();
	}

    private void FixedUpdate()
    {
        UpdatePlayerPosition(Time.deltaTime);
    }

    /// <summary>
    /// Check mouse clicks and set mouseClickPosition
    /// </summary>
    void UpdateMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            mouseClickPosition = mousePositionRay.origin;
            isMoving = true;
        }
    }

    /// <summary>
    /// Move the player accordingly to the mouse click position
    /// </summary>
    /// <param name="elapsedTime">Time elapsed since last fixedUpdate</param>
    void UpdatePlayerPosition(float elapsedTime)
    {
        if (isMoving)
        {
            float totalMoveAmount = elapsedTime * speed;
            Vector3 distanceVector = mouseClickPosition - transform.position;
            float diagonalRatio = totalMoveAmount / (Mathf.Sqrt(Mathf.Pow(distanceVector.x, 2) + Mathf.Pow(distanceVector.y, 2)));
            float horizontalMoveAmount = diagonalRatio * distanceVector.x;
            float verticalMoveAmount = diagonalRatio * distanceVector.y;
            transform.Translate(horizontalMoveAmount, verticalMoveAmount, 0);
            // Check that we haven't exceeded the position
            Vector3 newDistanceVector = mouseClickPosition - transform.position;
            if (newDistanceVector.x * distanceVector.x < 0 && newDistanceVector.y * distanceVector.y < 0)
            {
                isMoving = false;
            }
        }
    }
}
