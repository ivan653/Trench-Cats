using System;
using Unity.Netcode;
using UnityEngine;

public class BuildingProject : NetworkBehaviour
{
    private ulong ownerClientId;
    private float buildingDuration = 5f;
    private float currentBuildingTime = 0f;
    private Vector2Int position;
    //private BuildingType buildingType;
    private string buildingID;
    private bool isSelfAssembling = true;
    private Unit builder;
    private bool isStartBuilding = false;

    // Змінили сигнатуру події, щоб передавати clientId
    public event Action<ulong, Vector2Int, string> onBuildCompleted;

    public void Initialize(ulong clientId, Vector2Int position, string buildingID, float buildingDuration, bool isSelfAssembling)
    {
        this.ownerClientId = clientId;
        this.position = position;
        this.buildingID = buildingID;
        this.isSelfAssembling = isSelfAssembling;
        this.buildingDuration = buildingDuration;

        if (isSelfAssembling)
        {
            StartBuilding();
        }
    }

    private void StartBuilding()
    {
        isStartBuilding = true;
    }

    public void StartBuilding(Unit builder)
    {
        isStartBuilding = true;
        this.builder = builder;
    }

    void Update()
    {
        // Процес обчислюється тільки на сервері
        if (!IsServer) return;
        if (!isStartBuilding) return;

        // Перевіряємо умови зовнішнього будівництва (якщо не самозбірне)
        if (!isSelfAssembling && !CheckBuildingConditions()) 
        {
            // Тут за бажанням можна перервати будівництво або просто зупинити таймер
            return;
        }

        currentBuildingTime += Time.deltaTime;

        if (currentBuildingTime >= buildingDuration)
        {
            CompleteBuilding();
        }
    }

    private bool CheckBuildingConditions()
    {
        // Якщо будівельник знищений (null) або перестав будувати — умови не пройдені
        if (builder == null || !builder.gameObject.activeInHierarchy) return false;
        
        return builder.isBuilding.Value;
    }

    private void CompleteBuilding()
    {
        // 1. Сповіщаємо менеджер про завершення
        onBuildCompleted?.Invoke(ownerClientId, position, buildingID);

        // 2. Звільняємо юніта, якщо він був будівельником
        if (builder != null)
        {
            builder.isBuilding.Value = false;
        }

        // 3. Коректно видаляємо мережевий об'єкт проєкту
        if (TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}