public interface ITaggable
{
    string Id { get;}
    bool HasTag(string tag);
    //bool HasTag(TagSO tag);
}