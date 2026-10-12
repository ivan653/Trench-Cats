using System.Collections.Generic;
using UnityEngine;

public class BuildingsHotbarManager : MonoBehaviour
{
    public static BuildingsHotbarManager Instance { get; private set; }

    [field: SerializeField]
    public string selectedBuildingID { get; private set; } = "";

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
        HotbarsUI.Instance.OnBuildingsHotbarSlotClicked += HandleSlotSelected;
        HotbarsUI.Instance.DrawHotbar(elements, false);
    }

    void OnDestroy()
    {
        if (HotbarsUI.Instance != null)
            HotbarsUI.Instance.OnBuildingsHotbarSlotClicked -= HandleSlotSelected;
    }

    private void HandleSlotSelected(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= elements.Count) return;

        string clickedID = elements[slotIndex].itemData.Id;

        if (selectedBuildingID == clickedID)
        {
            PlayerInputManager.Instance.DeselectAll();
            return;
        }

        UnitsHotbarManager.Instance.ClearSelection();
        selectedBuildingID = clickedID;
        PlayerInputManager.Instance.SetInputMode(InputMode.Placing);
    }

    public void ClearSelection()
    {
        selectedBuildingID = null;
    }
}