using UnityEngine;

public class MapCell
{
    public Vector2Int position { get; private set; }
    public Building building { get; private set; }

    public void Initialize(Vector2Int pos)
    {
        position = pos;
    }

    public void SetBuilding(Building building)
    {
        this.building = building;
    }
}