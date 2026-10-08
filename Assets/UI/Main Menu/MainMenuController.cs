using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private RelayConnectionManager relayManager;

    private PanelRenderer _panelRenderer;
    private TextField _joinCodeInput;
    private Label _statusLabel;
    private Button _hostButton;
    private Button _joinButton;

    private void Awake()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        // Підписуємося на оновлення та побудову UI дерева
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        if (_panelRenderer != null)
        {
            _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
        UnbindEvents();
    }

    // Метод викликається автоматично при завантаженні або live-reload UXML
    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {

        UnbindEvents();

        _hostButton = root.Q<Button>("HostButton");
        _joinButton = root.Q<Button>("ConnectButton");
        _joinCodeInput = root.Q<TextField>("JoinCodeInput");
        _statusLabel = root.Q<Label>("StatusLabel");

        if (_hostButton != null) _hostButton.clicked += OnHostClicked;
        if (_joinButton != null) _joinButton.clicked += OnJoinClicked;
    }
    private void UnbindEvents()
    {
        if (_hostButton != null) _hostButton.clicked -= OnHostClicked;
        if (_joinButton != null) _joinButton.clicked -= OnJoinClicked;
    }
    private async void OnHostClicked()
    {
        Debug.Log("Host");
        SetButtonsInteractable(false);
        _statusLabel.text = "Creating Relay allocation...";

        string code = await relayManager.StartHost();
        if (!string.IsNullOrEmpty(code))
        {
            _statusLabel.text = $"Host running. Code: {code}";
        }
        else
        {
            _statusLabel.text = "Failed to host.";
            SetButtonsInteractable(true);
        }
    }

    private async void OnJoinClicked()
    {
        string code = _joinCodeInput.value?.Trim();
        if (string.IsNullOrEmpty(code))
        {
            _statusLabel.text = "Enter a valid code!";
            return;
        }

        SetButtonsInteractable(false);
        _statusLabel.text = "Connecting to Relay...";

        bool success = await relayManager.StartClient(code);
        if (success)
        {
            _statusLabel.text = "Connected as client.";
        }
        else
        {
            _statusLabel.text = "Failed to join.";
            SetButtonsInteractable(true);
        }
    }

    private void SetButtonsInteractable(bool state)
    {
        _hostButton.SetEnabled(state);
        _joinButton.SetEnabled(state);
        _joinCodeInput.SetEnabled(state);
    }
}
