using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    [SerializeField] private InputActionReference useAction; 
    [SerializeField] private InputActionReference cancelAction; 
    [SerializeField] private InputActionReference abilityAction; 
    [SerializeField] private InputActionReference switchModeAction; 
    
    [SerializeField] private InputMode currentInputMode = InputMode.Null;
    public InputMode CurrentInputMode => currentInputMode;

    private Camera mainCamera;

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
            return;
        }

        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        if (useAction != null && useAction.action != null)
        {
            useAction.action.Enable();
            useAction.action.performed += HandleUseInputAction;
        }
        if (cancelAction != null && cancelAction.action != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += HandleCancelInputAction;
        }
        if (abilityAction != null && abilityAction.action != null)
        {
            abilityAction.action.Enable();
            abilityAction.action.performed += HandleAbilityInputAction;
        }
        if (switchModeAction != null && switchModeAction.action != null)
        {
            switchModeAction.action.Enable();
            switchModeAction.action.performed += HandleSwithModeInputAction;
        }
    }

    void OnDisable()
    {
        if (useAction != null && useAction.action != null)
        {
            useAction.action.performed -= HandleUseInputAction;
            useAction.action.Disable();
        }

        if (cancelAction != null && cancelAction.action != null)
        {
            cancelAction.action.performed -= HandleCancelInputAction;
            cancelAction.action.Disable();
        }
        if (abilityAction != null && abilityAction.action != null)
        {
            abilityAction.action.performed -= HandleAbilityInputAction;
            abilityAction.action.Disable();
        }
        if (switchModeAction != null && switchModeAction.action != null)
        {
            switchModeAction.action.performed -= HandleSwithModeInputAction;
            switchModeAction.action.Disable();
        }
    }

    private void HandleUseInputAction(InputAction.CallbackContext context)
    {
        // Блокуємо виконання дії, якщо клік здійснено по інтерфейсу
        //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            //return;

        switch (currentInputMode)
        {
            case InputMode.Placing:
                if (HotbarsUI.Instance.IsUnitsHotbarSelected && UnitsHotbarManager.Instance.selectedUnitId != null)
                {
                    UnitPlacer.Instance.PlaceUnit(UnitsHotbarManager.Instance.selectedUnitId);
                }
                else if (!HotbarsUI.Instance.IsUnitsHotbarSelected && BuildingsHotbarManager.Instance.selectedBuildingID != null && BuildingsHotbarManager.Instance.selectedBuildingID != "")
                {
                    UnitPlacer.Instance.PlaceBuilding(BuildingsHotbarManager.Instance.selectedBuildingID);
                }
                break;

            case InputMode.AbilityUsing:
                Vector2Int? pos = GetMapCellUnderMouse();
                if (pos.HasValue)
                {
                    AbilitiesManager.Instance.UseArtilleryServerRpc(pos.Value);
                    // Якщо абілка одноразова — скидаємо стан після використання:
                    DeselectAll();
                }
                break;
            case InputMode.Null:
                Vector2Int? buildingPosition = GetMapCellUnderMouse();
                if(buildingPosition.HasValue)
                {
                    Building building = MapManager.Instance.GetBuildingOnCell(buildingPosition.Value);
                    if(building != null && building is Trench trench)
                    {
                        Debug.Log(building.name);
                        trench.ExpelUnitFromTrench();
                    }
                }
                break;
        }
    }

    private void HandleCancelInputAction(InputAction.CallbackContext context)
    {
        DeselectAll();
    }

    private void HandleAbilityInputAction(InputAction.CallbackContext context)
    {
        DeselectAll();
        SetInputMode(InputMode.AbilityUsing);
    }
    private void HandleSwithModeInputAction(InputAction.CallbackContext context)
    {
        HotbarsUI.Instance.SwitchHotbar();
    }

    public void SetInputMode(InputMode mode)
    {
        currentInputMode = mode;
    }

    /// <summary>
    /// Повне скидання активного вибору: повертає режим у Null та очищує дані у хотбарах
    /// </summary>
    public void DeselectAll()
    {
        currentInputMode = InputMode.Null;
        UnitsHotbarManager.Instance.ClearSelection();
        BuildingsHotbarManager.Instance.ClearSelection();
        // За потреби: AbilitiesManager.Instance.ClearSelection();
        // За потреби: HotbarsUI.Instance.DeselectAllSlotsVisual();
    }

    public Vector2Int? GetMapCellUnderMouse()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector2 mouseScreenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.TryGetComponent(out MapCellObject mapCell))
            {
                return new Vector2Int(mapCell.mapCell.position.x, mapCell.mapCell.position.y);
            }
        }

        return null;
    }
}

public enum InputMode
{
    Null,
    Placing,
    AbilityUsing
}