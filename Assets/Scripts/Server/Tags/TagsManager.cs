using System.Collections.Generic;
using UnityEngine;

public class TagsManager : MonoBehaviour
{
    public static TagsManager Instance { get; private set; }

    [SerializeField] private string jsonResourcePath = "tags";

    // Хешсет для зберігання всіх записів TagData
    private readonly HashSet<TagData> _allUnitTags = new();

    // Швидкий доступ за ID юніта O(1)
    private readonly Dictionary<string, TagData> _tagsById = new(System.StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadTagsFromJson();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadTagsFromJson()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(jsonResourcePath);
        if (jsonAsset == null)
        {
            Debug.LogError($"[UnitTagManager] JSON file not found at path: Resources/{jsonResourcePath}");
            return;
        }

        TagDatabaseWrapper wrapper = JsonUtility.FromJson<TagDatabaseWrapper>(jsonAsset.text);
        if (wrapper == null || wrapper.entities == null)
        {
            Debug.LogError("[UnitTagManager] Failed to parse tags JSON or file is empty.");
            return;
        }

        _allUnitTags.Clear();
        _tagsById.Clear();

        foreach (TagData data in wrapper.entities)
        {
            if (string.IsNullOrEmpty(data.id)) continue;

            // Додаємо в HashSet
            _allUnitTags.Add(data);

            // Кешуємо для швидкого отримання за ID
            _tagsById[data.id] = data;
        }

        Debug.Log($"[UnitTagManager] Successfully loaded {_allUnitTags.Count} unit tag configurations into HashSet.");
    }

    // Отримати об'єкт TagData для юніта за його ID
    public TagData GetTagData(string unitId)
    {
        _tagsById.TryGetValue(unitId, out TagData data);
        return data;
    }

    public string GetTag(string id, string tag)
    {
        _tagsById.TryGetValue(id, out TagData data);
        if(data != null)
            return data.tags.Find(x => x == tag);
        else 
            return null;
    }

    // Перевірка наявності тегу у юніта за O(1)
    public bool UnitHasTag(string unitId, string tag)
    {
        if (_tagsById.TryGetValue(unitId, out TagData data))
        {
            return data.HasTag(tag);
        }
        return false;
    }

    // Доступ до всього хешсету за потреби
    public IReadOnlyCollection<TagData> GetAllTags() => _allUnitTags;
}