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
  Bell,
  CreditCard,
  Clock,
  CheckCircle,
  TruckIcon,
  Building2,
  Loader2
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
  Accommodation,
  Company
} from "@/lib/api";

export default function CustomerDashboard() {
  const router = useRouter();
  const [user, setUser] = useState<UserProfile | null>(null);
  const [accommodations, setAccommodations] = useState<Accommodation[]>([]);
  const [companies, setCompanies] = useState<Company[]>([]);
  const [recentActivity, setRecentActivity] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [trackingNumber, setTrackingNumber] = useState("");

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const storedUser = getStoredUser();
        if (!storedUser) {
          router.push("/customer-portal/login");
          return;
        }
        const [profile, accoms, comps, orders, bookings] = await Promise.all([
          getProfile(),
          getAccommodations(),
          getCompanies(),
          getCustomerOrders(),
          getCustomerBookings(),
        ]);
        setUser(profile);
        setAccommodations(accoms || []);
        setCompanies(comps || []);
        const combined = [
          ...(orders || []).map((o: any) => ({ ...o, kind: "logistics", label: o.itemName || "Package", ref: o.trackingNum || o.id })),
          ...(bookings || []).map((b: any) => ({ ...b, kind: "accommodation", label: b.accomodationName || "Hotel", ref: b.bookingRefNo || b.id })),
        ]
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 3);
        setRecentActivity(combined);
      } catch (error) {
        console.error("Failed to fetch dashboard data:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchDashboardData();
  }, [router]);

  const handleTracking = (e: React.FormEvent) => {
    e.preventDefault();
    if (trackingNumber) {
      router.push(`/tracking?id=${encodeURIComponent(trackingNumber)}`);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!user) return null;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-50">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="text-2xl font-bold text-primary">
              GINILOG
            </Link>

          <nav className="hidden md:flex items-center space-x-8">
            <Link href="/customer-portal/dashboard" className="text-gray-900 font-medium">
              Dashboard
            </Link>
            <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900">
              My Orders & Bookings
            </Link>
            <Link href="/customer-portal/profile" className="text-gray-600 hover:text-gray-900">
              Profile
            </Link>
          </nav>

            <div className="flex items-center gap-4">
              <button className="relative p-2 text-gray-600 hover:text-gray-900">
                <Bell className="h-6 w-6" />
                <span className="absolute top-1 right-1 h-2 w-2 bg-primary rounded-full" />
              </button>
              
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                  {user.profilePicture ? (
                    <img src={user.profilePicture} alt={user.firstName} className="h-10 w-10 rounded-full object-cover" />
                  ) : (
                    <User className="h-5 w-5 text-primary" />
                  )}
                </div>
                <div className="hidden sm:block">
                  <p className="text-sm font-medium text-gray-900">
                    {user.firstName} {user.lastName}
                  </p>
                  <p className="text-xs text-gray-500">GINILOG Customer</p>
                </div>
              </div>

              <Button
                variant="ghost"
                size="icon"
                className="text-gray-600"
                onClick={() => { logout(); router.push("/customer-portal/login"); }}
              >
                <LogOut className="h-5 w-5" />
              </Button>
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        {/* Welcome Section */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Welcome back, {user.firstName}!
          </h1>
          <p className="text-gray-600 mt-1">
            Manage your bookings, track packages, and access all GINILOG services.
          </p>
        </div>

        {/* Quick Actions - Unified for all users */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <Card className="bg-primary text-white">
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
          <Card>
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
          <Card>
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
          <Card>
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
            <Card>
              <CardHeader className="flex flex-row items-center justify-between bg-gradient-to-r from-blue-600 to-blue-800 text-white rounded-t-lg">
                <div className="flex items-center gap-2">
                  <Building2 className="h-5 w-5" />
                  <CardTitle className="text-white">Accommodation</CardTitle>
                </div>
                <Link href="/customer-portal/accommodations">
                  <Button variant="secondary" size="sm">Explore</Button>
                </Link>
              </CardHeader>
              <CardContent className="p-6">
                <p className="text-gray-600 mb-4">Book hotels, apartments, and resorts for your stays.</p>
                <div className="space-y-3">
                  {accommodations.slice(0, 3).map((acc) => (
                    <div key={acc.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50">
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
            <Card>
              <CardHeader className="flex flex-row items-center justify-between bg-gradient-to-r from-primary to-red-800 text-white rounded-t-lg">
                <div className="flex items-center gap-2">
                  <Truck className="h-5 w-5" />
                  <CardTitle className="text-white">Logistics</CardTitle>
                </div>
                <Link href="/customer-portal/logistics">
                  <Button variant="secondary" size="sm">Send Package</Button>
                </Link>
              </CardHeader>
              <CardContent className="p-6">
                <p className="text-gray-600 mb-4">Send packages and track deliveries nationwide.</p>
                <div className="space-y-3">
                  {companies.slice(0, 3).map((comp) => (
                    <div key={comp.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50">
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
            <Card className="hover:shadow-lg transition-shadow cursor-pointer">
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
            <Card className="hover:shadow-lg transition-shadow cursor-pointer">
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
            <Card className="hover:shadow-lg transition-shadow cursor-pointer">
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
          </div>

          {/* Recent Activity */}
          <Card>
            <CardHeader>
              <CardTitle>Recent Activity</CardTitle>
            </CardHeader>
            <CardContent>
              {recentActivity.length === 0 ? (
                <p className="text-sm text-gray-500 text-center py-4">No recent activity yet.</p>
              ) : (
                <div className="space-y-3">
                  {recentActivity.map((item) => (
                    <div key={item.id} className="flex items-center gap-3 text-sm">
                      <div className={`h-8 w-8 rounded-full flex items-center justify-center ${item.kind === "accommodation" ? "bg-blue-100" : "bg-primary/10"}`}>
                        {item.kind === "accommodation"
                          ? <Building2 className="h-4 w-4 text-blue-600" />
                          : <TruckIcon className="h-4 w-4 text-primary" />}
                      </div>
                      <div>
                        <p className="text-gray-900">{item.label}</p>
                        <p className="text-gray-500 text-xs">
                          {item.orderStatus || item.bookingStatus || "Pending"} · {item.ref}
                        </p>
                      </div>
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
