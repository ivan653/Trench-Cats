using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Tutorials.Editor;
using UnityEngine;

public class BuildingManager : NetworkBehaviour
{
    public static BuildingManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private GameObject buildingProjectPrefab;
    //[SerializeField]
    //private Dictionary<BuildingType, GameObject> buildingsDictionary = new();
    [SerializeField]
    private List<BuildingSO> buildingsList = new();
    [SerializeField]
    private Dictionary<string, float> buildingDurationDictionary = new();

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlaceBuildingServerRpc(ulong clientId, Vector2Int position, string buildingID, bool ignoreLimitations = false, bool instantBuilding = false, RpcParams rpcParams = default)
    {
        if(!buildingsList.Find(x => x.ID == buildingID)) return;
        
        //clientId = rpcParams.Receive.SenderClientId;
        if(!ignoreLimitations)
        {
            bool isMine = TagsManager.Instance.GetTag(buildingID, Tag.Mine).IsNotNullOrEmpty();
            Building buildingOnCell = MapManager.Instance.GetBuildingOnCell(position);
            
            if(buildingOnCell != null)
            {
                if(!isMine || !buildingOnCell.HasTag(Tag.Deposits))
                {
                    return;   
                }
            }
            else if (isMine)
            {
                return;
            }

            if (EconomyManager.Instance.GetFunds(clientId) < buildingsList.Find(x => x.ID == buildingID).cost)
            {
                Debug.Log($"Client {clientId} does not have enough funds to place a unit.");
                return;
            }

            if(TagsManager.Instance.GetTag(buildingID, Tag.CanPlaceOnBattlefield) != null)
            {
                if (TeamsAllocation.Instance.blueTeamClientId == clientId && !TeamsAllocation.Instance.blueTeamBaseZone.IsFitsInZone(position) && !TeamsAllocation.Instance.battlefieldZone.IsFitsInZone(position))
                {
                    Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
                    return;
                } else if (TeamsAllocation.Instance.redTeamClientId == clientId && !TeamsAllocation.Instance.redTeamBaseZone.IsFitsInZone(position) && !TeamsAllocation.Instance.battlefieldZone.IsFitsInZone(position))
                {
                    Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
                    return;
                }
            }
            else
            {
                if (TeamsAllocation.Instance.blueTeamClientId == clientId && !TeamsAllocation.Instance.blueTeamBaseZone.IsFitsInZone(position))
                {
                    Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
                    return;
                } else if (TeamsAllocation.Instance.redTeamClientId == clientId && !TeamsAllocation.Instance.redTeamBaseZone.IsFitsInZone(position))
                {
                    Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
                    return;
                }
            }
        }
        
        if(instantBuilding == false)
        {
            GameObject buildingProjectInstance = Instantiate(buildingProjectPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            buildingProjectInstance.GetComponent<NetworkObject>().Spawn();

            if(buildingProjectInstance.TryGetComponent(out BuildingProject project))
            {
                if(TagsManager.Instance.GetTag(buildingID, Tag.SelfAssembling) != null)
                    project.Initialize(clientId, position, buildingID, buildingDurationDictionary[buildingID], true);
                else
                    project.Initialize(clientId, position, buildingID, buildingDurationDictionary[buildingID], false);

                project.onBuildCompleted += HandleBuildingComplete;
            }

        }
        else
        {
            HandleBuildingComplete(clientId, position, buildingID);
        }

        if(!ignoreLimitations)
            EconomyManager.Instance.ReduceFunds(clientId, buildingsList.Find(x => x.ID == buildingID).cost);
    }

    private void HandleBuildingComplete(ulong clientId, Vector2Int position, string buildingID)
    {
        if (!buildingsList.Find(x => x.ID == buildingID)) return;

        GameObject building = Instantiate(buildingsList.Find(x => x.ID == buildingID).prefabToSpawn, new Vector3(position.x, position.y, 0), Quaternion.identity);
        var netObj = building.GetComponent<NetworkObject>();
        
        // Спавнимо з урахуванням прав власника
        netObj.SpawnWithOwnership(clientId);

        // Якщо це будівля з власником (наприклад, фабрика), ініціалізуємо її
        if (building.TryGetComponent<Building>(out var buildingComponent))
        {
            buildingComponent.Initialize(clientId);
            MapManager.Instance.SetBuildingToCell(position, buildingComponent);
        }

        
    }
}