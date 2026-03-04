namespace AspNetGenerator;

public class EntitySource
{
    public string ClassName { get; set; }
    
    public List<EntityFieldSource> Fields { get; set; } = [];
}

public class EntityFieldSource
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsRelation { get; set; }
    public bool IsUnique { get; set; } = false;
    public bool IsList { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string? ForeignKeyName { get; set; } 
}