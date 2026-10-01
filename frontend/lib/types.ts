export type Role = "Admin" | "Tenant";
export type User = { id: string; fullName: string; email: string; role: Role };
export type Room = { id: string; code: string; name: string; monthlyRent: number; status: "Available" | "Occupied" | "Maintenance"; notes?: string };
export type Tenant = { id: string; fullName: string; phone: string; identityNumber: string; email?: string; permanentAddress?: string; contracts?: Contract[] };
export type Contract = { id: string; roomId: string; tenantId: string; room: Room; tenant: Tenant; startDate: string; endDate: string; depositAmount: number; terms: string; isActive: boolean };
export type Invoice = { id: string; room: Room; tenant: Tenant; billingMonth: string; dueDate: string; roomFee: number; electricityOld: number; electricityNew: number; electricityFee: number; waterOld: number; waterNew: number; waterFee: number; internetFee: number; trashFee: number; parkingFee: number; otherFee: number; totalAmount: number; status: "Unpaid" | "Paid" | "Overdue" };
export type Ticket = { id: string; room: Room; tenant: Tenant; title: string; description: string; imageUrl?: string; priority: string; status: "Pending" | "InProgress" | "Completed"; adminNote?: string; createdAt: string };
