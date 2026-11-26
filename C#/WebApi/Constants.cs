namespace WebApi;

public class Constants
{
    // Configuration keys, not the values themselves. The colon steps into a nested JSON object,
    // so "Jwt:Key" reads the "Key" property inside the "Jwt" section of appsettings.json.
    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtKey = "Jwt:Key";
    public const string JwtAudience = "Jwt:Audience";

    public const string HubPath = "/hub";
}
