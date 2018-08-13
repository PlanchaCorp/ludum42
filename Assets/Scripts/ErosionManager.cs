using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ErosionManager : MonoBehaviour {

    /// <summary>
    /// DAMAGE_MULTIPLIER is used to increase the effect of the erosion
    /// Set to 0.0f to stop the erosion
    /// Set to 10.0f to make the erosion insane
    /// </summary>
    [SerializeField] private float damageMultiplier;
    /// <summary>
    /// List of all the tiles that aren't water
    /// </summary>
	List<TileInfo> dryTiles = new List<TileInfo>();


    void Start () { 
        
        TileInfo[,] terrainTilesInfo = gameObject.GetComponent<MapManager>().GetTerrainInfo();

        // Create the notEmptyTiles list
        for(int i=0; i<terrainTilesInfo.GetLength(0); i++ ){
            for(int j=0; j<terrainTilesInfo.GetLength(1); j++ ){
                if(!terrainTilesInfo[i,j].GetIsFlooded()){
                    dryTiles.Add(terrainTilesInfo[i,j]);
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Move Look through all the sand tiles and erode the ones that are near the sea level
    /// </summary>
    public void Erode()
    {
        MapManager mapManager = gameObject.GetComponent<MapManager>();
        int[,] mapData = mapManager.GetMapDataCoordinates();

        foreach (TileInfo currentTile in dryTiles)
        {
            // On ne traite pas l'erosion sur les extremités
            if (currentTile.GetCoordinates()[0] == 0 ||
                 currentTile.GetCoordinates()[0] == mapData.GetLength(0) - 1 ||
                 currentTile.GetCoordinates()[1] == 0 ||
                 currentTile.GetCoordinates()[1] == mapData.GetLength(1) - 1)
            {
               // Debug.LogWarning("Extremité Atteinte");
               return;
            }
            else
            {
                // On teste les contacts avec l'eau
                // Si il y a contact l'eau => Dégat sur la tile.
                int cpt = 0;
                TileInfo[,] terrainInfo = mapManager.GetTerrainInfo();
                foreach ( Vector3Int n in currentTile.GetNeighboursCoordinates())
                {
                    TileInfo t = terrainInfo[n.x, n.y];
                    if ( t.GetIsFlooded())
                    {
                        cpt++;
                    }
                }
                currentTile.DecreaseDurability((float)cpt * damageMultiplier);

                // On regarde si la durabilité de la Tile est inférieure à 0.
                // Dans ce cas la, on déplace la Tile. ou enleve le chateau
                if (currentTile.ShallGetEroded())
                {
                    // On trouve le bon voisin
                    Vector3Int dest = FindNeighbour(currentTile, mapData, currentTile.GetNeighboursCoordinates(), currentTile.GetCoordinates());

                    // On déplace
                    mapManager.Dig(currentTile.GetCoordinates());
                    mapManager.Replenish(dest);

                    // On modifie les dryTiles
                    if (currentTile.GetIsFlooded())
                    {
                        dryTiles.Remove(currentTile);

                    }
                    if (!terrainInfo[dest.x, dest.y].GetIsFlooded())
                    {
                        dryTiles.Add(terrainInfo[dest.x, dest.y]);
                    }
                    currentTile.ResetDurability();
                }
            }
        }
    }
    
    /// <summary>
    /// Helper function for Erode
    /// </summary>
    /// <param name="currentTile"></param>
    /// <param name="mapData"></param>
    /// <param name="neighbours"></param>
    /// <param name="currentTileCoordinates"></param>
    /// <returns></returns>
    private Vector3Int FindNeighbour(TileInfo currentTile, int[,] mapData, Vector3Int[] neighbours, Vector3Int currentTileCoordinates)
    {
        Vector3Int higherNeighbour = new Vector3Int();
        Vector3Int lowerNeighbour = new Vector3Int();
        Vector3Int dest = new Vector3Int();

        dest.Equals(Vector3Int.zero);

        for (int j = 0; j < 6; j++)
        {
            if (mapData[neighbours[j].x, neighbours[j].y] < mapData[currentTileCoordinates[0], currentTileCoordinates[1]])
            {
                // Plus petit que soit
                if (lowerNeighbour != Vector3Int.zero ||
                    mapData[neighbours[j].x, neighbours[j].y] < mapData[lowerNeighbour[0], lowerNeighbour[1]])
                {
                    // Plus petit voisin vide ou
                    // Plus petit que le voisin petit actuel
                    lowerNeighbour[0] = neighbours[j].x;
                    lowerNeighbour[1] = neighbours[j].y;
                }
            }
            else if (mapData[neighbours[j].x, neighbours[j].y] > mapData[currentTileCoordinates[0], currentTileCoordinates[1]])
            {
                // Plus Grand que soit
                if (higherNeighbour != Vector3Int.zero ||
                    mapData[neighbours[j].x, neighbours[j].y] > mapData[higherNeighbour[0], higherNeighbour[1]])
                {
                    // Plus petit voisin vide ou
                    // Plus petit que le voisin petit actuel
                    higherNeighbour[0] = neighbours[j].x;
                    higherNeighbour[1] = neighbours[j].y;
                }
            }
        }

        if (lowerNeighbour != Vector3Int.zero)
        {
            dest = lowerNeighbour;
        }
        else if (higherNeighbour != Vector3Int.zero)
        {
            dest = higherNeighbour;
        }
        else
        {
            int n = Mathf.RoundToInt(Random.Range(0, 5));
            dest = neighbours[n];
        }

        return dest;
    }
}
