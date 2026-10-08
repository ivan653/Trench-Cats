using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    private bool matchStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        StartCoroutine(AddCommandPoints());
    }

    void Update()
    {
        // Тільки сервер має контролювати старт матчу та спавн гравців
        if (!IsServer) return;

        if (!matchStarted && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            var clients = NetworkManager.Singleton.ConnectedClientsList;

            // Наприклад, розподіляємо команди
            TeamsAllocation.Instance.RegisterClientToTeam(clients[0].ClientId, 1);
            TeamsAllocation.Instance.RegisterClientToTeam(clients[1].ClientId, 2);

            // Створюємо мережевих гравців для кожного клієнта
            PlayerDataManager.Instance.SpawnPlayersForClients();

            AddFundsToAllPlayers(100);
            
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.blueTeamClientId, new Vector2Int(0, 2), BuildingType.Factory, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.redTeamClientId, new Vector2Int(MapManager.Instance.GetMapWidth()-1, 2), BuildingType.Factory, true);

            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.blueTeamClientId, new Vector2Int(2, 3), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.redTeamClientId, new Vector2Int(MapManager.Instance.GetMapWidth()-3, 3), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.blueTeamClientId, new Vector2Int(7, 2), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.redTeamClientId, new Vector2Int(MapManager.Instance.GetMapWidth()-8, 2), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.blueTeamClientId, new Vector2Int(9, 1), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.redTeamClientId, new Vector2Int(MapManager.Instance.GetMapWidth()-10, 4), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.blueTeamClientId, new Vector2Int(8, 4), BuildingType.GoldDeposits, true);
            BuildingManager.Instance.PlaceBuildingServerRpc(TeamsAllocation.Instance.redTeamClientId, new Vector2Int(MapManager.Instance.GetMapWidth()-9, 1), BuildingType.GoldDeposits, true);

            matchStarted = true;
            Debug.Log("Match started and players spawned.");
        }
    }

    private void PlaceGoldDeposits(int count, Vector2Int scope)
    {
        for (int i = 0; i < count; i++)
        {
            
        }
    }

    public void HandleTeamDefeat(Team defeatedTeam)
    {
        if (!IsServer) return;

        ClientRpcParams winnerRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[]{TeamsAllocation.Instance.GetPlayerId(defeatedTeam == Team.Blue? Team.Red : Team.Blue)}}
        };

        ClientRpcParams loserRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[]{TeamsAllocation.Instance.GetPlayerId(defeatedTeam == Team.Blue? Team.Blue : Team.Red)}}
        };

        // Викликаємо RPC для обох груп з різними параметрами
        ShowEndScreenClientRpc("You Win!", winnerRpcParams);
        ShowEndScreenClientRpc("You Lose!", loserRpcParams);
    }

    [ClientRpc]
    private void ShowEndScreenClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        // Викликається локально на пристрої кожного клієнта, який отримав цей RPC
        EndScreenController.Instance.SetActive(true, message);
    }

    private IEnumerator AddCommandPoints()
    {
        var wait = new WaitForSeconds(2f);
        while (IsSpawned)
        {
            yield return wait;
            if (!matchStarted) continue;

            foreach (var id in TeamsAllocation.Instance.GetAllPlayersIds())
            {
                EconomyManager.Instance.AddCommandResource(id, 1f);
            }
        }
    }

    private void AddFundsToAllPlayers(float value)
    {
        foreach (var id in TeamsAllocation.Instance.GetAllPlayersIds())
        {
            EconomyManager.Instance.AddFunds(id, value);
        }
    }
}