using System.Collections;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class Factory : Building
{
    public override string Id => "factory";
    [SerializeField] private float productionTime = 5f;
    [SerializeField] private float productionAmount = 10f;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Економіка та спавн ресурсів працюють ТІЛЬКИ на сервері
        if (IsServer)
        {
            StartCoroutine(ProduceFundsRoutine());
        }
    }

    private IEnumerator ProduceFundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionTime);

            // Сервер нараховує ресурси власнику фабрики
            EconomyManager.Instance.AddFunds(ownerId.Value, productionAmount);
        }
    }
}