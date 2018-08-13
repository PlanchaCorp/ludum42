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
    private float damageMultiplier = 1f;

    private MapManager mapManager;
    private int[,] mapData;
    private TileInfo[,] terrain;


    void Start () {
        mapManager = gameObject.GetComponent<MapManager>();
        mapData = mapManager.GetMapDataCoordinates();
        terrain = mapManager.GetTerrainInfo();
    }

	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// Move Look through all the sand tiles and erode the ones that are near the sea level
    /// </summary>
    public void Erode()
    {
        for (int i = 0; i < terrain.GetLength(0); i++)
        {
            for (int j = 0; j < terrain.GetLength(1); j++)
            {
                TileInfo currentTile = terrain[i, j];

                // On ne traite pas l'erosion sur les extremités et l'eau
                if (currentTile.GetIsFlooded() ||
                    currentTile.GetIsSea())
                {
                    return;
                }
                else
                {
                    // On teste les contacts avec l'eau
                    // Si il y a contact l'eau => Dégat sur la tile.
                    int cpt = 0;
                    foreach (Vector3Int n in currentTile.GetNeighboursCoordinates())
                    {
                        TileInfo t = terrain[n.x, n.y];
                        if (t.GetIsFlooded())
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
                        Vector3Int dest = FindNeighbour(currentTile, mapData, currentTile.GetNeighboursCoordinates());

                        // On déplace
                        mapManager.Dig(currentTile.GetCoordinates());
                        mapManager.Replenish(dest);
                        currentTile.ResetDurability();
                    }
                }

            }
        }
    }

    /*
        foreach(TileInfo currentTile in dryTiles)
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
                        toBeRemoved.Add(currentTile);
                    }
                    if (!terrainInfo[dest.x, dest.y].GetIsFlooded())
                    {
                        toBeAdded.Add(terrainInfo[dest.x, dest.y]);
                    }
                    currentTile.ResetDurability();
                }
            }
        }

        foreach(TileInfo t in toBeAdded)
        {
            dryTiles.Add(t);
        }

        foreach (TileInfo t in toBeRemoved)
        {
            dryTiles.Remove(t);
        }
    */
    
    /// <summary>
    /// Helper function for Erode
    /// </summary>
    /// <param name="currentTile"></param>
    /// <param name="mapData"></param>
    /// <param name="neighbours"></param>
    /// <param name="currentTileCoordinates"></param>
    /// <returns></returns>
    private Vector3Int FindNeighbour(TileInfo currentTile, int[,] mapData, Vector3Int[] neighbours)
    {
        Vector3Int higherNeighbour = new Vector3Int();
        Vector3Int lowerNeighbour = new Vector3Int();
        Vector3Int dest = new Vector3Int();
        
        for (int j = 0; j < 6; j++)
        {
            TileInfo currentNeighbour = terrain[neighbours[j].x, neighbours[j].y];
            if (currentTile.GetHeight() < currentNeighbour.GetHeight())
            {
                // Plus petit que soit
                if (lowerNeighbour != Vector3Int.zero ||
                    currentTile.GetHeight() < terrain[lowerNeighbour[0], lowerNeighbour[1]].GetHeight())
                {
                    // Plus petit voisin vide ou
                    // Plus petit que le voisin petit actuel
                    lowerNeighbour[0] = neighbours[j].x;
                    lowerNeighbour[1] = neighbours[j].y;
                }
            }
            else if (currentTile.GetHeight() > currentNeighbour.GetHeight())
            {
                // Plus Grand que soit
                if (higherNeighbour != Vector3Int.zero ||
                    currentTile.GetHeight() > terrain[higherNeighbour[0], higherNeighbour[1]].GetHeight())
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
