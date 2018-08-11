using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErosionManager : MonoBehaviour {
	
	List<TileInfo> notEmptyTiles = new List<TileInfo>();

	[SerializeField] private GameObject terrain;
	[SerializeField] private float damageMultiplier;

    private bool flagTide;    

	// Use this for initialization
	void Start () {
        flagTide = true;
        damageMultiplier = 0.1f;
		int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
		for(int x=0; x<mapData.GetLength(0); x++){
			for(int y=0; y<mapData.GetLength(1); y++){
				if(mapData[x,y] > 0){
					TileInfo tile = new TileInfo(x, y);
					notEmptyTiles.Add(tile);
				};
			};
		};
	}

	// Update is called once per frame
	void Update () {
		if ( flagTide )
        {
            int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
            foreach ( TileInfo currentTile in notEmptyTiles )
            {
                /// On ne traite pas l'erosion sur les extremités
                if (currentTile.GetCoordinates()[0] == 0 ||
                     currentTile.GetCoordinates()[0] == mapData.GetLength(0) - 1 ||
                     currentTile.GetCoordinates()[1] == 0 ||
                     currentTile.GetCoordinates()[1] == mapData.GetLength(1) - 1)
                {
                    Debug.Log("Extremité");
                    // TODO
                }
                else
                {
                    /// On teste les contacts avec l'eau
                    /// Si il y a contact l'eau => Dégat sur la tile.

                    int[,] neighbours = currentTile.GetNeighboursCoordinates();
                    int cpt = 0;
                    for (int j = 0; j < 6; j++)
                    {
                        if (mapData[neighbours[j, 0], neighbours[j, 1]] == 0)
                        {
                            cpt++;
                        }
                    }
                    Debug.Log("Current counter : " + cpt.ToString());
                    currentTile.decreaseDurability((float)cpt * damageMultiplier);
                }
            }
        }
        flagTide = false;
	}
}
