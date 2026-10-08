using Unity.Netcode;
using UnityEngine;

public class PlacementManager : NetworkBehaviour
{
    public static PlacementManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject buildingPrefab;

    void Start()
    {
        
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlaceBuildingServerRpc(ulong clientId, int x, int y)
    {

        // Instantiate the building prefab at the specified position
        GameObject buildingInstance = Instantiate(buildingPrefab, new Vector3(x, y, 0), Quaternion.identity);
        buildingInstance.GetComponent<Factory>().Initialize(clientId);
        buildingInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

}