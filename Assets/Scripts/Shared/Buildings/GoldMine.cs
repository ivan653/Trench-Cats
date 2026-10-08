using System.Collections;
using UnityEngine;

public class GoldMine : Building
{
    public override string Id => "goldMine";

    [SerializeField] private float productionTime = 5f;
    [SerializeField] private float productionAmount = 10f;
    [SerializeField] private float currentProduction = 0;
    [SerializeField] private float storageSize = 100f;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            StartCoroutine(ProduceGoldRoutine());
        }
    }

    private IEnumerator ProduceGoldRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionTime);

            if(currentProduction + productionAmount > storageSize)
                currentProduction = storageSize;
            else
                currentProduction += productionAmount;
        }
    }

    public float UnloadStorage()
    {
        var temp = currentProduction;
        currentProduction = 0;
        return temp;   
    }
}