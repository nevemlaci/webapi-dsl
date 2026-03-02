namespace AspNetGenerator;

public class ActionSource
{
    public string MethodName { get; set; }
    public HttpVerb Verb { get; set; }
    public string RouteTemplate { get; set; }
    public string ReturnType { get; set; }
    public bool IsAsync { get; set; } = true;

    public List<(string Type, string Name)> Parameters { get; set; } = [];
}

public enum HttpVerb { Get, Post, Put, Delete }