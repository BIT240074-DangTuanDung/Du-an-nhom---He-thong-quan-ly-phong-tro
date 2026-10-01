using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Data;
using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;
using RoomRental.Api.Extensions;
using RoomRental.Api.Services;

namespace RoomRental.Api.Controllers;

[ApiController, Route("api/invoices"), Authorize]
public class InvoicesController(AppDbContext db, IInvoiceService invoiceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] DateOnly? month)
    {
        var query = db.Invoices.Include(x => x.Room).Include(x => x.Tenant).AsQueryable();
        if (!User.IsAdmin()) query = query.Where(x => x.Tenant.UserId == User.GetUserId());
        if (month is not null) query = query.Where(x => x.BillingMonth == new DateOnly(month.Value.Year, month.Value.Month, 1));
        return Ok(await query.OrderByDescending(x => x.BillingMonth).ThenBy(x => x.Room.Code).ToListAsync());
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create(CreateInvoiceRequest request)
    {
        var contract = await db.Contracts.Include(x => x.Room).SingleOrDefaultAsync(x => x.Id == request.ContractId && x.IsActive);
        if (contract is null) return BadRequest(new { message = "Hợp đồng đang hiệu lực không tồn tại." });
        var month = new DateOnly(request.BillingMonth.Year, request.BillingMonth.Month, 1);
        if (await db.Invoices.AnyAsync(x => x.RoomId == contract.RoomId && x.BillingMonth == month)) return Conflict(new { message = "Phòng này đã có hóa đơn trong tháng." });
        try { var invoice = invoiceService.Build(contract, request); db.Invoices.Add(invoice); await db.SaveChangesAsync(); return Ok(invoice); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:guid}/status"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateStatus(Guid id, UpdateInvoiceStatusRequest request)
    {
        var invoice = await db.Invoices.FindAsync(id); if (invoice is null) return NotFound();
        invoice.Status = request.Status; invoice.PaidAt = request.Status == InvoiceStatus.Paid ? DateTime.UtcNow : null;
        await db.SaveChangesAsync(); return Ok(invoice);
    }
}
