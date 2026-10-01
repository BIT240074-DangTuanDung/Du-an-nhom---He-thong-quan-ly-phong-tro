import "./globals.css";
import type { Metadata } from "next";
export const metadata: Metadata = { title: "Quản lý phòng trọ", description: "Room rental management system" };
export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) { return <html lang="vi"><body>{children}</body></html>; }
