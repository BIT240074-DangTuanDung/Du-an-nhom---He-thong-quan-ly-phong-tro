using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.DTOs;
using RoomRental.Api.Services;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/auth")]
public class AuthController(AppDbContext db, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == request.Email.ToLower());
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
        return Ok(new { token = tokenService.Create(user), user = new { user.Id, user.FullName, user.Email, user.Role } });
    }
}
