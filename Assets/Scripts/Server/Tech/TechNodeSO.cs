using System.Collections.Generic;
using UnityEngine;

public enum TechStatus { Locked, Available, Researched }

[CreateAssetMenu(fileName = "TechNode", menuName = "Research/Tech Node")]
public class TechNodeSO : ScriptableObject
{
    public string id;
    public string techName;
    [TextArea] public string description;
    public Sprite icon;
    public int cost;
    public List<TechNodeSO> prerequisites;
    public Vector2 gridPosition; // Координати на полотні (X, Y)
}