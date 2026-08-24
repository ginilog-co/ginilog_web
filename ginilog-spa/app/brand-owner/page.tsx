"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  AlertCircle,
  ArrowRight,
  Bell,
  Building2,
  Calendar,
  CheckCircle,
  Clock,
  DollarSign,
  Eye,
  Hotel,
  Loader2,
  MapPin,
  Package,
  Plus,
  Star,
  Truck,
  Users,
  XCircle,
} from "lucide-react";
import {
  getAccommodationReservations,
  getAccommodations,
  getAllRiders,
  getCompanies,
  getNotifications,
  getPackageOrders,
  getPayouts,
  getStoredUser,
  isAuthenticated,
  logout,
  validateSession,
} from "@/lib/api";

export default function BrandOwnerDashboard() {
  const router = useRouter();
  const [user, setUser] = useState<any>(null);
  const [company, setCompany] = useState<any>(null);
  const [properties, setProperties] = useState<any[]>([]);
  const [bookings, setBookings] = useState<any[]>([]);
  const [orders, setOrders] = useState<any[]>([]);
  const [riders, setRiders] = useState<any[]>([]);
  const [notifications, setNotifications] = useState<any[]>([]);
  const [payouts, setPayouts] = useState<any[]>([]);
  const [stats, setStats] = useState({
    totalProperties: 0,
    activeProperties: 0,
    totalBookings: 0,
    pendingBookings: 0,
    totalOrders: 0,
    pendingOrders: 0,
    totalRiders: 0,
    totalRevenue: 0,
    totalPayouts: 0,
    rating: 0,
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated() || !validateSession()) {
      router.push("/brand-owner/login");
      return;
    }

    const stored = getStoredUser();
    setUser(stored);

    if (stored) {
      fetchDashboardData(stored);
    } else {
      setIsLoading(false);
    }
  }, [router]);

  const formatCurrency = (amount: number) => `₦${(amount || 0).toLocaleString("en-NG")}`;

  const fetchDashboardData = async (userData: any) => {
    setIsLoading(true);
    setError(null);

    try {
      const companyId = userData?.companyId || userData?.company?.id || userData?.company?.companyId;

      const [accommodations, reservations, ordersData, companyList, riderList, notificationsData, payoutsData] = await Promise.all([
        getAccommodations().catch(() => []),
        getAccommodationReservations().catch(() => []),
        getPackageOrders().catch(() => []),
        getCompanies().catch(() => []),
        getAllRiders().catch(() => []),
        getNotifications().catch(() => []),
        getPayouts().catch(() => []),
      ]);

      setProperties(accommodations || []);
      setBookings(reservations || []);
      setOrders(ordersData || []);
      setRiders(riderList || []);
      setNotifications((notificationsData || []).slice(0, 5));
      setPayouts(payoutsData || []);

      const matchedCompany =
        (companyList || []).find((item: any) =>
          item.id === companyId ||
          item.companyId === companyId ||
          item.adminId === userData?.userId ||
          item.companyName === userData?.companyName ||
          item.name === userData?.companyName
        ) ||
        (companyList || [])[0] ||
        null;

      setCompany(matchedCompany);

      const activeProperties = (accommodations || []).filter((item: any) =>
        item.isAvailable !== false && item.available !== false
      ).length;

      const pendingBookings = (reservations || []).filter((item: any) => {
        const status = (item.bookingStatus || item.status || "").toLowerCase();
        return status === "pending" || status === "in_review";
      }).length;

      const pendingOrders = (ordersData || []).filter((item: any) => {
        const status = (item.orderStatus || item.status || "").toLowerCase();
        return ["pending", "processing", "in_transit", "assigned", "awaiting_payment"].includes(status);
      }).length;

      const totalRevenue = (reservations || []).reduce((sum: number, item: any) => {
        return sum + Number(item.totalAmount || item.bookingAmount || 0);
      }, 0) + (ordersData || []).reduce((sum: number, item: any) => {
        return sum + Number(item.itemCost || item.amount || 0);
      }, 0);

      const pendingPayouts = (payoutsData || []).filter((item: any) => {
        const status = (item.status || item.payoutStatus || "").toLowerCase();
        return status === "pending" || status === "processing";
      }).length;

      const avgRating = (accommodations || []).reduce((sum: number, item: any) => {
        const ratingValue = Number(item.rating ?? 4.5);
        return sum + (Number.isFinite(ratingValue) ? ratingValue : 4.5);
      }, 0) / Math.max((accommodations || []).length, 1);

      setStats({
        totalProperties: accommodations?.length || 0,
        activeProperties,
        totalBookings: reservations?.length || 0,
        pendingBookings,
        totalOrders: ordersData?.length || 0,
        pendingOrders,
        totalRiders: riderList?.length || 0,
        totalRevenue,
        totalPayouts: pendingPayouts,
        rating: Number(avgRating.toFixed(1)),
      });
    } catch (err) {
      console.error("❌ Failed to fetch brand owner dashboard data:", err);
      setError("Failed to load your dashboard. Please refresh the page.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = async () => {
    await logout();
    router.push("/brand-owner/login");
  };

  const getStatusBadge = (status: string | undefined) => {
    const value = (status || "pending").toLowerCase();
    const map: Record<string, { label: string; className: string; icon: any }> = {
      confirmed: { label: "Confirmed", className: "bg-green-100 text-green-800", icon: CheckCircle },
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800", icon: Clock },
      processing: { label: "Processing", className: "bg-amber-100 text-amber-800", icon: Clock },
      in_transit: { label: "In Transit", className: "bg-blue-100 text-blue-800", icon: Truck },
      delivered: { label: "Delivered", className: "bg-green-100 text-green-800", icon: CheckCircle },
      completed: { label: "Completed", className: "bg-blue-100 text-blue-800", icon: CheckCircle },
      cancelled: { label: "Cancelled", className: "bg-red-100 text-red-800", icon: XCircle },
      rejected: { label: "Rejected", className: "bg-red-100 text-red-800", icon: XCircle },
      approved: { label: "Approved", className: "bg-emerald-100 text-emerald-800", icon: CheckCircle },
      assigned: { label: "Assigned", className: "bg-purple-100 text-purple-800", icon: Truck },
      awaiting_payment: { label: "Awaiting Payment", className: "bg-orange-100 text-orange-800", icon: Clock },
    };

    return map[value] || { label: status || "Pending", className: "bg-yellow-100 text-yellow-800", icon: Clock };
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center h-96 gap-4">
        <AlertCircle className="h-12 w-12 text-red-500" />
        <p className="text-red-600 text-center">{error}</p>
        <Button onClick={() => window.location.reload()} className="gap-2">
          <Loader2 className="h-4 w-4" />
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-sm text-gray-500 mt-1">
            Welcome back, {user?.firstName || user?.fullName || "Brand owner"}. Your operations are running smoothly.
          </p>
          {company && (
            <p className="mt-1 text-xs text-gray-500">
              Company: <span className="font-semibold text-gray-700">{company.companyName || company.name || "Your Company"}</span>
            </p>
          )}
        </div>

        <div className="flex flex-wrap gap-2">
          <Button onClick={() => router.push("/brand-owner/properties/add")} className="gap-2">
            <Plus className="h-4 w-4" />
            Add Property
          </Button>
          <Button variant="outline" onClick={() => router.push("/brand-owner/logistics/riders/add")} className="gap-2">
            <Users className="h-4 w-4" />
            Add Rider
          </Button>
          <Button variant="outline" onClick={handleLogout} className="gap-2 text-red-600 hover:text-red-700 hover:bg-red-50">
            Logout
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Properties</p>
                <p className="text-2xl font-bold text-gray-900">{stats.totalProperties}</p>
                <p className="mt-1 text-xs text-green-600">{stats.activeProperties} active</p>
              </div>
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                <Hotel className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Bookings</p>
                <p className="text-2xl font-bold text-gray-900">{stats.totalBookings}</p>
                <p className="mt-1 text-xs text-yellow-600">{stats.pendingBookings} pending</p>
              </div>
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-purple-50 text-purple-600">
                <Calendar className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Orders</p>
                <p className="text-2xl font-bold text-gray-900">{stats.totalOrders}</p>
                <p className="mt-1 text-xs text-orange-600">{stats.pendingOrders} pending</p>
              </div>
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-orange-50 text-orange-600">
                <Package className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Revenue</p>
                <p className="text-2xl font-bold text-gray-900">{formatCurrency(stats.totalRevenue)}</p>
                <p className="mt-1 text-xs text-emerald-600">{stats.totalRiders} riders online</p>
              </div>
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-green-50 text-green-600">
                <DollarSign className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-6 xl:grid-cols-[1.2fr_0.8fr]">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Company Overview</CardTitle>
            <Badge className="bg-emerald-100 text-emerald-700">Active</Badge>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-4 rounded-xl border border-gray-200 bg-gray-50 p-4">
              <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-blue-100 text-blue-700">
                <Building2 className="h-6 w-6" />
              </div>
              <div>
                <p className="font-semibold text-gray-900">{company?.companyName || company?.name || "Brand Owner Company"}</p>
                <p className="text-sm text-gray-500">{user?.email || user?.emailAddress || "No email provided"}</p>
              </div>
            </div>

            <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
              <div className="rounded-xl border bg-white p-3">
                <p className="text-xs text-gray-500">Riders</p>
                <p className="mt-1 text-xl font-bold text-gray-900">{stats.totalRiders}</p>
              </div>
              <div className="rounded-xl border bg-white p-3">
                <p className="text-xs text-gray-500">Payouts</p>
                <p className="mt-1 text-xl font-bold text-gray-900">{stats.totalPayouts}</p>
              </div>
              <div className="rounded-xl border bg-white p-3">
                <p className="text-xs text-gray-500">Rating</p>
                <p className="mt-1 text-xl font-bold text-gray-900">{stats.rating}/5</p>
              </div>
            </div>

            <div className="space-y-2 text-sm text-gray-600">
              <div className="flex items-center gap-2">
                <MapPin className="h-4 w-4 text-gray-400" />
                <span>{company?.address || company?.location || "Address not added yet"}</span>
              </div>
              <div className="flex items-center gap-2">
                <Users className="h-4 w-4 text-gray-400" />
                <span>{company?.companyType || "Logistics / Travel / Hospitality"}</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Live Updates</CardTitle>
            <Bell className="h-4 w-4 text-gray-500" />
          </CardHeader>
          <CardContent className="space-y-3">
            {notifications.length === 0 ? (
              <div className="rounded-xl border border-dashed border-gray-200 bg-gray-50 p-6 text-center text-sm text-gray-500">
                No notifications yet.
              </div>
            ) : (
              notifications.map((item) => {
                const status = getStatusBadge(item.status || item.type || "pending");
                const StatusIcon = status.icon;
                return (
                  <div key={item.id || item.title} className="rounded-xl border border-gray-200 bg-gray-50 p-3">
                    <div className="flex items-center justify-between gap-2">
                      <p className="font-medium text-gray-900">{item.title || item.subject || "Update"}</p>
                      <Badge className={`${status.className} flex items-center gap-1 px-2 py-1`}>
                        <StatusIcon className="h-3 w-3" />
                        {status.label}
                      </Badge>
                    </div>
                    <p className="mt-1 text-sm text-gray-600">{item.body || item.message || item.description || "New activity is available."}</p>
                  </div>
                );
              })
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="text-base">Recent Properties</CardTitle>
          <Link href="/brand-owner/properties" className="flex items-center gap-1 text-sm text-primary hover:underline">
            View All <ArrowRight className="h-3 w-3" />
          </Link>
        </CardHeader>
        <CardContent>
          {properties.length === 0 ? (
            <div className="py-8 text-center text-gray-500">
              <Hotel className="mx-auto mb-3 h-12 w-12 text-gray-300" />
              <p>No properties yet.</p>
              <Button variant="outline" className="mt-3" onClick={() => router.push("/brand-owner/properties/add")}>
                <Plus className="mr-2 h-4 w-4" />
                Add Property
              </Button>
            </div>
          ) : (
            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              {properties.slice(0, 3).map((property) => (
                <div key={property.id} className="overflow-hidden rounded-xl border border-gray-200 bg-white transition-shadow hover:shadow-md">
                  <div className="relative h-40 bg-gray-200">
                    {property.accomodationImages?.[0] ? (
                      <img src={property.accomodationImages[0]} alt={property.accomodationName} className="h-full w-full object-cover" />
                    ) : (
                      <div className="flex h-full items-center justify-center bg-gray-100">
                        <Hotel className="h-12 w-12 text-gray-400" />
                      </div>
                    )}
                    <Badge className={`absolute right-2 top-2 ${property.isBooked ? "bg-red-100 text-red-700" : "bg-green-100 text-green-700"}`}>
                      {property.isBooked ? "Booked" : "Available"}
                    </Badge>
                  </div>

                  <div className="p-4">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <h4 className="font-medium text-gray-900">{property.accomodationName || "Unnamed Property"}</h4>
                        <p className="text-xs text-gray-500">{property.accomodationType || "Property"}</p>
                      </div>
                      <div className="flex items-center gap-1 text-sm font-medium">
                        <Star className="h-3 w-3 fill-yellow-400 text-yellow-400" />
                        {property.rating || 4.5}
                      </div>
                    </div>

                    <div className="mt-2 flex items-center gap-2 text-sm text-gray-500">
                      <MapPin className="h-3 w-3" />
                      {property.location || "Location not set"}
                    </div>

                    <div className="mt-4 flex items-center justify-between">
                      <span className="text-lg font-bold text-primary">{formatCurrency(property.bookingAmount || 0)}</span>
                      <Link href={`/brand-owner/properties/${property.id}`}>
                        <Button size="sm" variant="ghost">
                          <Eye className="h-3 w-3" />
                        </Button>
                      </Link>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Recent Bookings</CardTitle>
            <Link href="/brand-owner/reservations" className="flex items-center gap-1 text-sm text-primary hover:underline">
              View All <ArrowRight className="h-3 w-3" />
            </Link>
          </CardHeader>
          <CardContent>
            {bookings.length === 0 ? (
              <div className="py-8 text-center text-gray-500">No bookings yet.</div>
            ) : (
              <div className="space-y-3">
                {bookings.slice(0, 5).map((booking) => {
                  const status = getStatusBadge(booking.bookingStatus || booking.status);
                  const StatusIcon = status.icon;

                  return (
                    <div key={booking.id} className="rounded-xl bg-gray-50 p-3">
                      <div className="flex items-start justify-between gap-3">
                        <div>
                          <p className="font-medium text-gray-900">{booking.guestName || booking.customerName || "Guest"}</p>
                          <p className="text-sm text-gray-500">{booking.accomodationName || "Property"}</p>
                        </div>
                        <Badge className={`${status.className} flex items-center gap-1`}>
                          <StatusIcon className="h-3 w-3" />
                          {status.label}
                        </Badge>
                      </div>

                      <div className="mt-2 flex items-center justify-between text-sm text-gray-500">
                        <span>
                          {booking.checkInDate ? new Date(booking.checkInDate).toLocaleDateString() : "N/A"}
                          {' '}to{' '}
                          {booking.checkOutDate ? new Date(booking.checkOutDate).toLocaleDateString() : "N/A"}
                        </span>
                        <span className="font-semibold text-gray-900">{formatCurrency(booking.totalAmount || 0)}</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="text-base">Recent Orders</CardTitle>
            <Link href="/brand-owner/orders" className="flex items-center gap-1 text-sm text-primary hover:underline">
              View All <ArrowRight className="h-3 w-3" />
            </Link>
          </CardHeader>
          <CardContent>
            {orders.length === 0 ? (
              <div className="py-8 text-center text-gray-500">No orders yet.</div>
            ) : (
              <div className="space-y-3">
                {orders.slice(0, 5).map((order) => {
                  const status = getStatusBadge(order.orderStatus || order.status);
                  const StatusIcon = status.icon;

                  return (
                    <div key={order.id} className="rounded-xl bg-gray-50 p-3">
                      <div className="flex items-start justify-between gap-3">
                        <div>
                          <p className="font-medium text-gray-900">{order.itemName || "Parcel Order"}</p>
                          <p className="text-sm text-gray-500">Tracking: {order.trackingNum || "N/A"}</p>
                        </div>
                        <Badge className={`${status.className} flex items-center gap-1`}>
                          <StatusIcon className="h-3 w-3" />
                          {status.label}
                        </Badge>
                      </div>

                      <div className="mt-2 flex items-center justify-between text-sm text-gray-500">
                        <span>{order.senderName || "Customer"}</span>
                        <span className="font-semibold text-gray-900">{formatCurrency(order.itemCost || order.amount || 0)}</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="text-base">Riders & Fleet</CardTitle>
          <Link href="/brand-owner/logistics/riders" className="flex items-center gap-1 text-sm text-primary hover:underline">
            Manage Riders <ArrowRight className="h-3 w-3" />
          </Link>
        </CardHeader>
        <CardContent>
          {riders.length === 0 ? (
            <div className="py-8 text-center text-gray-500">No riders added yet.</div>
          ) : (
            <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
              {riders.slice(0, 6).map((rider) => (
                <div key={rider.id} className="rounded-xl border border-gray-200 bg-white p-3">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-medium text-gray-900">{rider.fullName || rider.name || "Rider"}</p>
                      <p className="text-xs text-gray-500">{rider.riderType || rider.type || "Fleet rider"}</p>
                    </div>
                    <div className="flex h-10 w-10 items-center justify-center rounded-full bg-purple-100 text-purple-600">
                      <Truck className="h-4 w-4" />
                    </div>
                  </div>

                  <div className="mt-3 flex items-center justify-between text-sm text-gray-600">
                    <span>Status</span>
                    <Badge className={rider.isAvailable === false ? "bg-red-100 text-red-700" : "bg-green-100 text-green-700"}>
                      {rider.isAvailable === false ? "Off duty" : "Available"}
                    </Badge>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      <div className="grid grid-cols-2 gap-4 md:grid-cols-4">
        <Link href="/brand-owner/properties" className="rounded-xl border border-gray-200 bg-white p-4 text-center transition-all hover:shadow-md">
          <Hotel className="mx-auto mb-2 h-6 w-6 text-primary" />
          <p className="text-sm font-medium text-gray-900">Properties</p>
          <p className="text-xs text-gray-500">Manage listings</p>
        </Link>
        <Link href="/brand-owner/reservations" className="rounded-xl border border-gray-200 bg-white p-4 text-center transition-all hover:shadow-md">
          <Calendar className="mx-auto mb-2 h-6 w-6 text-primary" />
          <p className="text-sm font-medium text-gray-900">Bookings</p>
          <p className="text-xs text-gray-500">Track guests</p>
        </Link>
        <Link href="/brand-owner/orders" className="rounded-xl border border-gray-200 bg-white p-4 text-center transition-all hover:shadow-md">
          <Package className="mx-auto mb-2 h-6 w-6 text-primary" />
          <p className="text-sm font-medium text-gray-900">Orders</p>
          <p className="text-xs text-gray-500">Fulfillment</p>
        </Link>
        <Link href="/brand-owner/logistics/riders" className="rounded-xl border border-gray-200 bg-white p-4 text-center transition-all hover:shadow-md">
          <Truck className="mx-auto mb-2 h-6 w-6 text-primary" />
          <p className="text-sm font-medium text-gray-900">Riders</p>
          <p className="text-xs text-gray-500">Fleet overview</p>
        </Link>
      </div>
    </div>
  );
}
