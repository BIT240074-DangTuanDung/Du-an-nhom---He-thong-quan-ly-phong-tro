using System.ComponentModel.DataAnnotations;

namespace RoomRental.Api.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ApplicationUser : Entity
{
    [MaxLength(150)] public string FullName { get; set; } = string.Empty;
    [MaxLength(180)] public string Email { get; set; } = string.Empty;
    [MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Tenant? Tenant { get; set; }
}

public class Room : Entity
{
    [MaxLength(30)] public string Code { get; set; } = string.Empty;
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    [MaxLength(500)] public string? Notes { get; set; }
    public ICollection<LeaseContract> Contracts { get; set; } = new List<LeaseContract>();
}

public class Tenant : Entity
{
    [MaxLength(150)] public string FullName { get; set; } = string.Empty;
    [MaxLength(30)] public string Phone { get; set; } = string.Empty;
    [MaxLength(50)] public string IdentityNumber { get; set; } = string.Empty;
    [MaxLength(180)] public string? Email { get; set; }
    [MaxLength(300)] public string? PermanentAddress { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<LeaseContract> Contracts { get; set; } = new List<LeaseContract>();
}

public class LeaseContract : Entity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal DepositAmount { get; set; }
    [MaxLength(4000)] public string Terms { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Invoice : Entity
{
    public Guid ContractId { get; set; }
    public LeaseContract Contract { get; set; } = null!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public DateOnly BillingMonth { get; set; }
    public DateOnly DueDate { get; set; }
    public int ElectricityOld { get; set; }
    public int ElectricityNew { get; set; }
    public decimal ElectricityUnitPrice { get; set; }
    public int WaterOld { get; set; }
    public int WaterNew { get; set; }
    public decimal WaterUnitPrice { get; set; }
    public decimal RoomFee { get; set; }
    public decimal ElectricityFee { get; set; }
    public decimal WaterFee { get; set; }
    public decimal InternetFee { get; set; }
    public decimal TrashFee { get; set; }
    public decimal ParkingFee { get; set; }
    public decimal OtherFee { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime? PaidAt { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}

public class RepairTicket : Entity
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    [MaxLength(150)] public string Title { get; set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; set; } = string.Empty;
    [MaxLength(500)] public string? ImageUrl { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Pending;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    [MaxLength(1000)] public string? AdminNote { get; set; }
}
