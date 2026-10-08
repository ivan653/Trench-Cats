using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArtilleryAbility : NetworkBehaviour, IAbility<ArtilleryData>
{
    //public static ArtilleryAbility Instance { get; private set; }

    /*void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }*/

    [field:SerializeField]
    public int radius {get; private set;} = 3;
    [field:SerializeField]
    public int operationalRange {get; private set;} = 15;
    [field:SerializeField]
    public int projectilesCount {get; private set;} = 4;

    [field: SerializeField] public AbilityType Type { get; private set; } = AbilityType.Artillery;
    [field: SerializeField] public float Cost { get; private set; } = 10f;
    [field: SerializeField] public float ReloadDuration { get; private set; } = 40f;

    public NetworkVariable<double> CooldownEndTime { get; private set; } = new NetworkVariable<double>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    [SerializeField]
    private GameObject projectilePrefab;

    public bool IsReady => NetworkManager.Singleton.ServerTime.Time >= CooldownEndTime.Value;

    public override void OnNetworkSpawn()
    {
        
    }

    public void Use(ArtilleryData inputData)
    {
        if(!IsServer || !IsReady) return;

        CooldownEndTime.Value = NetworkManager.Singleton.ServerTime.Time + ReloadDuration;

        var targetPosition = inputData.TargetPosition;

        // 1. Формуємо всі можливі координати області 3х3 (від -1 до +1 відносно центру)
        List<Vector2Int> availableOffsets = new List<Vector2Int>(9);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                availableOffsets.Add(new Vector2Int(x, y));
            }
        }

        // Обмежуємо кількість снарядів максимальною кількістю доступних клітинок (9)
        int count = Mathf.Min(projectilesCount, availableOffsets.Count);
        Vector2Int[] projectilesPositions = new Vector2Int[count];

        // 2. Алгоритм Фішера-Єйтса: вибираємо випадковий елемент і міняємо його з кінцем списку
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, availableOffsets.Count);
            
            // Swap
            Vector2Int temp = availableOffsets[i];
            availableOffsets[i] = availableOffsets[randomIndex];
            availableOffsets[randomIndex] = temp;

            projectilesPositions[i] = targetPosition + availableOffsets[i];
        }

        foreach (var position in projectilesPositions)
        {
            GameObject projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y), Quaternion.identity);

            NetworkObject netObj = projectile.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn(); // Саме це тригерить OnNetworkSpawn() на хості та клієнтах
            }
        }
    }
}