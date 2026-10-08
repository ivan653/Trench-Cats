using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    [Header("Екрани (Sub-UXML)")]
    [SerializeField] private VisualTreeAsset gameplayInterfaceUXML;
    [SerializeField] private VisualTreeAsset researchTreeUXML;
    [SerializeField] private VisualTreeAsset settingsUXML;
    [SerializeField] private VisualTreeAsset endScreenUXML;

    private VisualElement _contentSlot;

    private void OnEnable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
            //panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        InitUI(root);
    }

    private void InitUI(VisualElement root)
    {
        if (root == null) return;

        // Контейнер, куди будемо вставляти екрани
        _contentSlot = root.Q<VisualElement>("ContentSlot");

        // Відкриваємо головне меню за замовчуванням
        ShowScreen(gameplayInterfaceUXML);
    }

    public void ShowScreen(VisualTreeAsset screenAsset)
    {
        if (_contentSlot == null || screenAsset == null) return;

        // Очищаємо лише внутрішній контейнер, не чіпаючи інший інтерфейс
        _contentSlot.Clear();

        // Створюємо екран із шаблону
        VisualElement screenInstance = screenAsset.Instantiate();
        screenInstance.style.flexGrow = 1; // Розтягуємо на весь контейнер

        _contentSlot.Add(screenInstance);
    }
}