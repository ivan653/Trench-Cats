using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    public static MapManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private int mapWidth = 10;
    [SerializeField]
    private int mapHeight = 6;
    public List<MapLine> map {get; private set;} = new();
    [SerializeField]
    private GameObject defeatZonePrefab;


    void Start()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            map.Add(new MapLine(mapWidth));
        }
 
        MapRenderer.instance.DrawMap(map);
        MapRenderer.instance.CreateDefeatZone(new Vector2Int(-1, mapHeight/2), defeatZonePrefab);

        GameObject zone = defeatZonePrefab;
        zone.GetComponent<DefeatZone>().isLeftSide = false;
        MapRenderer.instance.CreateDefeatZone(new Vector2Int(mapWidth, mapHeight/2), zone);
        
    }

    public int GetMapWidth()
    {
        return mapWidth;
    }

    public int GetMapHeight()
    {
        return mapHeight;
    }

    public void SetBuildingToCell(Vector2Int cellPosition, Building building)
    {
        map[cellPosition.y].cells[cellPosition.x].SetBuilding(building);
    }
    public Building GetBuildingOnCell(Vector2Int cellPosition)
    {
        return map[cellPosition.y].cells[cellPosition.x].building;
    }

}
