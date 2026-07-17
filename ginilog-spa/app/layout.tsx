import type { Metadata } from "next";
import { Inter, Geist } from "next/font/google";
import "./globals.css";
import { AppShell } from "@/components/app-shell";
import { Analytics } from "@vercel/analytics/next";
import { cn } from "@/lib/utils";
import { DownloadAppBanner } from "@/components/DownloadAppBanner";

// floating variant
<DownloadAppBanner
  variant="floating"
  isDismissible={true}
/>

const geist = Geist({subsets:['latin'],variable:'--font-sans'});

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Ginilog - Logistics and Accommodation Solutions",
  description: "Your one-stop shop for logistics and accommodation services",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={cn("font-sans", geist.variable)}>
      <body className={inter.className}>
        <AppShell>{children}</AppShell>
        <Analytics />
      </body>
    </html>
  );
}
