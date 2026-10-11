using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Transporter : Unit
{
    private Vector2 position;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool isLoaded = false;
    [SerializeField] private float currentCargo = 0;

    protected override void ExecuteBehavior()
    {   
        // Логіка переміщення та економіки повинна виконуватися лише на сервері
        if (!IsServer) return;

        position = transform.position;

        if (!isLoaded)
        {
            GoldMine targetMine = SearchForMines();

            if (targetMine != null)
            {
                if (Mathf.Abs(targetPosition.x - position.x) < 0.5f)
                {
                    GetCargo(targetMine);
                    return;
                }
                    
                if (targetPosition.x > position.x && moveDirection.Value == Direction.Left)
                    SwitchMoveDirection();
                else if (targetPosition.x < position.x && moveDirection.Value == Direction.Right)
                    SwitchMoveDirection();

                Move();
            }
        }
        else
        {
            if (team.Value == Team.Blue)
            {
                if (position.x <= 0)
                {
                    TakeCargo();
                    return;
                }
                moveDirection.Value = Direction.Left;
            }
            else
            {
                int mapEdge = MapManager.Instance.GetMapWidth() - 1;
                if (position.x >= mapEdge)
                {
                    TakeCargo();
                    return;
                }
                moveDirection.Value = Direction.Right;
            }
            
            Move();
        }
    }

    public override void Attack()
    {
        // Транспорт не атакує
    }

    private GoldMine SearchForMines()
    {
        var searchLine = MapManager.Instance.map[mapLine];
        GoldMine bestMine = null;
        float bestScore = float.MinValue;

        // Пошук за один прохід без виділення пам'яті під List
        for (int i = 0; i < searchLine.cells.Count; i++)
        {
            var cell = searchLine.cells[i];
            if (cell.building != null && 
                cell.building.HasTag(Tag.Mine) && 
                TeamsAllocation.Instance.GetTeam(cell.building.ownerId.Value) == team.Value)
            {
                if (cell.building is GoldMine mine)
                {
                    float dist = Mathf.Abs(position.x - cell.position.x);
                    float score = mine.currentProduction - dist;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMine = mine;
                        targetPosition = cell.position;
                    }
                }
            }
        }

        return bestMine;
    }

    private void GetCargo(GoldMine mine)
    {
        currentCargo = mine.UnloadStorage();
        isLoaded = true;
    }

    private void TakeCargo()
    {
        EconomyManager.Instance.AddFunds(TeamsAllocation.Instance.GetPlayerId(team.Value), currentCargo);
        currentCargo = 0;
        isLoaded = false;
    }
}