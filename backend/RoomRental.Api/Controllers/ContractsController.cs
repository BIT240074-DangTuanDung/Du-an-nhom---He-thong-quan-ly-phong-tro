using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;
using RoomRental.Api.Extensions;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/contracts"), Authorize]
public class ContractsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var query = db.Contracts.Include(x => x.Room).Include(x => x.Tenant).AsQueryable();
        if (!User.IsAdmin()) query = query.Where(x => x.Tenant.UserId == User.GetUserId());
        return Ok(await query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.StartDate).ToListAsync());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create(CreateContractRequest request)
    {
        if (request.EndDate <= request.StartDate) return BadRequest(new { message = "Ngày kết thúc phải sau ngày bắt đầu." });
        var room = await db.Rooms.FindAsync(request.RoomId); if (room is null) return BadRequest(new { message = "Phòng không tồn tại." });
        if (room.Status != RoomStatus.Available) return Conflict(new { message = "Chỉ có thể tạo hợp đồng cho phòng trống." });
        if (!await db.Tenants.AnyAsync(x => x.Id == request.TenantId)) return BadRequest(new { message = "Khách thuê không tồn tại." });
        var contract = new LeaseContract { RoomId = request.RoomId, TenantId = request.TenantId, StartDate = request.StartDate, EndDate = request.EndDate, DepositAmount = request.DepositAmount, Terms = request.Terms.Trim(), IsActive = true };
        room.Status = RoomStatus.Occupied; db.Contracts.Add(contract); await db.SaveChangesAsync();
        return Ok(await db.Contracts.Include(x => x.Room).Include(x => x.Tenant).SingleAsync(x => x.Id == contract.Id));
    }

    [HttpPut("{id:guid}/close"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Close(Guid id)
    {
        var contract = await db.Contracts.Include(x => x.Room).SingleOrDefaultAsync(x => x.Id == id); if (contract is null) return NotFound();
        contract.IsActive = false; contract.Room.Status = RoomStatus.Available; await db.SaveChangesAsync(); return Ok(contract);
    }
}
