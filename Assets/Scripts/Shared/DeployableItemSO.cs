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

    // Конкретний тип або категорія
    public ItemCategory category;
    
    //[Header("Specific Type")]
    //public UnitType unitType;       // Заповнюється, якщо це Unit
    //public BuildingType buildingType; // Заповнюється, якщо це Building
}
