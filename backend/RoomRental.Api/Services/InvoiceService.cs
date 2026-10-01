using RoomRental.Api.Domain;
using RoomRental.Api.DTOs;

namespace RoomRental.Api.Services;

public interface IInvoiceService { Invoice Build(LeaseContract contract, CreateInvoiceRequest request); }

public class InvoiceService : IInvoiceService
{
    public Invoice Build(LeaseContract contract, CreateInvoiceRequest request)
    {
        if (request.ElectricityNew < request.ElectricityOld || request.WaterNew < request.WaterOld)
            throw new ArgumentException("Chỉ số mới không thể nhỏ hơn chỉ số cũ.");

        var electricity = (request.ElectricityNew - request.ElectricityOld) * request.ElectricityUnitPrice;
        var water = (request.WaterNew - request.WaterOld) * request.WaterUnitPrice;
        var total = contract.Room.MonthlyRent + electricity + water + request.InternetFee + request.TrashFee + request.ParkingFee + request.OtherFee;
        return new Invoice {
            ContractId = contract.Id, RoomId = contract.RoomId, TenantId = contract.TenantId,
            BillingMonth = new DateOnly(request.BillingMonth.Year, request.BillingMonth.Month, 1), DueDate = request.DueDate,
            ElectricityOld = request.ElectricityOld, ElectricityNew = request.ElectricityNew, ElectricityUnitPrice = request.ElectricityUnitPrice,
            WaterOld = request.WaterOld, WaterNew = request.WaterNew, WaterUnitPrice = request.WaterUnitPrice,
            RoomFee = contract.Room.MonthlyRent, ElectricityFee = electricity, WaterFee = water,
            InternetFee = request.InternetFee, TrashFee = request.TrashFee, ParkingFee = request.ParkingFee, OtherFee = request.OtherFee,
            TotalAmount = total, Notes = request.Notes
        };
    }
}
