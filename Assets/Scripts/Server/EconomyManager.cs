using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EconomyManager : NetworkBehaviour
{
    public static EconomyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    //[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddFunds(ulong clientId, float amount)
    {
        if (!IsServer) return;

        var playerData = PlayerDataManager.Instance.GetPlayer(clientId);
        if (playerData != null)
        {
            playerData.AddFunds(amount);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId } // Тільки цей клієнт отримає виклик
                }
            };

            NotifyFundsUpdatedClientRpc(playerData.Funds.Value, clientRpcParams);
        }
        else
        {
            Debug.LogWarning($"Player with ClientId {clientId} not found.");
        }
        
        // Тут можна оновити NetworkVariable або розіслати оновлення через ClientRpc
    }

    public void ReduceFunds(ulong clientId, float amount)
    {
        if (!IsServer) return;

        var playerData = PlayerDataManager.Instance.GetPlayer(clientId);
        if (playerData != null)
        {
            playerData.ReduceFunds(amount);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId } // Тільки цей клієнт отримає виклик
                }
            };

            NotifyFundsUpdatedClientRpc(playerData.Funds.Value, clientRpcParams);
        }
        else
        {
            Debug.LogWarning($"Player with ClientId {clientId} not found.");
        }
    }

    public float GetFunds(ulong clientId)
    {
        if (PlayerDataManager.Instance.GetPlayer(clientId) != null)
        {
            return PlayerDataManager.Instance.GetPlayer(clientId).Funds.Value;
        }
        Debug.LogWarning($"Player with ClientId {clientId} not found.");
        return -1f;
    }

    public void AddCommandResource(ulong clientId, float amount)
    {
        if (!IsServer) return;

        var playerData = PlayerDataManager.Instance.GetPlayer(clientId);
        if (playerData != null)
        {
            playerData.AddCommandResource(amount);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId } // Тільки цей клієнт отримає виклик
                }
            };

            NotifyCommandResourceUpdatedClientRpc(playerData.CommandResource.Value, clientRpcParams);
        }
        else
        {
            Debug.LogWarning($"Player with ClientId {clientId} not found.");
        }
        
        // Тут можна оновити NetworkVariable або розіслати оновлення через ClientRpc
    }

    public void ReduceCommandResource(ulong clientId, float amount)
    {
        if (!IsServer) return;

        var playerData = PlayerDataManager.Instance.GetPlayer(clientId);
        if (playerData != null)
        {
            playerData.ReduceCommandResource(amount);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId } // Тільки цей клієнт отримає виклик
                }
            };

            NotifyCommandResourceUpdatedClientRpc(playerData.Funds.Value, clientRpcParams);
        }
        else
        {
            Debug.LogWarning($"Player with ClientId {clientId} not found.");
        }
    }

    public float GetCommandResource(ulong clientId)
    {
        if (PlayerDataManager.Instance.GetPlayer(clientId) != null)
        {
            return PlayerDataManager.Instance.GetPlayer(clientId).CommandResource.Value;
        }
        Debug.LogWarning($"Player with ClientId {clientId} not found.");
        return -1f;
    }

    

    [ClientRpc]
    private void NotifyFundsUpdatedClientRpc(float newValue, ClientRpcParams clientRpcParams = default)
    {
        // Оскільки Unity сама доставляє цей пакет тільки потрібному клієнту,
        // нам більше не потрібно порівнювати LocalClientId вручну!
        UIEvents.TriggerFundsUpdated(newValue);
    }

    [ClientRpc]
    private void NotifyCommandResourceUpdatedClientRpc(float newValue, ClientRpcParams clientRpcParams = default)
    {
        UIEvents.TriggerCommandResourceUpdated(newValue);
    }
    
}