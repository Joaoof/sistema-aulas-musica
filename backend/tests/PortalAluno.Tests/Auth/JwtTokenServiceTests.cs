using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using PortalAluno.API.Auth;
using Xunit;

namespace PortalAluno.Tests.Auth;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateSut() => new(Options.Create(new JwtSettings
    {
        Key = "test-key-com-pelo-menos-256-bits-para-hmac-sha256!!",
        Issuer = "portal-aluno",
        Audience = "portal-aluno-web",
        ExpiryMinutes = 60,
    }));

    [Fact]
    public void CreateToken_EmiteJwt_ComSubDoAluno_E_EmailEClaims()
    {
        var studentId = Guid.NewGuid();

        var token = CreateSut().CreateToken(studentId, "ana@portal.dev", "Student");

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(studentId.ToString(), jwt.Subject);
        Assert.Equal("portal-aluno", jwt.Issuer);
        Assert.Contains(jwt.Audiences, a => a == "portal-aluno-web");
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "ana@portal.dev");
        Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Student");
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}
