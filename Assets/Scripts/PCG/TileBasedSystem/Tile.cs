using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2Int position; // Tile position in the grid
    public string tileType;     // Type of tile (e.g., "grass", "water", "road")
    
    public Tile(Vector2Int pos, string type)
    {
        position = pos;
        tileType = type;
    }
}