using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private Sprite mapCellSprite;
    [SerializeField]
    private Transform mapUIDirectory;
    [SerializeField]
    private GameObject mapCellObject;

    public void DrawMap(List<MapLine> map)
    {
        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].cells.Count; j++)
            {
                map[i].cells[j].Initialize(new Vector2Int(j, i));
                GameObject cell = Instantiate(mapCellObject, new Vector3(j, i, 1), Quaternion.identity);
                cell.GetComponent<MapCellObject>().Initialize(map[i].cells[j]);
                cell.transform.SetParent(mapUIDirectory);
            }
        }
    }

    public void CreateDefeatZone(Vector2Int position, GameObject defeatZonePrefab)
    {
        Instantiate(defeatZonePrefab, new Vector3(position.x, position.y), Quaternion.identity);
    }
}
