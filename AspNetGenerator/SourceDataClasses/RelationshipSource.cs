namespace AspNetGenerator.SourceDataClasses;

public class RelationshipSource
{
    public string PrincipalEntity { get; set; }
    public string DependentEntity { get; set; } 
    public string NavigationProperty { get; set; } 
    public string CollectionProperty { get; set; } 
    public string ForeignKeyName { get; set; }   
    
    public bool IsSelfReferencingManyToMany { get; set; } = false;
    public string JoinTableName { get; set; }  
    public string LeftForeignKey { get; set; }    
    public string RightForeignKey { get; set; }   
    
    public bool IsOneToOne { get; set; } = false;
}