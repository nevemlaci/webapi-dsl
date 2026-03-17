namespace AspNetGenerator;

public class ActionSource
{
    public string MethodName { get; set; }
    public HttpVerb Verb { get; set; }
    public string RouteTemplate { get; set; }
    public string ReturnType { get; set; }
    public bool IsAsync { get; set; } = true;

    public List<(string Type, string Name)> Parameters { get; set; } = [];
    public List<FilterInfo> Filters { get; set; } = [];
}

public class FilterInfo
{
    public string FieldName { get; set; }
    public string Type { get; set; }
    public string ParamName { get; set; }
    public string MinParamName { get; set; }
    public string MaxParamName { get; set; }
}

public enum HttpVerb { Get, Post, Put, Delete }