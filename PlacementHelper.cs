using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementHelper
{
    public static List<Direction> FindRoadNeigh(Vector3Int pos, ICollection<Vector3Int> collection)
    {
        List<Direction> roadNeighDir = new List<Direction>();
        if (collection.Contains(pos + Vector3Int.right)) // right meaning positive x-axis
        {
            roadNeighDir.Add(Direction.right);
        }
        if (collection.Contains(pos - Vector3Int.right)) // left meaning negative x-axis 
        {
            roadNeighDir.Add(Direction.left);
        }
        if (collection.Contains(pos + new Vector3Int(0, 0, 1))) //  positive z-axis
        {
            roadNeighDir.Add(Direction.up);
        }
        if (collection.Contains(pos - new Vector3Int(0, 0, 1))) // negative z-axis
        {
            roadNeighDir.Add(Direction.down);
        }

        return roadNeighDir;
    }

    public static Vector3Int GetOffsetFromDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.up:
                return new Vector3Int(0, 0, 1);
            case Direction.down:
                return new Vector3Int(0, 0, -1);
            case Direction.left:
                return  Vector3Int.left;
            case Direction.right:
                return Vector3Int.right;
            default:
                break;
        }
        throw new System.Exception("Oh no ;-; No direction such as: " + dir);
    }

    // Reverse the facing direction of the houses 
    public static Direction GetReverseDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.up:
                return Direction.down;
            case Direction.down:
                return Direction.up;
            case Direction.left:
                return Direction.right;
            case Direction.right:
                return Direction.left;
            default:
                break;
        }
        throw new System.Exception("Oh no ;-; No direction such as: " + dir + " for housing");
    }
}
