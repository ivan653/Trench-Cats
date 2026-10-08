using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    [SerializeField] private UnityTransport transport;

    public void OnClickHostButton()
    {
        // Для хоста зазвичай використовується слухач на всіх інтерфейсах ("0.0.0.0") або "127.0.0.1"
        string ip = "127.0.0.1"; 
        ConnectionManager.instance.StartHost(ip, 7777);
    }

    public void OnClickClientButton()
    {
        // Зчитуємо IP, який ввів інший гравець у поле введення
        string ip = "127.0.0.1";
        ConnectionManager.instance.StartClient(ip, 7777);
    }

    public void StartHost(string ip, ushort port)
    {
        transport.SetConnectionData(ip, port);
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient(string ip, ushort port)
    {
        transport.SetConnectionData(ip, port);
        NetworkManager.Singleton.StartClient();
    }
}