using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeployableItem", menuName = "Game/Deployable Item")]
[Serializable]
public abstract class DeployableItemSO : ScriptableObject
{
    [field:SerializeField]
    public string Id {get; private set;}   
    public string itemName;
    public float cost;
    public Sprite sprite;
    public GameObject prefabToSpawn;

}
