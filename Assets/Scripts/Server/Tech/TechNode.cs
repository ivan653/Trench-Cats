using System.Collections.Generic;
using UnityEngine;

public class TechNode
{
    public string id;
    public string techName;
    public ResearchActionSO researchAction;
    public string description;
    public Sprite icon;
    public int cost;
    public TechStatus status = TechStatus.Locked;
    public List<TechNode> prerequisites = new();
    public Vector2Int gridPosition;
    public TechNode(TechNodeSO techNodeSO)
    {
        this.id = techNodeSO.id;
        this.techName = techNodeSO.techName;
        this.researchAction = techNodeSO.researchAction;
        this.description = techNodeSO.description;
        this.icon = techNodeSO.icon;
        this.cost = techNodeSO.cost;
        this.status = techNodeSO.status;
        for (int i = 0; i < techNodeSO.prerequisites.Count; i++)
        {
            if (techNodeSO.prerequisites[i] != null)
            {
                this.prerequisites.Add(new TechNode(techNodeSO.prerequisites[i]));
            }
        }
        this.gridPosition = techNodeSO.gridPosition;
    }
}