using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Decoration : ScriptableObject
{
   public AnimatedTile tile;
   public int MinHeigth;
}