using System.Collections.Generic;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;

public class AbilitiesManager : NetworkBehaviour
{
    public static AbilitiesManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private List<AbilitySO> abilities;

    [SerializeField]
    private Dictionary<ulong, ArtilleryAbility> playersArtilleryDictionary = new();

    public void RegisterPlayerArtillery(ulong clientId, ArtilleryAbility ability)
    {
        playersArtilleryDictionary[clientId] = ability;
    }

    public void UnregisterPlayer(ulong clientId)
    {
        playersArtilleryDictionary.Remove(clientId);
    }
    public override void OnNetworkSpawn()
    {
        //AbilityUIController.Instance.InitializeSlots(abilities);
    }

    private void Start()
    {
        //AbilityUIController.Instance.InitializeSlots(abilities);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UseArtilleryServerRpc(Vector2Int position, RpcParams rpcParams = default)
    {
        ulong playerId = rpcParams.Receive.SenderClientId;
        var artillery = playersArtilleryDictionary[playerId];

        if(artillery.Cost > EconomyManager.Instance.GetCommandResource(playerId) ||
        position.x > artillery.operationalRange) return;

        artillery.Use(new ArtilleryData(){TargetPosition = position});
        EconomyManager.Instance.ReduceCommandResource(playerId, artillery.Cost);
    }
}