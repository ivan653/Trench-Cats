using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TeamsAllocation : MonoBehaviour
{
    public static TeamsAllocation Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private int maxPlayersPerTeam = 1;
    private int team1Count = 0;
    private int team2Count = 0;
    public ulong blueTeamClientId { get; private set; } 
    public ulong redTeamClientId { get; private set; }

    [SerializeField]
    private int battlefieldZoneSize = 5;
    [SerializeField]
    private int baseZoneSize = 5;
    public MapZone blueTeamBaseZone;
    public MapZone redTeamBaseZone;
    public MapZone battlefieldZone;
    //public int teamBuildingAreaSize = 5;
    //public int teamPlacingAreaSize = 7;

    void Start()
    {
        blueTeamBaseZone = new MapZone(new Vector2Int(0, 0), new Vector2Int(baseZoneSize, MapManager.Instance.GetMapHeight()));
        redTeamBaseZone = new MapZone(new Vector2Int(MapManager.Instance.GetMapWidth()-baseZoneSize, 0), new Vector2Int(MapManager.Instance.GetMapWidth(), MapManager.Instance.GetMapHeight()));
        battlefieldZone = new MapZone(new Vector2Int(baseZoneSize, 0), new Vector2Int(MapManager.Instance.GetMapWidth()-baseZoneSize, MapManager.Instance.GetMapHeight()));
    }

    public void RegisterClientToTeam(ulong clientId, int teamNumber)
    {
        if (teamNumber == 1)
        {
            blueTeamClientId = clientId;
            //PlayerDataManager.Instance.GetPlayer(clientId)?.SetTeam("Team1");
        }
        else if (teamNumber == 2)
        {
            redTeamClientId = clientId;
            //PlayerDataManager.Instance.GetPlayer(clientId)?.SetTeam("Team2");
        }
    }

    public ulong GetPlayerId(Team team)
    {
        if(team == Team.Blue)
            return blueTeamClientId;
        if(team == Team.Red)
            return redTeamClientId;
        return 0;
    }

    public Team GetTeam(ulong playerId)
    {
        if(playerId == GetPlayerId(Team.Blue))
            return Team.Blue;
        else
            return Team.Red;
    }

    public List<ulong> GetAllPlayersIds()
    {
        List<ulong> ids = new()
        {
            GetPlayerId(Team.Blue),
            GetPlayerId(Team.Red)
        };

        return ids;
    }

    public int AllocateTeam()
    {
        if (team1Count < maxPlayersPerTeam)
        {
            team1Count++;
            return 1; // Assign to Team 1
        }
        else if (team2Count < maxPlayersPerTeam)
        {
            team2Count++;
            return 2; // Assign to Team 2
        }
        else
        {
            Debug.LogWarning("Both teams are full. Cannot allocate a new player.");
            return -1; // Indicate that both teams are full
        }
    }

    void OnDrawGizmosSelected()
    {
        // Отримуємо висоту та ширину (з MapManager під час гри або дефолтні значення для редактора)
        int mapWidth = (MapManager.Instance != null) ? MapManager.Instance.GetMapWidth() : 20;
        int mapHeight = (MapManager.Instance != null) ? MapManager.Instance.GetMapHeight() : 10;

        // Розраховуємо межі зон (аналогічно до методу Start)
        Vector2Int blueMin = new Vector2Int(0, 0);
        Vector2Int blueMax = new Vector2Int(baseZoneSize, mapHeight);

        Vector2Int redMin = new Vector2Int(mapWidth - baseZoneSize, 0);
        Vector2Int redMax = new Vector2Int(mapWidth, mapHeight);

        Vector2Int battleMin = new Vector2Int(baseZoneSize, 0);
        Vector2Int battleMax = new Vector2Int(mapWidth - baseZoneSize, mapHeight);

        // Малюємо зону синьої бази
        DrawZoneGizmo(blueMin, blueMax, new Color(0f, 0.5f, 1f, 0.25f), Color.blue);

        // Малюємо зону червоної бази
        DrawZoneGizmo(redMin, redMax, new Color(1f, 0f, 0f, 0.25f), Color.red);

        // Малюємо зону поля бою
        DrawZoneGizmo(battleMin, battleMax, new Color(1f, 0.92f, 0.016f, 0.15f), Color.yellow);
    }

    private void DrawZoneGizmo(Vector2Int min, Vector2Int max, Color fillColor, Color wireColor)
    {
        // Ширина і висота зони
        float width = max.x - min.x;
        float height = max.y - min.y;

        if (width <= 0 || height <= 0) return;

        // Центр прямокутника у світових координатах
        Vector3 center = new Vector3(min.x + width / 2f - 0.5f, min.y + height / 2f - 0.5f, 0f);
        Vector3 size = new Vector3(width, height, 0.1f);

        // 1. Напівпрозора заливка
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        // 2. Чіткий контур
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(center, size);
    }
}