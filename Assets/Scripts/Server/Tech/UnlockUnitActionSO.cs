using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockUnitAction", menuName = "Game/Researches")]
public class UnlockUnitActionSO : ResearchActionSO
{
    [SerializeField] private UnitSO unlockedUnit;
    public override void Invoke()
    {
        UnitsManager.Instance.AddUnitServerRpc(unlockedUnit.Id);
        UnitsHotbarManager.Instance.AddUnitToHotbar(unlockedUnit);
    }
}