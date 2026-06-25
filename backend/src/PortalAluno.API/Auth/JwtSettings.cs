namespace PortalAluno.API.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = "portal-aluno";
    public string Audience { get; set; } = "portal-aluno-web";
    public int ExpiryMinutes { get; set; } = 720;
}
