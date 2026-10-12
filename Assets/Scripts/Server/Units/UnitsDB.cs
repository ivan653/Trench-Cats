using System.Collections.Generic;
using UnityEngine;

public class UnitsDB : MonoBehaviour
{
    public static UnitsDB Instance { get; private set; }

    [SerializeField] private List<UnitSO> units;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    public UnitSO GetUnitById(string id)
    {
        return units.Find(unit => unit.Id == id);
    }
}