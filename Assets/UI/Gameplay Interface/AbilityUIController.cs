using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityUIController : MonoBehaviour
{
    public static AbilityUIController Instance { get; private set; }

    [Header("UI Templates & Data")]
    [SerializeField] private VisualTreeAsset abilitySlotTemplate;
    [SerializeField] private PanelRenderer _panelRenderer;

    private VisualElement _root;
    private VisualElement _abilitySlotsContainer;
    private List<AbilitySO> _abilities;
    private readonly List<AbilitySlotUI> _activeSlots = new List<AbilitySlotUI>();

    public event Action<int> OnSlotClicked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (_panelRenderer != null)
            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        if (_panelRenderer != null)
            _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient) return;

        double currentTime = NetworkManager.Singleton.LocalTime.Time;

        for (int i = 0; i < _activeSlots.Count; i++)
        {
            _activeSlots[i].UpdateCooldown(currentTime);
        }
    }

    // Приймаємо список конфігів і посилання на здібності конкретного локального гравця
    public void InitializeSlots(List<AbilitySO> abilities, PlayerAbilities localPlayerAbilities)
    {
        _abilities = abilities;
        if (_root != null && _abilitySlotsContainer != null)
            RenderSlots(abilities, localPlayerAbilities);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        _root = root;
        _abilitySlotsContainer = _root.Q<VisualElement>("AbilitySlotsContainer");

        // Якщо локальний гравець уже заспавнений, перемальовуємо слоти
        if (_abilities != null && _abilitySlotsContainer != null && PlayerAbilities.LocalInstance != null)
            RenderSlots(_abilities, PlayerAbilities.LocalInstance);
    }

    private void RenderSlots(List<AbilitySO> abilities, PlayerAbilities localPlayerAbilities)
    {
        if (abilitySlotTemplate == null || _abilitySlotsContainer == null) return;

        _abilitySlotsContainer.Clear();
        _activeSlots.Clear();

        for (int i = 0; i < abilities.Count; i++)
        {
            CreateSlot(abilities[i], i, _abilitySlotsContainer, localPlayerAbilities);
        }
    }

    private void CreateSlot(AbilitySO ability, int index, VisualElement container, PlayerAbilities localPlayerAbilities)
    {
        VisualElement slotInstance = abilitySlotTemplate.Instantiate();
        slotInstance.style.flexShrink = 0;

        var icon = slotInstance.Q<VisualElement>("AbilityIcon");
        if (icon != null && ability.Icon != null)
        {
            icon.style.backgroundImage = new StyleBackground(ability.Icon);
        }

        // Отримуємо екземпляр здібності конкретно локального гравця за типом/індексом
        IAbility abilityInstance = localPlayerAbilities.GetAbility(ability.Type);

        var slotController = new AbilitySlotUI(
            slotInstance, 
            abilityInstance, 
            () => OnSlotClicked?.Invoke(index)
        );

        _activeSlots.Add(slotController);
        container.Add(slotInstance);
    }
}