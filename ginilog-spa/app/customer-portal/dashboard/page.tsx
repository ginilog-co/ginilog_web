"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Search,
  Package,
  Home,
  Truck,
  User,
  LogOut,
  CreditCard,
  Clock,
  CheckCircle,
  TruckIcon,
  Building2,
  Loader2,
  AlertTriangle,
  Menu,
  X,
  LayoutDashboard,
  ShoppingBag,
  UserCircle,
  ChevronRight,
  LogOut as LogOutIcon,
  Sparkles,
  Star
} from "lucide-react";
import { Label } from "@/components/ui/label";
import {
  getProfile,
  getStoredUser,
  UserProfile,
  getAccommodations,
  getCompanies,
  getCustomerOrders,
  getCustomerBookings,
  logout,
  clearAuthData,
  Accommodation,
  Company
} from "@/lib/api";
import { Badge } from "@/components/ui/badge";

function isUnauthorizedError(error: unknown): boolean {
  if (!(error instanceof Error)) return false;
  const msg = error.message.toLowerCase();
  return msg.includes("401") || msg.includes("unauthorized") || msg.includes("unauthenticated");
}

// Guards against a dashboard <-> login redirect loop
const REDIRECT_GUARD_KEY = "dashboardAuthRedirectCount";
const REDIRECT_GUARD_LIMIT = 1;

function canAutoRedirectToLogin(): boolean {
  if (typeof window === "undefined") return true;
  const count = Number(sessionStorage.getItem(REDIRECT_GUARD_KEY) || "0");
  return count < REDIRECT_GUARD_LIMIT;
}

function recordRedirectAttempt(): void {
  if (typeof window === "undefined") return;
  const count = Number(sessionStorage.getItem(REDIRECT_GUARD_KEY) || "0");
  sessionStorage.setItem(REDIRECT_GUARD_KEY, String(count + 1));
}

function clearRedirectGuard(): void {
  if (typeof window === "undefined") return;
  sessionStorage.removeItem(REDIRECT_GUARD_KEY);
}

export default function CustomerDashboard() {
  const router = useRouter();
  const [user, setUser] = useState<UserProfile | null>(null);
  const [accommodations, setAccommodations] = useState<Accommodation[]>([]);
  const [companies, setCompanies] = useState<Company[]>([]);
  const [recentActivity, setRecentActivity] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [trackingNumber, setTrackingNumber] = useState("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [needsManualLogin, setNeedsManualLogin] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    let isMounted = true;

    const goToLogin = () => {
      if (canAutoRedirectToLogin()) {
        recordRedirectAttempt();
        router.push("/customer-portal/login");
      } else {
        setNeedsManualLogin(true);
        setIsLoading(false);
      }
    };

    const fetchDashboardData = async () => {
      const storedUser = getStoredUser();
      if (!storedUser) {
        goToLogin();
        return;
      }

      try {
        const [profileResult, accomsResult, compsResult, ordersResult, bookingsResult] =
          await Promise.allSettled([
            getProfile(),
            getAccommodations(),
            getCompanies(),
            getCustomerOrders(),
            getCustomerBookings(),
          ]);

        if (!isMounted) return;

        if (profileResult.status === "rejected") {
          console.error("Failed to fetch profile:", profileResult.reason);
          if (isUnauthorizedError(profileResult.reason)) {
            goToLogin();
            return;
          }
          setErrorMessage(
            profileResult.reason instanceof Error
              ? profileResult.reason.message
              : "Failed to load your profile. Please try again."
          );
          setIsLoading(false);
          return;
        }

        clearRedirectGuard();
        setUser(profileResult.value);

        if (accomsResult.status === "fulfilled") {
          setAccommodations(accomsResult.value || []);
        } else {
          console.error("Failed to fetch accommodations:", accomsResult.reason);
        }

        if (compsResult.status === "fulfilled") {
          setCompanies(compsResult.value || []);
        } else {
          console.error("Failed to fetch companies:", compsResult.reason);
        }

        const orders = ordersResult.status === "fulfilled" ? ordersResult.value : [];
        if (ordersResult.status === "rejected") {
          console.error("Failed to fetch orders:", ordersResult.reason);
        }

        const bookings = bookingsResult.status === "fulfilled" ? bookingsResult.value : [];
        if (bookingsResult.status === "rejected") {
          console.error("Failed to fetch bookings:", bookingsResult.reason);
        }

        const combined = [
          ...(orders || []).map((o: any) => ({ ...o, kind: "logistics", label: o.itemName || "Package", ref: o.trackingNum || o.id })),
          ...(bookings || []).map((b: any) => ({ ...b, kind: "accommodation", label: b.accomodationName || "Hotel", ref: b.bookingRefNo || b.id })),
        ]
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 3);

        setRecentActivity(combined);
      } catch (error) {
        console.error("Unexpected error loading dashboard:", error);
        if (isMounted) {
          setErrorMessage(
            error instanceof Error ? error.message : "Something went wrong loading your dashboard."
          );
        }
      } finally {
        if (isMounted) setIsLoading(false);
      }
    };

    fetchDashboardData();

    return () => {
      isMounted = false;
    };
  }, [router]);

  const handleTracking = (e: React.FormEvent) => {
    e.preventDefault();
    if (trackingNumber) {
      router.push(`/tracking?id=${encodeURIComponent(trackingNumber)}`);
    }
  };

  const handleLogout = () => {
    logout();
    clearAuthData();
    router.push("/customer-portal/login");
  };

  // Mobile menu navigation items - includes Logout as a menu item
  const menuItems = [
    { icon: LayoutDashboard, label: "Dashboard", href: "/customer-portal/dashboard" },
    { icon: ShoppingBag, label: "My Orders & Bookings", href: "/customer-portal/orders" },
    { icon: UserCircle, label: "Profile", href: "/customer-portal/profile" },
    { icon: LogOutIcon, label: "Logout", href: "#", isLogout: true },
  ];

  // Get user initials for avatar fallback
  const getInitials = () => {
    if (!user) return "U";
    return `${user.firstName?.[0] || ""}${user.lastName?.[0] || ""}`.toUpperCase();
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (needsManualLogin) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <Card className="max-w-md w-full">
          <CardContent className="p-8 text-center">
            <User className="h-10 w-10 text-primary mx-auto mb-4" />
            <h2 className="text-lg font-semibold text-gray-900 mb-2">
              Please sign in again
            </h2>
            <p className="text-sm text-gray-600 mb-6">
              Your session couldn&apos;t be verified. This can happen if your sign-in
              expired or didn&apos;t fully complete. Please sign in again to continue.
            </p>
            <Button
              onClick={() => {
                clearAuthData();
                router.push("/customer-portal/login");
              }}
              className="w-full"
            >
              Go to login
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (errorMessage) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <Card className="max-w-md w-full">
          <CardContent className="p-8 text-center">
            <AlertTriangle className="h-10 w-10 text-amber-500 mx-auto mb-4" />
            <h2 className="text-lg font-semibold text-gray-900 mb-2">
              Couldn&apos;t load your dashboard
            </h2>
            <p className="text-sm text-gray-600 mb-6">{errorMessage}</p>
            <Button onClick={() => window.location.reload()} className="w-full">
              Try again
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (!user) return null;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-50 shadow-sm">
        <div className="container mx-auto px-4 py-3">
          <div className="flex items-center justify-between">
            <Link href="/" className="text-2xl font-bold text-primary flex items-center gap-2">
          
              GINILOG
            </Link>

            {/* Desktop Navigation */}
            <nav className="hidden md:flex items-center space-x-8">
              <Link href="/customer-portal/dashboard" className="text-gray-900 font-medium hover:text-primary transition-colors">
                Dashboard
              </Link>
              <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900 transition-colors">
                My Orders & Bookings
              </Link>
              <Link href="/customer-portal/profile" className="text-gray-600 hover:text-gray-900 transition-colors">
                Profile
              </Link>
            </nav>

            {/* Desktop Right Side */}
            <div className="hidden md:flex items-center gap-4">
              {/* User Profile */}
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full border-2 border-primary/20 overflow-hidden bg-primary/10 flex items-center justify-center flex-shrink-0">
                  {user.profilePicture ? (
                    <img 
                      src={user.profilePicture} 
                      alt={user.firstName} 
                      className="h-full w-full object-cover" 
                    />
                  ) : (
                    <span className="text-primary font-semibold text-sm">
                      {getInitials()}
                    </span>
                  )}
                </div>
                <div className="hidden lg:block text-left">
                  <p className="text-sm font-medium text-gray-900">
                    {user.firstName} {user.lastName}
                  </p>
                  <p className="text-xs text-gray-500">GINILOG Customer</p>
                </div>
              </div>

              {/* Logout Button */}
              <Button
                variant="ghost"
                size="sm"
                className="text-red-600 hover:text-red-700 hover:bg-red-50"
                onClick={handleLogout}
              >
                <LogOut className="h-4 w-4 mr-2" />
                Logout
              </Button>
            </div>

            {/* Mobile Menu Button */}
            <button
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="md:hidden p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors focus:outline-none"
              aria-label="Toggle menu"
            >
              {isMobileMenuOpen ? (
                <X className="h-6 w-6" />
              ) : (
                <Menu className="h-6 w-6" />
              )}
            </button>
          </div>
        </div>

        {/* Mobile Menu Overlay */}
        {isMobileMenuOpen && (
          <div className="md:hidden fixed inset-0 z-50 bg-black/50" onClick={() => setIsMobileMenuOpen(false)}>
            <div className="absolute right-0 top-0 h-full w-80 max-w-[85vw] bg-white shadow-2xl" onClick={(e) => e.stopPropagation()}>
              {/* Mobile Menu Header */}
              <div className="p-4 border-b bg-gradient-to-r from-primary/5 to-blue-50">
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-full border-2 border-primary/20 overflow-hidden bg-primary/10 flex items-center justify-center flex-shrink-0">
                      {user.profilePicture ? (
                        <img 
                          src={user.profilePicture} 
                          alt={user.firstName} 
                          className="h-full w-full object-cover" 
                        />
                      ) : (
                        <span className="text-primary font-semibold text-lg">
                          {getInitials()}
                        </span>
                      )}
                    </div>
                    <div>
                      <p className="font-semibold text-gray-900 text-base">
                        {user.firstName} {user.lastName}
                      </p>
                      <p className="text-xs text-gray-500">{user.email}</p>
                    </div>
                  </div>
                  <button
                    onClick={() => setIsMobileMenuOpen(false)}
                    className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  >
                    <X className="h-5 w-5" />
                  </button>
                </div>
                {/* Quick stats in mobile menu */}
                <div className="flex items-center gap-4 text-xs">
                  <div className="flex items-center gap-1">
                    <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />
                    <span className="text-gray-600">4.8</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <CheckCircle className="h-3 w-3 text-green-500" />
                    <span className="text-gray-600">{recentActivity.length} activities</span>
                  </div>
                </div>
              </div>

              {/* Mobile Menu Items - includes Logout */}
              <nav className="p-4 space-y-1 overflow-y-auto" style={{ maxHeight: 'calc(100vh - 200px)' }}>
                {menuItems.map((item) => (
                  item.isLogout ? (
                    // Logout button in mobile menu
                    <button
                      key={item.label}
                      onClick={() => {
                        setIsMobileMenuOpen(false);
                        handleLogout();
                      }}
                      className="w-full flex items-center justify-between p-3 rounded-lg hover:bg-red-50 transition-colors group"
                    >
                      <div className="flex items-center gap-3">
                        <div className="h-8 w-8 rounded-lg bg-red-50 group-hover:bg-red-100 flex items-center justify-center transition-colors">
                          <item.icon className="h-4 w-4 text-red-600 group-hover:text-red-700 transition-colors" />
                        </div>
                        <span className="text-red-600 group-hover:text-red-700 font-medium transition-colors">
                          {item.label}
                        </span>
                      </div>
                      <ChevronRight className="h-4 w-4 text-red-400 group-hover:text-red-500 transition-colors" />
                    </button>
                  ) : (
                    <Link
                      key={item.href}
                      href={item.href}
                      onClick={() => setIsMobileMenuOpen(false)}
                      className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors group"
                    >
                      <div className="flex items-center gap-3">
                        <div className="h-8 w-8 rounded-lg bg-gray-100 group-hover:bg-primary/10 flex items-center justify-center transition-colors">
                          <item.icon className="h-4 w-4 text-gray-500 group-hover:text-primary transition-colors" />
                        </div>
                        <span className="text-gray-700 group-hover:text-primary font-medium transition-colors">
                          {item.label}
                        </span>
                      </div>
                      <ChevronRight className="h-4 w-4 text-gray-400 group-hover:text-primary transition-colors" />
                    </Link>
                  )
                ))}
              </nav>
            </div>
          </div>
        )}
      </header>

      <main className="container mx-auto px-4 py-8">
        {/* Welcome Section */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Welcome back, {user.firstName}! 👋
          </h1>
          <p className="text-gray-600 mt-1">
            Manage your bookings, track packages, and access all GINILOG services.
          </p>
        </div>

        {/* Quick Actions - Unified for all users */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <Card className="bg-primary text-white shadow-lg shadow-primary/20">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-white/80 text-sm">Wallet Balance</p>
                  <p className="text-3xl font-bold mt-1">₦{user.moneyBoxBalance?.toLocaleString() || '0'}</p>
                </div>
                <CreditCard className="h-8 w-8 text-white/80" />
              </div>
            </CardContent>
          </Card>
          <Card className="shadow-md hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">Referral Code</p>
                  <p className="text-xl font-bold mt-1 text-gray-900">{user.referralCode || 'N/A'}</p>
                </div>
                <User className="h-8 w-8 text-primary" />
              </div>
            </CardContent>
          </Card>
          <Card className="shadow-md hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <form onSubmit={handleTracking} className="space-y-2">
                <Label htmlFor="track" className="text-sm text-gray-500">Track Package/Booking</Label>
                <div className="flex gap-2">
                  <Input 
                    id="track" 
                    placeholder="Enter ID..." 
                    value={trackingNumber}
                    onChange={(e) => setTrackingNumber(e.target.value)}
                  />
                  <Button size="icon" type="submit"><Search className="h-4 w-4" /></Button>
                </div>
              </form>
            </CardContent>
          </Card>
          <Card className="shadow-md hover:shadow-lg transition-shadow">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">Status</p>
                  <p className="text-xl font-bold mt-1 text-green-600">{user.userStatus ? 'Active' : 'Inactive'}</p>
                </div>
                <CheckCircle className="h-8 w-8 text-green-600" />
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Unified Dashboard - All Services Available */}
        <div className="space-y-8">
          {/* Services Grid */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            {/* Accommodation Section */}
            <Card className="shadow-md hover:shadow-lg transition-shadow overflow-hidden">
              <CardHeader className="flex flex-row items-center justify-between bg-gradient-to-r from-blue-600 to-blue-800 text-white rounded-t-lg">
                <div className="flex items-center gap-2">
                  <Building2 className="h-5 w-5" />
                  <CardTitle className="text-white">Accommodation</CardTitle>
                </div>
                <Link href="/customer-portal/accommodations">
                  <Button variant="secondary" size="sm" className="bg-white/20 hover:bg-white/30 text-white border-0">
                    Explore
                  </Button>
                </Link>
              </CardHeader>
              <CardContent className="p-6">
                <p className="text-gray-600 mb-4">Book hotels, apartments, and resorts for your stays.</p>
                <div className="space-y-3">
                  {accommodations.slice(0, 3).map((acc) => (
                    <div key={acc.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50 transition-colors">
                      <div className="h-12 w-12 rounded-lg overflow-hidden flex-shrink-0">
                        <img src={acc.accomodationImages?.[0] || "/service-1.jpg"} alt={acc.accomodationName} className="h-full w-full object-cover" />
                      </div>
                      <div className="flex-1">
                        <h4 className="font-semibold text-gray-900 text-sm">{acc.accomodationName}</h4>
                        <p className="text-xs text-gray-500">{acc.location}</p>
                      </div>
                    </div>
                  ))}
                  {accommodations.length === 0 && (
                    <div className="text-center py-4 text-gray-500 italic text-sm">
                      No accommodations available at the moment.
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>

            {/* Logistics Section */}
            <Card className="shadow-md hover:shadow-lg transition-shadow overflow-hidden">
              <CardHeader className="flex flex-row items-center justify-between bg-gradient-to-r from-primary to-red-800 text-white rounded-t-lg">
                <div className="flex items-center gap-2">
                  <Truck className="h-5 w-5" />
                  <CardTitle className="text-white">Logistics</CardTitle>
                </div>
                <Link href="/customer-portal/logistics">
                  <Button variant="secondary" size="sm" className="bg-white/20 hover:bg-white/30 text-white border-0">
                    Send Package
                  </Button>
                </Link>
              </CardHeader>
              <CardContent className="p-6">
                <p className="text-gray-600 mb-4">Send packages and track deliveries nationwide.</p>
                <div className="space-y-3">
                  {companies.slice(0, 3).map((comp) => (
                    <div key={comp.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50 transition-colors">
                      <div className="h-10 w-10 rounded-lg overflow-hidden flex-shrink-0 bg-gray-100 flex items-center justify-center">
                        {comp.companyLogo ? (
                          <img src={comp.companyLogo} alt={comp.companyName} className="h-full w-full object-cover" />
                        ) : (
                          <Truck className="h-5 w-5 text-gray-400" />
                        )}
                      </div>
                      <div className="flex-1">
                        <h4 className="font-semibold text-gray-900 text-sm">{comp.companyName}</h4>
                        <p className="text-xs text-gray-500">Starting from ₦{comp.valueCharge?.toLocaleString()}</p>
                      </div>
                    </div>
                  ))}
                  {companies.length === 0 && (
                    <div className="text-center py-4 text-gray-500 italic text-sm">
                      No logistics companies available.
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Quick Actions Row */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Link href="/customer-portal/accommodations">
              <Card className="hover:shadow-lg transition-shadow cursor-pointer hover:scale-[1.02] transform duration-200">
                <CardContent className="p-6">
                  <div className="flex items-center gap-4">
                    <div className="h-12 w-12 bg-blue-100 rounded-lg flex items-center justify-center">
                      <Home className="h-6 w-6 text-blue-600" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-gray-900">Find Stays</h3>
                      <p className="text-sm text-gray-500">Search accommodations</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </Link>
            <Link href="/customer-portal/logistics">
              <Card className="hover:shadow-lg transition-shadow cursor-pointer hover:scale-[1.02] transform duration-200">
                <CardContent className="p-6">
                  <div className="flex items-center gap-4">
                    <div className="h-12 w-12 bg-primary/10 rounded-lg flex items-center justify-center">
                      <Package className="h-6 w-6 text-primary" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-gray-900">Send Package</h3>
                      <p className="text-sm text-gray-500">Book logistics service</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </Link>
            <Link href="/tracking">
              <Card className="hover:shadow-lg transition-shadow cursor-pointer hover:scale-[1.02] transform duration-200">
                <CardContent className="p-6">
                  <div className="flex items-center gap-4">
                    <div className="h-12 w-12 bg-green-100 rounded-lg flex items-center justify-center">
                      <Search className="h-6 w-6 text-green-600" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-gray-900">Track Item</h3>
                      <p className="text-sm text-gray-500">Track orders & bookings</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </Link>
          </div>

          {/* Recent Activity */}
          <Card className="shadow-md hover:shadow-lg transition-shadow">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Clock className="h-5 w-5 text-primary" />
                Recent Activity
              </CardTitle>
            </CardHeader>
            <CardContent>
              {recentActivity.length === 0 ? (
                <p className="text-sm text-gray-500 text-center py-4">No recent activity yet.</p>
              ) : (
                <div className="space-y-3">
                  {recentActivity.map((item) => (
                    <div key={item.id} className="flex items-center gap-3 text-sm p-2 rounded-lg hover:bg-gray-50 transition-colors">
                      <div className={`h-8 w-8 rounded-full flex items-center justify-center ${item.kind === "accommodation" ? "bg-blue-100" : "bg-primary/10"}`}>
                        {item.kind === "accommodation"
                          ? <Building2 className="h-4 w-4 text-blue-600" />
                          : <TruckIcon className="h-4 w-4 text-primary" />}
                      </div>
                      <div className="flex-1">
                        <p className="text-gray-900 font-medium">{item.label}</p>
                        <p className="text-gray-500 text-xs">
                          {item.orderStatus || item.bookingStatus || "Pending"} · {item.ref}
                        </p>
                      </div>
                      <Badge variant="outline" className="text-xs">
                        {item.kind === "accommodation" ? "Booking" : "Order"}
                      </Badge>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}