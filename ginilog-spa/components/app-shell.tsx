"use client";

import { usePathname } from "next/navigation";
import { Navbar } from "@/components/navbar";
import { Footer } from "@/components/footer";

export function AppShell({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();

  const hideMarketingChrome =
    pathname.startsWith("/customer-portal") || 
    pathname.startsWith("/admin-dashboard") ||
    pathname.startsWith("/admin-login");

  if (hideMarketingChrome) {
    return <main>{children}</main>;
  }

  return (
    <>
      <Navbar />
      <main>{children}</main>
      <Footer />
    </>
  );
}
