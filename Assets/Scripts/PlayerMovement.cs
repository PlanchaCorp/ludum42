using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Speed of the player
    /// </summary>
    [SerializeField]
    private float speed = 2.5f;

    /// <summary>
    /// Position of the last mouse click
    /// </summary>
    private Vector3 mouseClickPosition;

    /// <summary>
    /// Boolean asserting that player is currently moving
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Bonus activation for speed boost
    /// </summary>
    private bool quarterSpeedBoost = false;

    // Update is called once per frame
    void Update () {
        UpdateMouseClick();
	}

    private void FixedUpdate()
    {
        UpdatePlayerPosition(Time.deltaTime);
    }

    /// <summary>
    /// Check mouse clicks and call the SetMouseClick method
    /// </summary>
    private void UpdateMouseClick()
    {
        if (Input.GetMouseButton(1))
        {
            Ray mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            SetMouseClick(mousePositionRay.origin);
        }
    }
    
    /// <summary>
    /// Set new direction position of the player
    /// </summary>
    /// <param name="position">Position of the new direction of the player</param>
    public void SetMouseClick(Vector3 position)
    {
        gameObject.GetComponent<Animator>().SetBool("isMoving", true);
        mouseClickPosition = position;
        isMoving = true;
    }

    /// <summary>
    /// Move the player accordingly to the mouse click position
    /// </summary>
    /// <param name="elapsedTime">Time elapsed since last fixedUpdate</param>
    private void UpdatePlayerPosition(float elapsedTime)
    {
        if (isMoving)
        {
            float totalMoveAmount = (quarterSpeedBoost) ? elapsedTime * speed * 5f : elapsedTime * speed;
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
        } else
        {
            gameObject.GetComponent<Animator>().SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Activate or deactivate quarter speed bonus boost
    /// </summary>
    /// <param name="value">Activation or deactivation</param>
    public void SetQuarterSpeedBonus(bool value)
    {
        this.quarterSpeedBoost = value;
    }
}
