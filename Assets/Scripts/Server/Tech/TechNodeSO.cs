using System.Collections.Generic;
using UnityEngine;

public enum TechStatus { Locked, Available, Researched }

[CreateAssetMenu(fileName = "TechNode", menuName = "Research/Tech Node")]
public class TechNodeSO : ScriptableObject
{
    public string id;
    public string techName;
    public ResearchActionSO researchAction;
    [TextArea] public string description;
    public Sprite icon;
    public int cost;
    public TechStatus status = TechStatus.Locked;
    public List<TechNodeSO> prerequisites;
    public Vector2Int gridPosition; // Координати на полотні (X, Y)
}