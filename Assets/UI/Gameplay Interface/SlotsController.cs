using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotsController : MonoBehaviour
{
    public static SlotsController Instance;

    [Header("UI Templates & Data")]
    [SerializeField] private VisualTreeAsset unitSlotTemplate;
    [SerializeField] private PanelRenderer _panelRenderer;

    private VisualElement _unitsSlotsContainer;
    private VisualElement _buildingsSlotsContainer;
    private VisualElement _root;
    private List<DeployableItemSO> _units;
    private List<DeployableItemSO> _buildings;

    // Подія для зв'язку з HotbarsUI (передає індекс слота та тип)
    public event Action<int, bool> OnSlotClicked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (_panelRenderer == null)
            _panelRenderer = GetComponent<PanelRenderer>();

        if (_panelRenderer != null)
        {
            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnDestroy()
    {
        if (_panelRenderer != null)
        {
            _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        _root = root;
        _unitsSlotsContainer = _root.Q<VisualElement>("UnitSlotsContainer");
        _buildingsSlotsContainer = _root.Q<VisualElement>("BuildingSlotsContainer");

        if (_units != null && _unitsSlotsContainer != null)
            RenderSlots(_units, isUnitSlots: true);

        if (_buildings != null && _buildingsSlotsContainer != null)
            RenderSlots(_buildings, isUnitSlots: false);

        // Встановлюємо актуальну видимість після перезавантаження дерева
        if (HotbarsUI.Instance != null)
        {
            SetActiveHotbar(HotbarsUI.Instance.IsUnitsHotbarSelected);
        }
    }

    public void InitializeSlots(List<DeployableItemSO> items, bool isUnitSlots)
    {
        if (isUnitSlots)
        {
            _units = items;
            if (_root != null && _unitsSlotsContainer != null)
                RenderSlots(items, true);
        }
        else
        {
            _buildings = items;
            if (_root != null && _buildingsSlotsContainer != null)
                RenderSlots(items, false);
        }
    }

    public void SetActiveHotbar(bool showUnits)
    {
        if (_unitsSlotsContainer != null)
            _unitsSlotsContainer.style.display = showUnits ? DisplayStyle.Flex : DisplayStyle.None;

        if (_buildingsSlotsContainer != null)
            _buildingsSlotsContainer.style.display = showUnits ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void RenderSlots(List<DeployableItemSO> items, bool isUnitSlots)
    {
        if (unitSlotTemplate == null) return;

        VisualElement targetContainer = isUnitSlots ? _unitsSlotsContainer : _buildingsSlotsContainer;
        if (targetContainer == null) return;

        targetContainer.style.flexDirection = FlexDirection.Row;
        targetContainer.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            CreateSlot(items[i], i, targetContainer, isUnitSlots);
        }
    }

    private void CreateSlot(DeployableItemSO item, int index, VisualElement container, bool isUnitSlot)
    {
        VisualElement slotInstance = unitSlotTemplate.Instantiate();
        slotInstance.style.flexShrink = 0;

        var slotButton = slotInstance.Q<Button>("SlotButton");
        var unitIcon = slotInstance.Q<VisualElement>("UnitIcon");
        var unitNameLabel = slotInstance.Q<Label>("UnitName");
        var unitCostLabel = slotInstance.Q<Label>("UnitCost");

        if (unitNameLabel != null) unitNameLabel.text = item.name;
        if (unitCostLabel != null) unitCostLabel.text = item.cost.ToString();

        if (unitIcon != null && item.sprite != null)
        {
            // Якщо використовується Sprite:
            unitIcon.style.backgroundImage = new StyleBackground(item.sprite);
        }

        if (slotButton != null)
        {
            slotButton.clicked += () => OnSlotClicked?.Invoke(index, isUnitSlot);
        }

        container.Add(slotInstance);
    }
}