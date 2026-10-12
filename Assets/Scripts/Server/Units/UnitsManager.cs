using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitsManager : NetworkBehaviour
{
    public static UnitsManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private List<UnitSO> unitsList = new();


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlaceUnitServerRpc(ulong clientId, Vector2Int position, string unitId)
    {
        if(!unitsList.Find(x => x.Id == unitId)) return;
        
        if (TeamsAllocation.Instance.blueTeamClientId == clientId && !TeamsAllocation.Instance.blueTeamBaseZone.IsFitsInZone(position))
        {
            Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
            return;
        } else if (TeamsAllocation.Instance.redTeamClientId == clientId && !TeamsAllocation.Instance.redTeamBaseZone.IsFitsInZone(position))
        {
            Debug.Log($"Client {clientId} is trying to place a building in the restricted area.");
            return;
        }

        if (EconomyManager.Instance.GetFunds(clientId) < unitsList.Find(x => x.Id == unitId).cost)
        {
            Debug.Log($"Client {clientId} does not have enough funds to place a unit.");
            return;
        }

        GameObject unitInstance = Instantiate(unitsList.Find(x => x.Id == unitId).prefabToSpawn, new Vector3(position.x, position.y, 0), Quaternion.identity);
        unitInstance.GetComponent<NetworkObject>().Spawn();
        if(unitInstance.TryGetComponent<Unit>(out Unit unit))
        {
            unit.Initialize(position.y, TeamsAllocation.Instance.blueTeamClientId == clientId ? Team.Blue : Team.Red,
                TeamsAllocation.Instance.blueTeamClientId == clientId ? Direction.Right : Direction.Left,
                TeamsAllocation.Instance.blueTeamClientId == clientId ? Direction.Right : Direction.Left);
        }

        EconomyManager.Instance.ReduceFunds(clientId, unitsList.Find(x => x.Id == unitId).cost);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddUnitServerRpc(string unitId)
    {
        unitsList.Add(UnitsDB.Instance.GetUnitById(unitId));
    }
}