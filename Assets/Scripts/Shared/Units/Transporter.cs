using UnityEngine;
using Unity.Netcode;
using System;

public class Transporter : Unit
{
    private Vector2 position;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool isLoaded = false;
    [SerializeField] private float currentCargo = 0;
    protected override void ExecuteBehavior()
    {   
        position = transform.position;

        if(!isLoaded)
        {
            bool isMineFound = SearchForMines() != null;

            if(isMineFound)
            {
                if(Math.Abs(targetPosition.x - position.x) < 0.5)
                    GetCargo();
                    
                if(targetPosition.x > position.x && moveDirection.Value == Direction.Left)
                    SwitchMoveDirection();
                else if(targetPosition.x < position.x && moveDirection.Value == Direction.Right)
                    SwitchMoveDirection();

                Move();
            }
        }
        else
        {

            if(team.Value == Team.Blue)
            {
                if (position.x <= 0)
                {
                    TakeCargo();
                }
                moveDirection.Value = Direction.Left;
            }
            else
            {
                if (position.x >= MapManager.Instance.GetMapWidth()-1)
                {
                    TakeCargo();
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

        for(int i = 0; i < searchLine.cells.Count; i++)
        {
            if(searchLine.cells[i].building != null && searchLine.cells[i].building.HasTag(Tag.Mine) && TeamsAllocation.Instance.GetTeam(searchLine.cells[i].building.ownerId.Value) == team.Value)
            {
                targetPosition = searchLine.cells[i].position;
                return (GoldMine)searchLine.cells[i].building;
            }
        }

        return null;
    }

    private void GetCargo()
    {
        GoldMine mine = SearchForMines();
        
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