using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAction : MonoBehaviour {
    private Tilemap terrainTilemap;

    // Use this for initialization
    void Start ()
    {
        terrainTilemap = GameObject.FindGameObjectWithTag("TerrainTilemap").GetComponent<Tilemap>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            Vector3Int cellPosition = terrainTilemap.WorldToCell(mouseWorldPosition);
            Debug.Log(terrainTilemap.GetTile(cellPosition));
        }
    }
}
