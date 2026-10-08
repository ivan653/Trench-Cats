using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Linq;

#if UNITY_EDITOR
using Unity.Multiplayer.PlayMode;
#endif

public class MPPMAutoConnect : MonoBehaviour
{
    [SerializeField] private ushort defaultPort = 7777;
    [SerializeField] private string localAddress = "127.0.0.1";

    private void Start()
    {
        // Не блокуємо виконання у збірках (Standalone Build)
#if !UNITY_EDITOR
        return;
#endif

#if UNITY_EDITOR
        // Перевіряємо, чи запущено кілька інстансів MPPM
        if (!CurrentPlayer.Tags.Contains("client"))
        {
            // Головний редактор запускається як хост
            StartAsHost();
        }
        else
        {
            // Усі віртуальні клони підключаються локально як клієнти
            StartAsClient();
        }
#endif
    }

#if UNITY_EDITOR
    private void StartAsHost()
    {
        ConfigureLocalTransport();
        bool success = NetworkManager.Singleton.StartHost();
        Debug.Log($"[MPPM] Запуск головного редактора як HOST: {(success ? "Успішно" : "Помилка")}");
    }

    private void StartAsClient()
    {
        ConfigureLocalTransport();
        bool success = NetworkManager.Singleton.StartClient();
        Debug.Log($"[MPPM] Запуск Virtual Player ({CurrentPlayer.Tags}) як CLIENT: {(success ? "Успішно" : "Помилка")}");
    }

    private void ConfigureLocalTransport()
    {
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (transport != null)
        {
            // Скидаємо Relay і встановлюємо пряме локальне UDP-з'єднання loopback
            transport.SetConnectionData(localAddress, defaultPort);
        }
    }
#endif
}