using UnityEngine.UIElements;

public class TechNodeUI
{
    public readonly string techId;
    private readonly Label researchProgress;

    public TechNodeUI(string techId, VisualElement root)
    {
        this.techId = techId;
        researchProgress = root.Q<Label>("TechCost");
    }
    
    public void UpdateProgress(float currentProgress)
    {
        researchProgress.text = currentProgress.ToString();
    }
}