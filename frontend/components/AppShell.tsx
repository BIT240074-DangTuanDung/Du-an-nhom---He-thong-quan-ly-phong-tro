"use client";
import { useEffect, useState } from "react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { Building2, ClipboardList, FileText, LayoutDashboard, LogOut, ReceiptText, TriangleAlert, Users } from "lucide-react";
import { logout, session } from "@/lib/api";
import { User } from "@/lib/types";

const adminLinks = [
  ["/dashboard", "Tổng quan", LayoutDashboard], ["/rooms", "Phòng trọ", Building2], ["/tenants", "Khách thuê", Users],
  ["/contracts", "Hợp đồng", FileText], ["/invoices", "Hóa đơn", ReceiptText], ["/tickets", "Sự cố", TriangleAlert]
] as const;
const tenantLinks = [["/contracts", "Hợp đồng của tôi", FileText], ["/invoices", "Hóa đơn của tôi", ReceiptText], ["/tickets", "Yêu cầu sửa chữa", TriangleAlert]] as const;

export default function AppShell({ title, action, children }: { title: string; action?: React.ReactNode; children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null); const router = useRouter(); const pathname = usePathname();
  useEffect(() => { const current = session(); if (!current) router.replace("/login"); else setUser(current.user); }, [router]);
  if (!user) return <div className="grid min-h-screen place-items-center text-slate-500">Đang tải...</div>;
  const links = user.role === "Admin" ? adminLinks : tenantLinks;
  return <div className="min-h-screen lg:grid lg:grid-cols-[230px_1fr]">
    <aside className="border-b border-slate-200 bg-white p-4 lg:min-h-screen lg:border-b-0 lg:border-r">
      <div className="mb-7 flex items-center gap-2 text-lg font-bold text-teal-800"><Building2 size={24} /> Nhà Trọ Pro</div>
      <nav className="flex gap-1 overflow-x-auto lg:block">{links.map(([href, label, Icon]) => <Link key={href} href={href} className={`mb-1 flex shrink-0 items-center gap-3 rounded-md px-3 py-2.5 text-sm font-semibold ${pathname === href ? "bg-teal-700 text-white" : "text-slate-600 hover:bg-slate-100"}`}><Icon size={18}/>{label}</Link>)}</nav>
      <div className="mt-6 border-t border-slate-200 pt-4 text-sm"><p className="font-bold">{user.fullName}</p><p className="mb-3 text-slate-500">{user.role === "Admin" ? "Quản trị viên" : "Khách thuê"}</p><button title="Đăng xuất" onClick={() => { logout(); router.replace("/login"); }} className="btn btn-quiet w-full"><LogOut size={16}/> Đăng xuất</button></div>
    </aside>
    <main className="min-w-0 p-4 sm:p-7"><header className="mb-6 flex flex-wrap items-center justify-between gap-3"><h1 className="text-xl font-bold text-slate-800">{title}</h1>{action}</header>{children}</main>
  </div>;
}
