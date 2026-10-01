using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Domain;

namespace RoomRental.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<LeaseContract> Contracts => Set<LeaseContract>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<RepairTicket> RepairTickets => Set<RepairTicket>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().HasIndex(x => x.Email).IsUnique();
        builder.Entity<Room>().HasIndex(x => x.Code).IsUnique();
        builder.Entity<Tenant>().HasIndex(x => x.IdentityNumber).IsUnique();
        builder.Entity<Tenant>().HasOne(x => x.User).WithOne(x => x.Tenant).HasForeignKey<Tenant>(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LeaseContract>().HasOne(x => x.Room).WithMany(x => x.Contracts).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LeaseContract>().HasOne(x => x.Tenant).WithMany(x => x.Contracts).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Invoice>().HasIndex(x => new { x.RoomId, x.BillingMonth }).IsUnique();
        builder.Entity<Invoice>().HasOne(x => x.Contract).WithMany(x => x.Invoices).HasForeignKey(x => x.ContractId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Invoice>().HasOne(x => x.Room).WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Invoice>().HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<RepairTicket>().HasOne(x => x.Room).WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<RepairTicket>().HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        foreach (var entity in builder.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal))) { property.SetPrecision(18); property.SetScale(2); }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Entity>().Where(x => x.State == EntityState.Modified)) entry.Entity.UpdatedAt = DateTime.UtcNow;
        return base.SaveChangesAsync(cancellationToken);
    }
}
