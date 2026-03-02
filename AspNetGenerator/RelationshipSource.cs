namespace AspNetGenerator;

public class RelationshipSource
{
    public string PrincipalEntity { get; set; } // A "fő" entitás (pl. User)
    public string DependentEntity { get; set; } // Az "alárendelt" entitás (pl. Post)
    public string NavigationProperty { get; set; } // pl. "Author"
    public string CollectionProperty { get; set; } // pl. "Posts"
    public string ForeignKeyName { get; set; }     // pl. "UserId"
}