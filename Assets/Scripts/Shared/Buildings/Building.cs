using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Tutorials.Editor;
using UnityEngine;

public abstract class Building : NetworkBehaviour, ITaggable
{
    public abstract string Id { get; }
    public NetworkVariable<ulong> ownerId = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField]
    protected float cost = 100;
    [SerializeField]
    protected float buildingTime = 10;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void Initialize(ulong clientId)
    {
        if(IsServer)
        {
            ownerId.Value = clientId;
        }
    }

    public virtual bool HasTag(string tag)
    {
        if(!TagsManager.Instance.GetTag(Id, tag).IsNullOrEmpty())
            return true;
        else
            return false;
    }
}