using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;
using RoomRental.Api.Extensions;
using RoomRental.Api.Services;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/tickets"), Authorize]
public class TicketsController(AppDbContext db, IFileStorageService storage) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = db.RepairTickets.Include(x => x.Room).Include(x => x.Tenant).AsQueryable();
        if (!User.IsAdmin()) query = query.Where(x => x.Tenant.UserId == User.GetUserId());
        return Ok(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] CreateTicketRequest request, IFormFile? image)
    {
        var tenant = await db.Tenants.SingleOrDefaultAsync(x => x.UserId == User.GetUserId());
        if (tenant is null) return BadRequest(new { message = "Tài khoản này chưa được gắn với khách thuê." });
        var permitted = await db.Contracts.AnyAsync(x => x.RoomId == request.RoomId && x.TenantId == tenant.Id && x.IsActive);
        if (!User.IsAdmin() && !permitted) return Forbid();
        try {
            var ticket = new RepairTicket { RoomId = request.RoomId, TenantId = tenant.Id, Title = request.Title.Trim(), Description = request.Description.Trim(), Priority = request.Priority, ImageUrl = await storage.SaveTicketImageAsync(image) };
            db.RepairTickets.Add(ticket); await db.SaveChangesAsync(); return Ok(ticket);
        } catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, UpdateTicketRequest request)
    {
        var ticket = await db.RepairTickets.FindAsync(id); if (ticket is null) return NotFound();
        ticket.Status = request.Status; ticket.AdminNote = request.AdminNote?.Trim(); await db.SaveChangesAsync(); return Ok(ticket);
    }
}
