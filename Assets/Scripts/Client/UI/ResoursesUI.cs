using Unity.Netcode;
using UnityEngine;

public class ResoursesUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI fundsText;
    private Player localPlayer;

    void Update()
    {
        // Шукаємо локального гравця, якщо він ще не знайдений
        if (localPlayer == null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            var clientObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            if (clientObj != null)
            {
                localPlayer = clientObj.GetComponent<Player>();
                if (localPlayer != null)
                {
                    // Підписуємося на зміни NetworkVariable безпосередньо у гравця
                    localPlayer.Funds.OnValueChanged += OnFundsChanged;
                    UpdateFundsDisplay(localPlayer.Funds.Value);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (localPlayer != null)
        {
            localPlayer.Funds.OnValueChanged -= OnFundsChanged;
        }
    }

    private void OnFundsChanged(float oldValue, float newValue)
    {
        UpdateFundsDisplay(newValue);
    }

    private void UpdateFundsDisplay(float funds)
    {
        if (fundsText != null)
        {
            fundsText.text = $"Funds: {funds}";
        }
    }
}