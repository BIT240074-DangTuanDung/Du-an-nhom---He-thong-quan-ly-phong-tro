"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { session } from "@/lib/api";
export default function Home() { const router = useRouter(); useEffect(() => router.replace(session()?.user.role === "Admin" ? "/dashboard" : "/invoices"), [router]); return null; }
