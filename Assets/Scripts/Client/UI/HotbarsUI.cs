using System;
using System.Collections.Generic;
using UnityEngine;

public class HotbarsUI : MonoBehaviour
{
    public static HotbarsUI Instance { get; private set; }

    public bool IsUnitsHotbarSelected { get; private set; } = true;

    public event Action<int> OnUnitsHotbarSlotClicked;
    public event Action<int> OnBuildingsHotbarSlotClicked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Підписка на події вибору зі SlotsController
        if (SlotsController.Instance != null)
        {
            SlotsController.Instance.OnSlotClicked += HandleSlotClicked;
        }
    }

    private void OnDestroy()
    {
        if (SlotsController.Instance != null)
        {
            SlotsController.Instance.OnSlotClicked -= HandleSlotClicked;
        }
    }

    public void DrawHotbar(List<SlotData> elements, bool isUnitSlots)
    {
        List<DeployableItemSO> items = new(elements.Count);
        for (int i = 0; i < elements.Count; i++)
        {
            items.Add(elements[i].itemData);
        }

        SlotsController.Instance.InitializeSlots(items, isUnitSlots);
    }

    public void SwitchHotbar()
    {
        IsUnitsHotbarSelected = !IsUnitsHotbarSelected;
        SlotsController.Instance.SetActiveHotbar(IsUnitsHotbarSelected);
    }

    private void HandleSlotClicked(int index, bool isUnitSlot)
    {
        if (isUnitSlot)
        {
            OnUnitsHotbarSlotClicked?.Invoke(index);
        }
        else
        {
            OnBuildingsHotbarSlotClicked?.Invoke(index);
        }
    }
}