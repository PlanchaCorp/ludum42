using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ErosionManager : MonoBehaviour {
	
	List<TileInfo> notEmptyTiles = new List<TileInfo>();

	[SerializeField] private GameObject terrain;
	[SerializeField] private float damageMultiplier;

    /// <summary>
    /// Tilemaps of the terrains
    /// </summary>
    private List<Tilemap> terrainTilemaps = new List<Tilemap>();

    // Use this for initialization
    void Start () {
        damageMultiplier = 0.5f;
		int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
		for(int x=0; x<mapData.GetLength(0); x++){
			for(int y=0; y<mapData.GetLength(1); y++){
				if(mapData[x,y] > 0){
					TileInfo tile = new TileInfo(x, y);
					notEmptyTiles.Add(tile);
				};
			};
		};

        GameObject[] terrainsObjects = GameObject.FindGameObjectsWithTag("TerrainTilemap");
        int i = 0;
        foreach (GameObject terrainObject in terrainsObjects)
        {
            terrainTilemaps.Add(terrainObject.GetComponent<Tilemap>());
            i++;
        }

    }

	// Update is called once per frame
	void Update () {

	}

    public void Erode()
    {
        Debug.Log("EROSION");
        int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
        foreach (TileInfo currentTile in notEmptyTiles)
        {
            // On ne traite pas l'erosion sur les extremités
            if (currentTile.GetCoordinates()[0] == 0 ||
                 currentTile.GetCoordinates()[0] == mapData.GetLength(0) - 1 ||
                 currentTile.GetCoordinates()[1] == 0 ||
                 currentTile.GetCoordinates()[1] == mapData.GetLength(1) - 1)
            {
                Debug.LogWarning("Extremité Atteinte");
                // TODO
            }
            else
            {
                // On teste les contacts avec l'eau
                // Si il y a contact l'eau => Dégat sur la tile.
                int[,] neighbours = currentTile.GetNeighboursCoordinates();
                int cpt = 0;
                for (int j = 0; j < 6; j++)
                {
                    if (mapData[neighbours[j, 0], neighbours[j, 1]] == 0)
                    {
                        cpt++;
                    }
                }
                //Debug.Log("Current counter : " + cpt.ToString());
                currentTile.DecreaseDurability((float)cpt * damageMultiplier);

                // On regarde si la durabilité de la Tile est inférieure à 0.
                // Dans ce cas la, on déplace la Tile.
                if ( currentTile.GetDurability() <= 0)
                {
                    int[] higherNeighbour = new int[2];
                    int[] lowerNeighbour = new int[2];
                    int[] dest = new int[2];
                    int[] currentTileCoordinates = currentTile.GetCoordinates();

                    for (int j = 0; j < 6; j++)
                    {
                        if (mapData[neighbours[j, 0], neighbours[j, 1]] < mapData[currentTileCoordinates[0], currentTileCoordinates[1]])
                        {
                            // Plus petit que soit
                            if (lowerNeighbour.Length == 0 ||
                                mapData[neighbours[j, 0], neighbours[j, 1]] < mapData[lowerNeighbour[0], lowerNeighbour[1]])
                            {
                                // Plus petit voisin vide ou
                                // Plus petit que le voisin petit actuel
                                lowerNeighbour[0] = neighbours[j, 0];
                                lowerNeighbour[1] = neighbours[j, 1];
                            }
                        }
                        else if (mapData[neighbours[j, 0], neighbours[j, 1]] > mapData[currentTileCoordinates[0], currentTileCoordinates[1]])
                        {
                            // Plus Grand que soit
                            if (higherNeighbour.Length == 0 ||
                                mapData[neighbours[j, 0], neighbours[j, 1]] > mapData[higherNeighbour[0], higherNeighbour[1]])
                            {
                                // Plus petit voisin vide ou
                                // Plus petit que le voisin petit actuel
                                higherNeighbour[0] = neighbours[j, 0];
                                higherNeighbour[1] = neighbours[j, 1];
                            }
                        }
                    }

                    if ( lowerNeighbour.Length != 0 )
                    {
                        dest = lowerNeighbour;
                    }
                    else if ( higherNeighbour.Length != 0 )
                    {
                        dest = higherNeighbour;
                    }
                    else
                    {
                        int n = Mathf.RoundToInt(Random.Range(0, 5));
                        dest[0] = neighbours[n, 0];
                        dest[1] = neighbours[n, 1];
                    }

                    // On déplace
                    Dig(currentTileCoordinates[0], currentTileCoordinates[1]);
                    Replenish(dest[0], dest[1]);

                    currentTile.ResetDurability();
                }
            }
        }
    }

    /// <summary>
    ///  On suppose la case a enlevé valide
    /// </summary>
    /// <param name="tileCoordinates"></param>
    private void Dig( int x, int y )
    {
        // On modifie les données de la map
        int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
        mapData[x, y]--;
        
        Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
        int i = terrainTilemaps.Count - 1;
        int replacedTile = -1;
        Vector3Int diggingPosition = new Vector3Int(
            x - terrain.GetComponent<IslandGenerator>().mapSize/2,
            y - terrain.GetComponent<IslandGenerator>().mapSize/2,
            0);
        while (i >= 0)
        {
            Tilemap terrainTilemap = terrainTilemaps[i];
            if (terrainTilemap.GetTile(diggingPosition) != null && i > 0)
            {
                terrainTilemap.SetTile(diggingPosition, null);
                replacedTile = i - 1;
            }
            if (replacedTile == i)
            {
                terrainTilemap.SetTile(diggingPosition, tiles[i]);
            }
            i--;
        }        
    }

    /// <summary>
    /// Replenish a case of sand, if possible
    /// </summary>
    /// <param name="replenishPosition">Position where to replenish sand</param>
    private void Replenish( int x, int y )
    {
        // On modifie les données de la map
        int[,] mapData = terrain.GetComponent<IslandGenerator>().map;
        mapData[x, y]++;

        Tile[] tiles = GameObject.FindGameObjectWithTag("Grid").GetComponent<IslandGenerator>().GetSandTiles();
        int i = 0;
        int replacedTile = -1;
        bool tileReplaced = false;
        Vector3Int replenishPosition = new Vector3Int(
            x - terrain.GetComponent<IslandGenerator>().mapSize / 2,
            y - terrain.GetComponent<IslandGenerator>().mapSize / 2,
            0);

        while (i < terrainTilemaps.Count)
        {
            Tilemap terrainTilemap = terrainTilemaps[i];
            if (terrainTilemap.GetTile(replenishPosition) != null && i < terrainTilemaps.Count - 2)
            {
                terrainTilemap.SetTile(replenishPosition, null);
                replacedTile = i + 1;
            }
            if (replacedTile == i && !tileReplaced)
            {
                terrainTilemap.SetTile(replenishPosition, tiles[i]);
                tileReplaced = true;
            }
            i++;
        }
    }
}
