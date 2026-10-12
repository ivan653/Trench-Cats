using System.Collections.Generic;
using UnityEngine;

public class UnitsHotbarManager : MonoBehaviour
{
    public static UnitsHotbarManager Instance { get; private set; }

    [field: SerializeField]
    public string selectedUnitId { get; private set; }

    [SerializeField]
    private List<SlotData> elements = new();

    void Awake()
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

    void Start()
    {
        HotbarsUI.Instance.OnUnitsHotbarSlotClicked += HandleSlotSelected;
        HotbarsUI.Instance.DrawHotbar(elements, true);
    }

    void OnDestroy()
    {
        if (HotbarsUI.Instance != null)
            HotbarsUI.Instance.OnUnitsHotbarSlotClicked -= HandleSlotSelected;
    }

    private void HandleSlotSelected(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= elements.Count) return;

        string clickedId = elements[slotIndex].itemData.Id;

        if (selectedUnitId == clickedId)
        {
            PlayerInputManager.Instance.DeselectAll();
            return;
        }

        BuildingsHotbarManager.Instance.ClearSelection();
        selectedUnitId = clickedId;
        PlayerInputManager.Instance.SetInputMode(InputMode.Placing);
    }

    public void ClearSelection()
    {
        selectedUnitId = null;
    }


    public void AddUnitToHotbar(UnitSO unit)
    {
        elements.Add(new SlotData(){itemData = unit});
        HotbarsUI.Instance.DrawHotbar(elements, true);
    }
}