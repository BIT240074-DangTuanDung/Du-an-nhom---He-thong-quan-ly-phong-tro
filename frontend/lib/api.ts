import { User } from "./types";
export const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5075/api";
export function session(): { token: string; user: User } | null { if (typeof window === "undefined") return null; const raw = localStorage.getItem("rental_session"); return raw ? JSON.parse(raw) : null; }
export function setSession(value: { token: string; user: User }) { localStorage.setItem("rental_session", JSON.stringify(value)); }
export function logout() { localStorage.removeItem("rental_session"); }
export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = session()?.token;
  const headers = new Headers(options.headers);
  if (token) headers.set("Authorization", `Bearer ${token}`);
  if (options.body && !(options.body instanceof FormData)) headers.set("Content-Type", "application/json");
  const response = await fetch(`${API_URL}${path}`, { ...options, headers });
  if (!response.ok) { const data = await response.json().catch(() => null); throw new Error(data?.message || "Yêu cầu không thành công."); }
  return response.status === 204 ? undefined as T : response.json();
}
export const money = (value: number) => new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND", maximumFractionDigits: 0 }).format(value);
export const date = (value: string) => new Date(value).toLocaleDateString("vi-VN");
