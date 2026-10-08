using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MapLine
{
    public List<MapCell> cells { private set; get; } = new();

    public MapLine(int cellsCount)
    {
        for (int i = 0; i < cellsCount; i++)
        {
            cells.Add(new MapCell());
        }
    }
}