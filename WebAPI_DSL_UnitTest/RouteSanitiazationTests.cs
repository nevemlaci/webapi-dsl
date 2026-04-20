using WebAPI_DSL_Lib.Util;

namespace WebAPI_DSL_UnitTest;

public class RouteSanitiazationTests
{
    [TestCase("users", "users")]
    [TestCase("Users", "users")]
    [TestCase("  Users  ", "users")]
    [TestCase("user_profile", "user_profile")]
    [TestCase("user-profile", "user-profile")]
    [TestCase("User_123-ABC", "user_123-abc")]
    public void SanitizeRoute_ValidRoute_ReturnsLowercaseTrimmedRoute(string input, string expected)
    {
        var result = RouteSanitiazation.SanitizeRoute(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void SanitizeRoute_EmptyOrWhitespaceRoute_ThrowsArgumentException(string? input)
    {
        Assert.Throws<ArgumentException>(() => RouteSanitiazation.SanitizeRoute(input!));
    }

    [TestCase("users/list")]
    [TestCase("users.list")]
    [TestCase("users list")]
    [TestCase("users?query")]
    [TestCase("users#1")]
    public void SanitizeRoute_InvalidCharacters_ThrowsArgumentException(string input)
    {
        var ex = Assert.Throws<ArgumentException>(() => RouteSanitiazation.SanitizeRoute(input));

        Assert.That(ex!.Message, Does.Contain($"Invalid route format: '{input.Trim()}'"));
    }
}

