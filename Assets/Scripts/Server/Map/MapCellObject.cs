using UnityEngine;

public class MapCellObject : MonoBehaviour
{
    public MapCell mapCell { get; private set; }

    public void Initialize(MapCell cell)
    {
        mapCell = cell;
    }
}