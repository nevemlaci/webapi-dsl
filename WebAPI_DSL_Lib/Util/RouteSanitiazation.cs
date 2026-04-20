using System.Text.RegularExpressions;

namespace WebAPI_DSL_Lib.Util;

public static class RouteSanitiazation
{
    public static string SanitizeRoute(string route)
    {
        if (route == null)
        {
            throw new ArgumentException("Route cannot be null!");
        }
        route = route.Trim();
        
        if (string.IsNullOrWhiteSpace(route))
        {
            throw new ArgumentException("Route cannot be empty.");
        }
        
        if (!Regex.IsMatch(route, "^[a-zA-Z0-9_-]+$"))
        {
            throw new ArgumentException($"Invalid route format: '{route}'.");
        }
        
        route = route.ToLowerInvariant();

        return route;
    }
}