using Unity.Netcode;
using UnityEngine;

public class UnitPlacer : NetworkBehaviour
{
    public static UnitPlacer Instance {get; private set;}
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /*void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
        {
            return; // Only allow clients to place units
        }
    }*/

    public void PlaceUnit(string unitId)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<MapCellObject>() != null)
        {
            MapCellObject mapCellObject = hit.collider.gameObject.GetComponent<MapCellObject>();
            Debug.Log("Clicked on MapCellObject at position: " + mapCellObject.mapCell.position);
            
            UnitsManager.Instance.PlaceUnitServerRpc(NetworkManager.Singleton.LocalClientId, mapCellObject.mapCell.position, unitId);
        }
    }

    public void PlaceBuilding(string buildingId)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        //RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.GetComponent<MapCellObject>() != null)
            {
                MapCellObject mapCellObject = hit.collider.gameObject.GetComponent<MapCellObject>();
                BuildingManager.Instance.PlaceBuildingServerRpc(NetworkManager.Singleton.LocalClientId, mapCellObject.mapCell.position, buildingId);
            }
        }
    }
}   