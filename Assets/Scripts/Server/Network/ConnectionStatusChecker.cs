using Unity.Netcode;
using UnityEngine;

public class ConnectionStatusChecker : MonoBehaviour
{
    private void Update()
    {
        if (NetworkManager.Singleton == null) return;

        // Перевірка: чи запущені взагалі мережеві процеси
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // Гра в автономному режимі (одиночна кампанія або головне меню)
            return;
        }

        // Перевірка ролі за допомогою властивостей Netcode
        if (NetworkManager.Singleton.IsHost)
        {
            // Гравець є Хостом (сервер + локальний клієнт)
            // Можна отримати кількість підключених клієнтів:
            int connectedClientsCount = NetworkManager.Singleton.ConnectedClients.Count;
            // Debug.Log($"Host active. Connected players: {connectedClientsCount}");
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            // Чистий виділений сервер (Dedicated Server)
            // Debug.Log("Dedicated Server is running.");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Клієнт успішно підключився до віддаленого хоста/сервера
            // Debug.Log("Connected to server as Client.");
        }
    }

    // Приклад методу для UI або інших скриптів
    public bool IsCurrentlyConnected()
    {
        if (NetworkManager.Singleton == null) return false;
        
        return NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer;
    }
}