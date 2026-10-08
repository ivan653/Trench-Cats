using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneController : MonoBehaviour
{
    private const string GAME_SCENE_NAME = "SampleScene";

    private void Start()
    {
        // Переконуємося, що менеджер не знищується при зміні сцен
        DontDestroyOnLoad(gameObject);

        // Підписуємося на події підключення клієнтів
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Логіка виконується ТІЛЬКИ на хості/сервері
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log($"Клієнт підключився! ID: {clientId}");

        // clientId 0 — це завжди сам хост. 
        // Якщо підключився будь-хто з клієнтів (clientId != 0):
        if (clientId != NetworkManager.ServerClientId)
        {
            LoadGameScene();
        }
    }

    // Метод для виклику (автоматично або через UI-кнопку хоста "Start Game")
    public void LoadGameScene()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Лише сервер/хост може змінювати мережеву сцену!");
            return;
        }

        // КРИТИЧНО: використовуємо NetworkSceneManager, а не SceneManager.LoadScene()
        NetworkManager.Singleton.SceneManager.LoadScene(
            GAME_SCENE_NAME, 
            LoadSceneMode.Single
        );
    }
}