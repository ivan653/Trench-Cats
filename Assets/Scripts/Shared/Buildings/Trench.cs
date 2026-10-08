using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Trench : Building
{
    public override string Id => "trench";
    [SerializeField]
    private int maxUnitsInsideCount = 3;
    //[SerializeField]
    //private int currentUnitsInsideCount = 0;
    [SerializeField]
    private List<Unit> units = new();
    [SerializeField]
    private List<string> blackList = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public bool HandleTrenchEntrance(Unit unit)
    {
        if(units.Count >= maxUnitsInsideCount || blackList.Exists(x => x == unit.id.Value)) return false;

        units.Add(unit);
        //currentUnitsInsideCount++;
        
        return true;
    }

    public void ExpelUnitFromTrench()
    {
        blackList.Add(units.First().id.Value.ToString());
        units.First().ExitFromTrench();
        units.RemoveAt(0);
    }

}