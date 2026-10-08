using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public ulong ClientId => NetworkObject.OwnerClientId;
    //public string teamId { get; private set; }

    // Автоматично синхронізується сервером з усіма клієнтами!
    public NetworkVariable<float> Funds {get; private set;} = new NetworkVariable<float>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<float> CommandResource {get; private set;} = new NetworkVariable<float>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    [field:SerializeField]
    public ArtilleryAbility artilleryAbility {get; private set;}

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            artilleryAbility = GetComponent<ArtilleryAbility>();

            if (artilleryAbility != null)
            {
                AbilitiesManager.Instance.RegisterPlayerArtillery(OwnerClientId, artilleryAbility);
            }
        }
    }

    public void AddFunds(float amount)
    {
        if (!IsServer) return;
        Funds.Value += amount;
    }

    public void ReduceFunds(float amount)
    {
        if (!IsServer) return;
        if(Funds.Value-amount < 0)
            Funds.Value = 0;
        else
            Funds.Value -= amount;
    }

    public void AddCommandResource(float amount)
    {
        if (!IsServer) return;
        CommandResource.Value += amount;
    }

    public void ReduceCommandResource(float amount)
    {
        if (!IsServer) return;
        if(CommandResource.Value-amount < 0)
            CommandResource.Value = 0;
        else
            CommandResource.Value -= amount;
    }

    /*private IEnumerator CommandResourceGenerationRoutine()
    {
        // Кешуємо об'єкт WaitForSeconds для уникнення виділення пам'яті (GC Allocation) кожну секунду
        WaitForSeconds wait = new WaitForSeconds(2f);
        float amountPerSecond = 0.5f; 

        // Цикл працюватиме доти, доки об'єкт гравця існує в мережі
        while (IsSpawned) 
        {
            yield return wait;
            
            // Запис дозволено, оскільки корутин виконується лише на сервері
            CommandResource.Value += amountPerSecond; 
        }
    }*/
}