namespace AspNetGenerator;

public class DtoSource
{
    public string ClassName { get; set; }
    public List<DtoFieldDefinition> Fields { get; set; } = [];
    
    public DtoSource(string entityName)
    {
        ClassName = $"{entityName}Dto";
    }
}

public class DtoFieldDefinition
{
    public string Type { get; set; }
    public string Name { get; set; }
}