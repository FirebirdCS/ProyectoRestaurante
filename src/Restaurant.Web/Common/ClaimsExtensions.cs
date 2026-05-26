using System.Security.Claims;

namespace Restaurant.Web.Common;

public static class ClaimsExtensions
{
    /// <summary>Id del usuario autenticado (claim NameIdentifier).</summary>
    public static int UserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
