// app/brand-owner/layout.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import Link from "next/link";
import {
  LayoutDashboard,
  Building2,
  Hotel,
  Users,
  Bell,
  User,
  LogOut,
  Menu,
  X,
  ChevronRight,
  BarChart3,
  Settings,
  Calendar,
  Truck,
  Megaphone,
  Home,
  CreditCard,
  Package,
  MapPin,
  Star,
  DollarSign,
  ClipboardList,
  Shield
} from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { 
  getStoredUser, 
  logout, 
  clearAuthData, 
  setupAutoRefresh,
  getTimeRemaining,
  isTokenNearExpiry,
  refreshAccessToken,
  validateSession,
  isAuthenticated
} from "@/lib/api";

const navItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/brand-owner" },
  { icon: Hotel, label: "Properties", href: "/brand-owner/properties" },
  { icon: Calendar, label: "Reservations", href: "/brand-owner/reservations" },
  { icon: Truck, label: "Orders", href: "/brand-owner/orders" },
  { 
    icon: Package, 
    label: "Logistics", 
    href: "/brand-owner/logistics",
    subItems: [
      { label: "Riders", href: "/brand-owner/logistics/riders" },
      { label: "Assign Orders", href: "/brand-owner/logistics/assign" },
      { label: "Track Deliveries", href: "/brand-owner/logistics/track" },
    ]
  },
  { 
    icon: Megaphone, 
    label: "Adverts", 
    href: "/brand-owner/adverts",
    subItems: [
      { label: "All Adverts", href: "/brand-owner/adverts" },
      { label: "Create Advert", href: "/brand-owner/adverts/add" },
    ]
  },
  { icon: Users, label: "Staff", href: "/brand-owner/staff" },
  { icon: BarChart3, label: "Reports", href: "/brand-owner/reports" },
  { icon: Bell, label: "Notifications", href: "/brand-owner/notifications" },
  { icon: User, label: "Profile", href: "/brand-owner/profile" },
  { icon: Settings, label: "Settings", href: "/brand-owner/settings" },
];

const AUTH_PAGES = [
  "/brand-owner/login",
  "/brand-owner/register",
  "/brand-owner/forgot-password",
  "/brand-owner/reset-password",
];

export default function BrandOwnerLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [user, setUser] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [expandedItems, setExpandedItems] = useState<string[]>([]);
  const [timeRemaining, setTimeRemaining] = useState<number>(0);
  const [isSessionValid, setIsSessionValid] = useState(true);

  const isAuthPage = AUTH_PAGES.some(page => pathname?.startsWith(page));

  // Check if user has Brand Owner role
  const isBrandOwner = (userData: any) => {
    return userData?.userType === "BrandOwner" || 
           userData?.staffType === "BrandOwner" ||
           userData?.role === "BrandOwner" ||
           userData?.roles?.includes("BrandOwner") ||
           userData?.adminType === "BrandOwner";
  };

  // Session monitoring with proper checks
  useEffect(() => {
    // If on auth page, don't run session checks
    if (isAuthPage) {
      setIsLoading(false);
      return;
    }

    const checkSession = () => {
      try {
        if (!isAuthenticated()) {
          console.log("🔒 No token found, redirecting to login");
          clearAuthData();
          router.replace("/brand-owner/login");
          return false;
        }

        if (!validateSession()) {
          console.log("⏰ Session expired or invalid, redirecting to login");
          clearAuthData();
          router.replace("/brand-owner/login");
          return false;
        }

        const stored = getStoredUser();
        if (!stored) {
          console.log("👤 No user found, redirecting to login");
          clearAuthData();
          router.replace("/brand-owner/login");
          return false;
        }

        // Verify user is a Brand Owner
        if (!isBrandOwner(stored)) {
          console.log("❌ User is not a Brand Owner");
          clearAuthData();
          router.replace("/brand-owner/login");
          return false;
        }

        // Update time remaining
        const remaining = getTimeRemaining();
        setTimeRemaining(remaining);

        // If token is near expiry, try to refresh
        if (isTokenNearExpiry() && remaining > 0) {
          console.log("🔄 Token near expiry, attempting refresh...");
          refreshAccessToken().then(refreshed => {
            if (refreshed) {
              console.log("✅ Token refreshed successfully");
              // Update user data after refresh
              const updatedUser = getStoredUser();
              if (updatedUser) {
                setUser(updatedUser);
              }
            }
          });
        }

        // If token is expired, redirect
        if (remaining <= 0) {
          console.log("⏰ Session expired, redirecting to login");
          clearAuthData();
          router.replace("/brand-owner/login");
          return false;
        }

        setUser(stored);
        setIsSessionValid(true);
        return true;

      } catch (error) {
        console.error("❌ Session check error:", error);
        clearAuthData();
        router.replace("/brand-owner/login");
        return false;
      }
    };

    // Initial check
    const isValid = checkSession();
    setIsLoading(false);

    if (!isValid) {
      return;
    }

    // Setup auto-refresh with delay
    const autoRefreshTimeout = setTimeout(() => {
      setupAutoRefresh();
    }, 3000);

    // Check session every 30 seconds
    const interval = setInterval(checkSession, 30000);
    
    // Check on visibility change
    const handleVisibilityChange = () => {
      if (document.visibilityState === "visible") {
        console.log("👁️ Tab became visible, checking session...");
        checkSession();
      }
    };
    document.addEventListener("visibilitychange", handleVisibilityChange);
    
    // Check on focus
    const handleFocus = () => {
      console.log("🎯 Page focused, checking session...");
      checkSession();
    };
    window.addEventListener("focus", handleFocus);

    return () => {
      clearTimeout(autoRefreshTimeout);
      clearInterval(interval);
      document.removeEventListener("visibilitychange", handleVisibilityChange);
      window.removeEventListener("focus", handleFocus);
    };
  }, [router, pathname, isAuthPage]);

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      clearAuthData();
      router.push("/brand-owner/login");
    }
  };

  const getInitials = () => {
    if (!user) return "B";
    return `${user.firstName?.[0] || ""}${user.surName?.[0] || user.lastName?.[0] || ""}`.toUpperCase();
  };

  const toggleExpand = (label: string) => {
    setExpandedItems(prev =>
      prev.includes(label)
        ? prev.filter(item => item !== label)
        : [...prev, label]
    );
  };

  const isActive = (href: string) => {
    return pathname === href || pathname?.startsWith(href + "/");
  };

  const formatTimeRemaining = (minutes: number) => {
    if (minutes <= 0) return "Expired";
    if (minutes < 1) return "Less than 1 min";
    if (minutes < 5) return `${minutes} min - Expiring soon!`;
    return `${minutes} min remaining`;
  };

  // Show loading only on protected pages
  if (isLoading && !isAuthPage) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-gray-500 text-sm">Loading...</p>
        </div>
      </div>
    );
  }

  // Render auth pages without sidebar
  if (isAuthPage) {
    return <>{children}</>;
  }

  return (
    <div className="min-h-screen flex bg-gray-50">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div className="fixed inset-0 bg-black/50 z-40 lg:hidden" onClick={() => setSidebarOpen(false)} />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-50 w-72 bg-gray-900 text-white transition-transform duration-300 lg:static lg:translate-x-0 flex flex-col ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        {/* Logo */}
        <div className="flex items-center justify-between px-6 py-5 border-b border-gray-800 flex-shrink-0">
          <div>
            <h1 className="text-xl font-bold tracking-tight">GINILOG</h1>
            <p className="text-xs text-gray-400 mt-0.5">Brand Owner Portal</p>
          </div>
          <button 
            className="lg:hidden text-gray-400 hover:text-white" 
            onClick={() => setSidebarOpen(false)}
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* User Profile */}
        <div className="px-6 py-4 border-b border-gray-800 flex-shrink-0">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-full bg-primary/20 flex items-center justify-center overflow-hidden flex-shrink-0">
              {user?.profilePicture ? (
                <img src={user.profilePicture} alt="Profile" className="h-full w-full object-cover" />
              ) : (
                <span className="text-sm font-semibold text-primary">{getInitials()}</span>
              )}
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-sm font-medium truncate">
                {user?.firstName} {user?.surName || user?.lastName}
              </p>
              <p className="text-xs text-gray-400 truncate">
                {user?.companyName || "Brand Owner"}
              </p>
            </div>
            {isBrandOwner(user) && (
              <Shield className="h-4 w-4 text-primary flex-shrink-0" />
            )}
          </div>
          {/* Session timer */}
          <div className="mt-2 text-xs">
            <span className={timeRemaining < 5 ? "text-red-400" : "text-gray-400"}>
              ⏱ {formatTimeRemaining(timeRemaining)}
            </span>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
          {navItems.map((item) => {
            const active = isActive(item.href);
            const hasSubItems = item.subItems && item.subItems.length > 0;
            const isExpanded = expandedItems.includes(item.label);

            return (
              <div key={item.href}>
                {hasSubItems ? (
                  <>
                    <button
                      onClick={() => {
                        toggleExpand(item.label);
                        if (!isExpanded && item.subItems && item.subItems.length > 0) {
                          router.push(item.subItems[0].href);
                        }
                      }}
                      className={`flex items-center gap-3 px-4 py-2.5 w-full rounded-lg text-sm font-medium transition-colors ${
                        active || item.subItems?.some(sub => isActive(sub.href))
                          ? "bg-primary/10 text-primary"
                          : "text-gray-300 hover:bg-gray-800 hover:text-white"
                      }`}
                    >
                      <item.icon className="h-5 w-5 flex-shrink-0" />
                      <span className="flex-1 text-left">{item.label}</span>
                      <ChevronRight 
                        className={`h-4 w-4 transition-transform ${isExpanded ? 'rotate-90' : ''}`} 
                      />
                    </button>
                    {isExpanded && item.subItems && (
                      <div className="ml-8 space-y-1 mt-1">
                        {item.subItems.map((subItem) => (
                          <Link
                            key={subItem.href}
                            href={subItem.href}
                            onClick={() => setSidebarOpen(false)}
                            className={`flex items-center gap-3 px-4 py-2 rounded-lg text-sm transition-colors ${
                              isActive(subItem.href)
                                ? "bg-white/20 text-white"
                                : "text-gray-400 hover:bg-gray-800 hover:text-white"
                            }`}
                          >
                            <span>{subItem.label}</span>
                          </Link>
                        ))}
                      </div>
                    )}
                  </>
                ) : (
                  <Link
                    href={item.href}
                    onClick={() => setSidebarOpen(false)}
                    className={`flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                      active
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
                )}
              </div>
            );
          })}
        </nav>

        {/* Footer */}
        <div className="px-3 py-4 border-t border-gray-800 flex-shrink-0">
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
        {/* Header */}
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
                  {navItems.find(n => n.href === pathname)?.label || 
                   navItems.find(n => n.subItems?.some(s => s.href === pathname))?.label || 
                   "Dashboard"}
                </h2>
                <p className="text-xs text-gray-500 hidden sm:block">
                  Welcome back, {user?.firstName || "Brand Owner"}!
                </p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              {/* Session timer in header */}
              <span className={`text-xs hidden sm:block ${timeRemaining < 5 ? 'text-red-500' : 'text-gray-400'}`}>
                ⏱ {formatTimeRemaining(timeRemaining)}
              </span>
              <Button 
                variant="ghost" 
                size="sm" 
                className="relative" 
                onClick={() => router.push("/brand-owner/notifications")}
              >
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