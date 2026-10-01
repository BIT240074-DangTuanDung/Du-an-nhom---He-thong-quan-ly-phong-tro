using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;
using RoomRental.Api.Extensions;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/tenants"), Authorize]
public class TenantsController(AppDbContext db) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAll() => Ok(await db.Tenants.Include(x => x.Contracts.Where(c => c.IsActive)).ThenInclude(c => c.Room).OrderBy(x => x.FullName).ToListAsync());

    [HttpGet("me")]
    public async Task<ActionResult> GetMe()
    {
        var tenant = await db.Tenants.Include(x => x.Contracts.Where(c => c.IsActive)).ThenInclude(c => c.Room).SingleOrDefaultAsync(x => x.UserId == User.GetUserId());
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create(CreateTenantRequest request)
    {
        if (await db.Tenants.AnyAsync(x => x.IdentityNumber == request.IdentityNumber.Trim())) return Conflict(new { message = "CCCD/CMND đã tồn tại." });
        ApplicationUser? account = null;
        if (!string.IsNullOrWhiteSpace(request.LoginPassword)) {
            if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest(new { message = "Cần email để tạo tài khoản khách thuê." });
            if (await db.Users.AnyAsync(x => x.Email == request.Email.ToLower())) return Conflict(new { message = "Email đã tồn tại." });
            account = new ApplicationUser { FullName = request.FullName.Trim(), Email = request.Email.ToLower(), Role = UserRole.Tenant, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.LoginPassword) };
        }
        var tenant = new Tenant { FullName = request.FullName.Trim(), Phone = request.Phone.Trim(), IdentityNumber = request.IdentityNumber.Trim(), Email = request.Email?.Trim(), PermanentAddress = request.PermanentAddress?.Trim(), User = account };
        db.Tenants.Add(tenant); await db.SaveChangesAsync(); return Ok(tenant);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, CreateTenantRequest request)
    {
        var tenant = await db.Tenants.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id); if (tenant is null) return NotFound();
        if (await db.Tenants.AnyAsync(x => x.IdentityNumber == request.IdentityNumber.Trim() && x.Id != id)) return Conflict(new { message = "CCCD/CMND đã tồn tại." });
        tenant.FullName = request.FullName.Trim(); tenant.Phone = request.Phone.Trim(); tenant.IdentityNumber = request.IdentityNumber.Trim(); tenant.Email = request.Email?.Trim(); tenant.PermanentAddress = request.PermanentAddress?.Trim();
        if (tenant.User is not null) { tenant.User.FullName = tenant.FullName; if (!string.IsNullOrWhiteSpace(tenant.Email)) tenant.User.Email = tenant.Email.ToLower(); if (!string.IsNullOrWhiteSpace(request.LoginPassword)) tenant.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.LoginPassword); }
        await db.SaveChangesAsync(); return Ok(tenant);
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var tenant = await db.Tenants.Include(x => x.Contracts).SingleOrDefaultAsync(x => x.Id == id); if (tenant is null) return NotFound();
        if (tenant.Contracts.Any()) return Conflict(new { message = "Không thể xóa khách thuê đã có hợp đồng." });
        db.Tenants.Remove(tenant); await db.SaveChangesAsync(); return NoContent();
    }
}
