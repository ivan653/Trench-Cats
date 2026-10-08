using System;
using System.Numerics;
using UnityEngine;

[Serializable]
public class MapZone
{
    public Vector2Int bottonLeftEdge;
    public Vector2Int topRightEdge;

    public MapZone(Vector2Int bottonLeftEdge, Vector2Int topRightEdge)
    {
        this.bottonLeftEdge = bottonLeftEdge;
        this.topRightEdge = topRightEdge;
    }

    public bool IsFitsInZone(Vector2Int position)
    {
        if(position.x < bottonLeftEdge.x || position.y < bottonLeftEdge.y) return false;
        if(position.x > topRightEdge.x || position.y > topRightEdge.y) return false;
        return true;
    }
}