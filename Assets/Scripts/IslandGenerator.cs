using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandGenerator : MonoBehaviour {


    [SerializeField]
    private Tilemap groundTileMap;

    [SerializeField]
    private Tile sandTile;
	// Use this for initialization
	void Start () {
        Vector3Int vectorPosition = Vector3Int.zero;
        for (int i = 0; i< 5; i++) {
            vectorPosition.x += 1; 
            Debug.Log(vectorPosition.x);
            groundTileMap.SetTile(vectorPosition, sandTile);
        }

	}
	

}
