using System.Security.Claims;
using Fundo.Applications.Data.Auditing;

namespace Fundo.Applications.WebApi.Configurations;

public sealed class HttpContextCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUsername()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return "system";
        }

        return user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst(ClaimTypes.Name)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value
            ?? user.Identity?.Name
            ?? "system";
    }
}
