using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAbilities : NetworkBehaviour
{
    public static PlayerAbilities LocalInstance { get; private set; }

    [SerializeField] private List<AbilitySO> abilitiesConfig;

    // Словник здібностей саме цього гравця (наприклад, компоненти на цьому ж GameObject)
    private readonly Dictionary<AbilityType, IAbility> _abilitiesMap = new();

    public override void OnNetworkSpawn()
    {
        // Збираємо всі IAbility, прикріплені до цього префаба гравця
        foreach (var ability in GetComponents<IAbility>())
        {
            _abilitiesMap[ability.Type] = ability;
        }

        // Ініціалізуємо UI тільки для локального клієнта (власника об'єкта)
        if (IsOwner)
        {
            LocalInstance = this;
            AbilityUIController.Instance.InitializeSlots(abilitiesConfig, this);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && LocalInstance == this)
        {
            LocalInstance = null;
        }
    }

    public IAbility GetAbility(AbilityType type)
    {
        _abilitiesMap.TryGetValue(type, out var ability);
        return ability;
    }
}