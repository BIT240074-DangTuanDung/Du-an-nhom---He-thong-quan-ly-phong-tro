"use client";
import { FormEvent, useState } from "react";
import { Building2, KeyRound, Mail } from "lucide-react";
import { api, setSession } from "@/lib/api";
import { useRouter } from "next/navigation";

export default function LoginPage() {
  const [email, setEmail] = useState("admin@nhatro.local"), [password, setPassword] = useState("Admin@123"), [error, setError] = useState(""), [loading, setLoading] = useState(false); const router = useRouter();
  async function submit(event: FormEvent) { event.preventDefault(); setLoading(true); setError(""); try { const result = await api<{token: string; user: { role: string }}> ("/auth/login", { method: "POST", body: JSON.stringify({ email, password }) }); setSession(result as never); router.replace(result.user.role === "Admin" ? "/dashboard" : "/invoices"); } catch (e) { setError(e instanceof Error ? e.message : "Không thể đăng nhập."); } finally { setLoading(false); } }
  return <main className="grid min-h-screen place-items-center p-4"><form onSubmit={submit} className="panel w-full max-w-md p-7 shadow-sm"><div className="mb-6 flex items-center gap-3 text-2xl font-bold text-teal-800"><Building2 size={31}/>Nhà Trọ Pro</div><p className="mb-6 text-sm text-slate-500">Đăng nhập để quản lý nơi ở của bạn.</p>{error && <p className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-700">{error}</p>}<div className="mb-4"><label>Email</label><div className="relative"><Mail className="absolute left-3 top-3 text-slate-400" size={18}/><input className="pl-10" value={email} onChange={e => setEmail(e.target.value)} type="email" required /></div></div><div className="mb-6"><label>Mật khẩu</label><div className="relative"><KeyRound className="absolute left-3 top-3 text-slate-400" size={18}/><input className="pl-10" value={password} onChange={e => setPassword(e.target.value)} type="password" required /></div></div><button disabled={loading} className="btn btn-primary w-full">{loading ? "Đang đăng nhập..." : "Đăng nhập"}</button><p className="mt-5 text-xs leading-5 text-slate-500">Admin: admin@nhatro.local / Admin@123<br/>Khách thuê: tenant@nhatro.local / Tenant@123</p></form></main>;
}
