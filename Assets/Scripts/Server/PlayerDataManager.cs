using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerDirectory;

    // Викликається сервером для створення об'єктів гравців у мережі
    public void SpawnPlayersForClients()
    {
        //if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject playerInstance = Instantiate(playerPrefab, playerDirectory);
            NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
            
            netObj.SpawnAsPlayerObject(client.ClientId);
            Debug.Log($"Spawned player for ClientId {client.ClientId} with NetworkObjectId {netObj.NetworkObjectId}");
        }
    }

    // Шукаємо гравця серед усіх активних NetworkObject в ігровій сесії
    public Player GetPlayer(ulong clientId)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            // Перевіряємо об'єкт, яким володіє цей клієнт
            if (client.Value.PlayerObject != null)
            {
                Player player = client.Value.PlayerObject.GetComponent<Player>();
                if (player != null && player.ClientId == clientId)
                {
                    return player;
                }
            }
        }
        return null;
    }

    /*public string GetPlayerTeam(ulong clientId)
    {
        return GetPlayer(clientId)?.teamId;
    }*/
}