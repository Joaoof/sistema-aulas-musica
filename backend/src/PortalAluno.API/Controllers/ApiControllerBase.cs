using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PortalAluno.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Id do aluno autenticado, extraído da claim "sub" do JWT.</summary>
    protected Guid? CurrentStudentId
    {
        get
        {
            var raw = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    /// <summary>Garante que o aluno autenticado é o dono do recurso solicitado.</summary>
    protected bool OwnsResource(Guid studentId) => CurrentStudentId == studentId;
}
