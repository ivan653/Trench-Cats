using UnityEngine;
using UnityEngine.UIElements;

public class EndScreenController : MonoBehaviour
{
    public static EndScreenController Instance;
    private PanelRenderer panelRenderer;
    private Label endLabel;
    private Button menuButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelRenderer = GetComponent<PanelRenderer>();
        // Підписуємося на оновлення та побудову UI дерева
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        endLabel = root.Q<Label>("EndText");
        menuButton = root.Q<Button>("MenuButton");

        //if (_hostButton != null) _hostButton.clicked += OnHostClicked;
        //if(menuButton != null) menuButton.clicked +=
    }

    public void SetActive(bool value)
    {
        panelRenderer.enabled = value;
    }

    public void SetActive(bool value, string text)
    {
        panelRenderer.enabled = value;
        endLabel.text = text;
    }
    //public 

    //private void On
}   