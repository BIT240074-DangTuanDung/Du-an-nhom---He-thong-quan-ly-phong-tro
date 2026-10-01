using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/dashboard"), Authorize(Roles = "Admin")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] DateOnly? month)
    {
        var billingMonth = month is null ? new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1) : new DateOnly(month.Value.Year, month.Value.Month, 1);
        var invoices = db.Invoices.Include(x => x.Room).Include(x => x.Tenant).Where(x => x.BillingMonth == billingMonth);
        return Ok(new {
            billingMonth,
            monthlyRevenue = await invoices.Where(x => x.Status == InvoiceStatus.Paid).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            outstandingAmount = await invoices.Where(x => x.Status != InvoiceStatus.Paid).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            vacantRooms = await db.Rooms.CountAsync(x => x.Status == RoomStatus.Available),
            occupiedRooms = await db.Rooms.CountAsync(x => x.Status == RoomStatus.Occupied),
            unpaidInvoices = await invoices.Where(x => x.Status != InvoiceStatus.Paid).OrderBy(x => x.DueDate).Select(x => new { x.Id, roomCode = x.Room.Code, tenantName = x.Tenant.FullName, x.TotalAmount, x.DueDate, x.Status }).ToListAsync(),
            openTickets = await db.RepairTickets.CountAsync(x => x.Status != TicketStatus.Completed)
        });
    }
}
