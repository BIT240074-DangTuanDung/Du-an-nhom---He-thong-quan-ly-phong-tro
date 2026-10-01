using System.Security.Claims;

namespace RoomRental.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException());
    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
}
