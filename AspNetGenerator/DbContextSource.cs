namespace AspNetGenerator;

public class DbContextSource
{
    public string ClassName { get; set; } = "AppDbContext";
    public List<DbSetSource> DbSets { get; set; } = [];
    public List<RelationshipSource> Relationships { get; set; } = [];
}