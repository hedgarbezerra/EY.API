namespace EY.Domain.Models.Options;

public class AuthenticationOptions
{
    public const string SettingsKey = "Authentication";

    public required Auth0Options Auth0 { get; set; }
    public required KeycloakOptions Keycloak { get; set; }
}

public class Auth0Options
{
    public const string Schema = "Auth0";
}

public class KeycloakOptions
{
    public const string Schema = "Keycloak";
}