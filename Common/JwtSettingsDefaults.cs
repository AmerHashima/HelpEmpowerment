namespace HelpEmpowermentApi.Common;

public static class JwtSettingsDefaults
{
    public const string DefaultIssuer = "HelpEmpowermentApi";

    public static string ResolveIssuer(string? configuredIssuer)
        => string.IsNullOrWhiteSpace(configuredIssuer) ? DefaultIssuer : configuredIssuer;
}
