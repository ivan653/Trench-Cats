using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayConnectionManager : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async Task<string> StartHost(int maxConnections = 4)
    {
        try
        {
            // 1. Запит на алокацію сервера Relay
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            // 2. Отримання Join Code
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 3. Налаштування UnityTransport даними алокації
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // 4. Запуск хоста
            NetworkManager.Singleton.StartHost();

            Debug.Log($"Host started. Join Code: {joinCode}");
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay error: {e.Message}");
            return null;
        }
    }

    public async Task<bool> StartClient(string joinCode)
    {
        try
        {
            // 1. Приєднання через Join Code
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // 2. Конфігурація UnityTransport для клієнта
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData // Додатковий параметр для клієнта
            );

            // 3. Запуск клієнта
            return NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Join error: {e.Message}");
            return false;
        }
    }
}