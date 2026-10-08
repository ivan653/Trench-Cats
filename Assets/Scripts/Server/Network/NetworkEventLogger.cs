using Unity.Netcode;
using UnityEngine;

public class NetworkEventLogger : MonoBehaviour
{
    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Клієнт з ID {clientId} успішно підключився.");

        if (NetworkManager.Singleton.IsServer && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Це локальний сервер/хост ініціював з'єднання.");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Клієнт з ID {clientId} відключився.");

        // Якщо це сталося на клієнті і відключився він сам (або сервер упав)
        if (!NetworkManager.Singleton.IsServer && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogWarning("З'єднання з сервером втрачено.");
            // Тут можна вивести на екран попап "Зв'язок втрачено" та повернути гравця в меню
        }
    }
}