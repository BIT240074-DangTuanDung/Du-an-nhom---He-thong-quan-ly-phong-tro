using Microsoft.EntityFrameworkCore;
using RoomRental.Api.Domain;

namespace RoomRental.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;
        var admin = new ApplicationUser { FullName = "Quản trị viên", Email = "admin@nhatro.local", Role = UserRole.Admin, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123") };
        var tenantUser = new ApplicationUser { FullName = "Nguyễn Minh Anh", Email = "tenant@nhatro.local", Role = UserRole.Tenant, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tenant@123") };
        var room = new Room { Code = "P101", Name = "Phòng 101", MonthlyRent = 3000000, Status = RoomStatus.Occupied };
        var emptyRoom = new Room { Code = "P102", Name = "Phòng 102", MonthlyRent = 3200000, Status = RoomStatus.Available };
        var tenant = new Tenant { FullName = tenantUser.FullName, Phone = "0900000001", IdentityNumber = "001200000001", Email = tenantUser.Email, PermanentAddress = "TP. Hồ Chí Minh", User = tenantUser };
        var contract = new LeaseContract { Room = room, Tenant = tenant, StartDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(9)), DepositAmount = 3000000, Terms = "Thanh toán trước ngày 05 hàng tháng.", IsActive = true };
        db.AddRange(admin, tenantUser, room, emptyRoom, tenant, contract);
        await db.SaveChangesAsync();
    }
}
