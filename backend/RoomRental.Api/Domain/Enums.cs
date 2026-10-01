namespace RoomRental.Api.Domain;

public enum UserRole { Admin, Tenant }
public enum RoomStatus { Available, Occupied, Maintenance }
public enum InvoiceStatus { Unpaid, Paid, Overdue }
public enum TicketStatus { Pending, InProgress, Completed }
public enum TicketPriority { Low, Medium, High, Urgent }
