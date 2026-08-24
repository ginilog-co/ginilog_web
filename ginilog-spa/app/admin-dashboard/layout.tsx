// app/admin-dashboard/layout.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import Link from "next/link";
import { 
  LayoutDashboard, 
  Users, 
  Building2, 
  Megaphone, 
  Bell, 
  LogOut, 
  Menu, 
  X,
  Settings,
  User,
  ChevronRight,
  Home,
  Package,
  DollarSign,
  BarChart3,
  UserCheck
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { getStoredUser, logout, clearAuthData, isAuthenticated, validateSession } from "@/lib/api";

const navItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/admin-dashboard" },
  { icon: Users, label: "Admins", href: "/admin-dashboard/admins" },
  { icon: Building2, label: "Brand Owners", href: "/admin-dashboard/brand-owners" },
  { icon: Megaphone, label: "Adverts", href: "/admin-dashboard/adverts" },
  { icon: Package, label: "Applications", href: "/admin-dashboard/company-applications" },
  { icon: DollarSign, label: "Payouts", href: "/admin-dashboard/payouts" },
  { icon: Bell, label: "Notifications", href: "/admin-dashboard/notifications" },
  { icon: BarChart3, label: "Reports", href: "/admin-dashboard/reports" },
  { icon: Settings, label: "Settings", href: "/admin-dashboard/settings" },
];

// Auth pages that don't need the sidebar
const AUTH_PAGES = [
  "/admin-dashboard/login",
  "/admin-dashboard/forgot-password",
  "/admin-dashboard/reset-password",
];

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [user, setUser] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const isAuthPage = AUTH_PAGES.some(page => pathname?.startsWith(page));

    if (isAuthPage) {
      setIsLoading(false);
      return;
    }

    const checkSession = () => {
      if (!isAuthenticated() || !validateSession()) {
        clearAuthData();
        router.replace("/admin-dashboard/login");
        return false;
      }

      const stored = getStoredUser();
      if (!stored) {
        clearAuthData();
        router.replace("/admin-dashboard/login");
        return false;
      }

      setUser(stored);
      setIsLoading(false);
      return true;
    };

    checkSession();
  }, [router, pathname]);

  const handleLogout = () => {
    logout();
    clearAuthData();
    router.push("/admin-dashboard/login");
  };

  const getInitials = () => {
    if (!user) return "A";
    return `${user.firstName?.[0] || ""}${user.surName?.[0] || user.lastName?.[0] || ""}`.toUpperCase();
  };

  // Check if current page is an auth page
  const isAuthPage = AUTH_PAGES.some(page => pathname?.startsWith(page));

  // Show loading only while checking auth for protected pages
  if (isLoading && !isAuthPage) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  // For auth pages, just render children without sidebar
  if (isAuthPage) {
    return <>{children}</>;
  }

  // For protected pages, show the full layout with sidebar
  return (
    <div className="min-h-screen flex bg-gray-50">
      {/* Mobile Overlay */}
      {sidebarOpen && (
        <div 
          className="fixed inset-0 bg-black/50 z-40 lg:hidden" 
          onClick={() => setSidebarOpen(false)} 
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-50 w-72 bg-gray-900 text-white transition-transform duration-300 lg:static lg:translate-x-0 ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        {/* Brand */}
        <div className="flex items-center justify-between px-6 py-5 border-b border-gray-800">
          <div>
            <h1 className="text-xl font-bold tracking-tight">GINILOG</h1>
            <p className="text-xs text-gray-400 mt-0.5">Admin Dashboard</p>
          </div>
          <button 
            className="lg:hidden text-gray-400 hover:text-white" 
            onClick={() => setSidebarOpen(false)}
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* User Profile */}
        <div className="px-6 py-4 border-b border-gray-800">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-full bg-primary/20 flex items-center justify-center overflow-hidden flex-shrink-0">
              {user?.profilePicture ? (
                <img src={user.profilePicture} alt="Profile" className="h-full w-full object-cover" />
              ) : (
                <span className="text-sm font-semibold text-primary">{getInitials()}</span>
              )}
            </div>
            <div className="min-w-0">
              <p className="text-sm font-medium truncate">
                {user?.firstName} {user?.surName || user?.lastName}
              </p>
              <p className="text-xs text-gray-400 truncate">
                {user?.adminType || "Administrator"}
              </p>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
          {navItems.map((item) => {
            const isActive = pathname === item.href || pathname?.startsWith(item.href + "/");
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => setSidebarOpen(false)}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                  isActive
                    ? "bg-primary/10 text-primary"
                    : "text-gray-300 hover:bg-gray-800 hover:text-white"
                }`}
              >
                <item.icon className="h-5 w-5 flex-shrink-0" />
                <span className="flex-1">{item.label}</span>
                {item.label === "Notifications" && (
                  <Badge className="bg-red-500 text-white text-xs px-1.5 py-0.5">3</Badge>
                )}
              </Link>
            );
          })}
        </nav>

        {/* Footer */}
        <div className="px-3 py-4 border-t border-gray-800">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 px-4 py-2.5 w-full rounded-lg text-sm font-medium text-red-400 hover:bg-red-500/10 hover:text-red-300 transition-colors"
          >
            <LogOut className="h-5 w-5" />
            Logout
          </button>
          <Link
            href="/"
            className="flex items-center gap-2 px-4 py-2 mt-1 rounded-lg text-xs text-gray-500 hover:text-gray-300 transition-colors"
          >
            <Home className="h-4 w-4" />
            Back to Home
          </Link>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Top Bar */}
        <header className="bg-white border-b border-gray-200 sticky top-0 z-30">
          <div className="flex items-center justify-between px-4 py-3">
            <div className="flex items-center gap-3">
              <button
                className="lg:hidden p-2 rounded-lg hover:bg-gray-100"
                onClick={() => setSidebarOpen(true)}
              >
                <Menu className="h-5 w-5 text-gray-600" />
              </button>
              <div>
                <h2 className="font-semibold text-gray-900">
                  {navItems.find(n => n.href === pathname)?.label || "Dashboard"}
                </h2>
                <p className="text-xs text-gray-500 hidden sm:block">
                  Welcome back, {user?.firstName || "Admin"}!
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Button variant="ghost" size="sm" className="relative" onClick={() => router.push("/admin-dashboard/notifications")}>
                <Bell className="h-5 w-5 text-gray-500" />
                <Badge className="absolute -top-0.5 -right-0.5 h-5 w-5 flex items-center justify-center p-0 text-xs bg-red-500 text-white border-2 border-white">
                  3
                </Badge>
              </Button>
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 p-6 overflow-auto">
          {children}
        </main>
      </div>
    </div>
  );
}