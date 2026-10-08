using System;
using System.Collections.Generic;

[Serializable]
public class TagData : IEquatable<TagData>
{
    public string id;
    public List<string> tags;

    // Внутрішній кешований хешсет тегів конкретного юніта
    private HashSet<string> _tagSet;

    public bool HasTag(string tag)
    {
        if (_tagSet == null)
        {
            _tagSet = new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
        }
        return _tagSet.Contains(tag);
    }

    // Перевизначення для коректного збереження об'єктів у HashSet<TagData>
    public bool Equals(TagData other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(id, other.id, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => Equals(obj as TagData);

    public override int GetHashCode()
    {
        return id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(id) : 0;
    }
}

[Serializable]
public class TagDatabaseWrapper
{
    public List<TagData> entities;
}