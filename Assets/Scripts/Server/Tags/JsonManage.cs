using System.Collections.Generic;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public static JsonManager Instance;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadFromResources();
        InitializeTagsDictionaty();
    }

    public HashSet<TagData> tagsDictionaty = new();
    public void LoadFromResources()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("TagsConfig");
        if (jsonFile != null)
        {
            TagData data = JsonUtility.FromJson<TagData>(jsonFile.text);
            Debug.Log($"Loaded entity: {data.id}, tags count: {data.tags.Count}");
        }
    }

    private void InitializeTagsDictionaty()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("TagsConfig");
        
    }
}