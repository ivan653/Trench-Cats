using UnityEngine;

public abstract class ResearchActionSO : ScriptableObject
{
    public string Id;
    public ResearchActionType actionType = ResearchActionType.None;

    public abstract void Invoke();
}