namespace RaceDayMVC.Services;

public static class SessionKeys
{
    public const string UserId = "UserId";
    public const string Role = "Role";
    public const string Name = "Name";
    /// <summary>The Part 2 API's session cookie (name=value), forwarded on every API call.</summary>
    public const string ApiCookie = "ApiCookie";
}

public static class Roles
{
    public const string Organiser = "Organiser";
    public const string Participant = "Participant";
}