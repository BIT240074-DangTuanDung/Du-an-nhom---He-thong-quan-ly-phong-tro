using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/rooms"), Authorize]
public class RoomsController(AppDbContext db) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAll() => Ok(await db.Rooms.OrderBy(x => x.Code).ToListAsync());

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create(CreateRoomRequest request)
    {
        if (await db.Rooms.AnyAsync(x => x.Code == request.Code.Trim())) return Conflict(new { message = "Mã phòng đã tồn tại." });
        var room = new Room { Code = request.Code.Trim(), Name = request.Name.Trim(), MonthlyRent = request.MonthlyRent, Status = request.Status, Notes = request.Notes?.Trim() };
        db.Rooms.Add(room); await db.SaveChangesAsync(); return CreatedAtAction(nameof(GetAll), new { id = room.Id }, room);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, CreateRoomRequest request)
    {
        var room = await db.Rooms.FindAsync(id); if (room is null) return NotFound();
        if (await db.Rooms.AnyAsync(x => x.Code == request.Code.Trim() && x.Id != id)) return Conflict(new { message = "Mã phòng đã tồn tại." });
        room.Code = request.Code.Trim(); room.Name = request.Name.Trim(); room.MonthlyRent = request.MonthlyRent; room.Status = request.Status; room.Notes = request.Notes?.Trim();
        await db.SaveChangesAsync(); return Ok(room);
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var room = await db.Rooms.Include(x => x.Contracts).SingleOrDefaultAsync(x => x.Id == id); if (room is null) return NotFound();
        if (room.Contracts.Any()) return Conflict(new { message = "Không thể xóa phòng đã có hợp đồng." });
        db.Rooms.Remove(room); await db.SaveChangesAsync(); return NoContent();
    }
}
