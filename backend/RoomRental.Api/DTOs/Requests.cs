using System.ComponentModel.DataAnnotations;
using RoomRental.Api.Domain;

namespace RoomRental.Api.DTOs;

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record CreateRoomRequest([Required, MaxLength(30)] string Code, [Required, MaxLength(100)] string Name, [Range(0, 999999999)] decimal MonthlyRent, RoomStatus Status, string? Notes);
public record CreateTenantRequest([Required, MaxLength(150)] string FullName, [Required] string Phone, [Required] string IdentityNumber, string? Email, string? PermanentAddress, string? LoginPassword);
public record CreateContractRequest(Guid RoomId, Guid TenantId, DateOnly StartDate, DateOnly EndDate, [Range(0, 999999999)] decimal DepositAmount, string Terms);
public record CreateInvoiceRequest(Guid ContractId, DateOnly BillingMonth, DateOnly DueDate, int ElectricityOld, int ElectricityNew, decimal ElectricityUnitPrice, int WaterOld, int WaterNew, decimal WaterUnitPrice, decimal InternetFee, decimal TrashFee, decimal ParkingFee, decimal OtherFee, string? Notes);
public record UpdateInvoiceStatusRequest(InvoiceStatus Status);
public record CreateTicketRequest(Guid RoomId, [Required, MaxLength(150)] string Title, [Required, MaxLength(2000)] string Description, TicketPriority Priority);
public record UpdateTicketRequest(TicketStatus Status, string? AdminNote);
