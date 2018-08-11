using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAction : MonoBehaviour {
    /// <summary>
    /// Range within which the player can interact
    /// </summary>
    public float actionRange = 3.5f;

    /// <summary>
    /// Tilemap of the terrain
    /// </summary>
    private Tilemap terrainTilemap;

    // Use this for initialization
    void Start ()
    {
        terrainTilemap = GameObject.FindGameObjectWithTag("TerrainTilemap").GetComponent<Tilemap>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        InteractWithTerrain();
    }

    /// <summary>
    /// Handle interaction with the terrain
    /// </summary>
    private void InteractWithTerrain()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            Vector3Int cellPosition = terrainTilemap.WorldToCell(mouseWorldPosition);
            TileBase tile = terrainTilemap.GetTile(cellPosition);
            if (tile != null)
            {
                Vector3 tileDistanceVector = cellPosition - transform.position;
                float tileDistance = Mathf.Sqrt(Mathf.Pow(tileDistanceVector.x, 2) + Mathf.Pow(tileDistanceVector.y, 2));
                if (tileDistance <= actionRange)
                {
                    Debug.Log("Interaction");
                }
            }
        }
    }
}
