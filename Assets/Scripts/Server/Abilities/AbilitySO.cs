using System;
using UnityEngine;

[Serializable]
public abstract class AbilitySO : ScriptableObject
{
    [Header("Base Settings")]
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private string abilityName;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private float cost = 10f;
    [SerializeField] private float reloadDuration = 40f;

    public AbilityType Type => abilityType;
    public string AbilityName => abilityName;
    public Sprite Icon => abilityIcon;
    public float Cost => cost;
    public float ReloadDuration => reloadDuration;

    /// <summary>
    /// Серверна логіка виконання здібності.
    /// </summary>
    /// <param name="casterClientId">ID клієнта, який викликав здібність.</param>
    /// <param name="targetPosition">Цільові координати на карті.</param>
    public abstract void Execute(ulong casterClientId, Vector2Int targetPosition);
}