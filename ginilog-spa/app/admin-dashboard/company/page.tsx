"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Truck, Hotel, Users, LogOut, Bell, Search, User, Loader2, AlertCircle,
  Package, Building2, ArrowLeft, Megaphone, UserCheck, TrendingUp,
} from "lucide-react";
import {
  getStoredUser, logout, adminGetProfile,
  getAllOrders, getAllCustomerReservations, getAllUsers,
  updateOrderStatus, updateCustomerReservation,
  getAllStaff, getAllAdverts,
} from "@/lib/api";

const ORDER_STATUSES = ["Open", "Accepted", "Picked", "Ongoing", "Completed", "Delivered", "Closed", "Cancelled", "Rejected"];
const BOOKING_STATUSES = ["Pending", "Confirmed", "Completed", "Cancelled"];

type Tab = "logistics" | "accommodation" | "staff" | "adverts" | "users";

export default function CompanyDashboard() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState<Tab>("logistics");
  const [adminProfile, setAdminProfile] = useState<any>(null);
  const [orders, setOrders] = useState<any[]>([]);
  const [reservations, setReservations] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);
  const [staff, setStaff] = useState<any[]>([]);
  const [adverts, setAdverts] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [updatingId, setUpdatingId] = useState<string | null>(null);

  useEffect(() => {
    const stored = getStoredUser();
    if (!stored) {
      router.push("/admin-dashboard/admin-login");
      return;
    }
    const fetchData = async () => {
      try {
        const [profile, allOrders, allReservations, allUsers, allStaff, allAdverts] = await Promise.all([
          adminGetProfile().catch(() => null),
          getAllOrders().catch(() => []),
          getAllCustomerReservations().catch(() => []),
          getAllUsers().catch(() => []),
          getAllStaff().catch(() => []),
          getAllAdverts().catch(() => []),
        ]);
        setAdminProfile(profile);
        setOrders(allOrders || []);
        setReservations(allReservations || []);
        setUsers(allUsers || []);
        setStaff(allStaff || []);
        setAdverts(allAdverts || []);
      } catch (err) {
        setError("Failed to load company data.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [router]);

  const isManager = adminProfile?.adminType === "Manager";

  // Revenue statistics (mirrors C# OrderStatistics)
  const today = new Date().toISOString().split("T")[0];
  const totalRevenue = orders.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const todayOrderList = orders.filter((o) => o.createdAt && o.createdAt.startsWith(today));
  const todayRevenue = todayOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const completedOrderList = orders.filter((o) => ["Completed", "Delivered", "Closed"].includes(o.orderStatus || ""));
  const completedRevenue = completedOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const pendingOrderList = orders.filter((o) => ["Open", "Picked", "Ongoing", "Accepted"].includes(o.orderStatus || ""));
  const pendingRevenue = pendingOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);

  const handleOrderStatusChange = async (id: string, newStatus: string) => {
    setUpdatingId(id);
    try {
      await updateOrderStatus(id, { OrderStatus: newStatus });
      setOrders((prev) => prev.map((o) => o.id === id ? { ...o, orderStatus: newStatus } : o));
    } catch (err) {
      console.error(err);
    } finally {
      setUpdatingId(null);
    }
  };

  const handleBookingStatusChange = async (id: string, newStatus: string) => {
    setUpdatingId(id);
    try {
      await updateCustomerReservation(id, { BookingStatus: newStatus });
      setReservations((prev) => prev.map((r) => r.id === id ? { ...r, bookingStatus: newStatus } : r));
    } catch (err) {
      console.error(err);
    } finally {
      setUpdatingId(null);
    }
  };

  const getStatusBadge = (status: string) => {
    const s = (status || "").toLowerCase();
    if (["delivered", "confirmed", "completed", "closed"].includes(s)) return "bg-green-100 text-green-800";
    if (["in_transit", "processing", "ongoing", "picked", "accepted"].includes(s)) return "bg-blue-100 text-blue-800";
    if (s === "cancelled" || s === "rejected") return "bg-red-100 text-red-800";
    return "bg-yellow-100 text-yellow-800";
  };

  const filteredOrders = orders.filter(
    (o) =>
      !search ||
      (o.trackingNum || "").toLowerCase().includes(search.toLowerCase()) ||
      (o.itemName || "").toLowerCase().includes(search.toLowerCase()) ||
      (o.senderName || "").toLowerCase().includes(search.toLowerCase()) ||
      (o.recieverName || "").toLowerCase().includes(search.toLowerCase())
  );

  const filteredReservations = reservations.filter(
    (r) =>
      !search ||
      (r.bookingRefNo || "").toLowerCase().includes(search.toLowerCase()) ||
      (r.accomodationName || "").toLowerCase().includes(search.toLowerCase()) ||
      (r.guestName || "").toLowerCase().includes(search.toLowerCase()) ||
      (r.customerName || "").toLowerCase().includes(search.toLowerCase())
  );

  const filteredStaff = staff.filter(
    (s) =>
      !search ||
      (s.firstName || "").toLowerCase().includes(search.toLowerCase()) ||
      (s.surName || "").toLowerCase().includes(search.toLowerCase()) ||
      (s.email || "").toLowerCase().includes(search.toLowerCase()) ||
      (s.companyName || "").toLowerCase().includes(search.toLowerCase())
  );

  const filteredAdverts = adverts.filter(
    (a) =>
      !search ||
      (a.advertName || "").toLowerCase().includes(search.toLowerCase()) ||
      (a.advertType || "").toLowerCase().includes(search.toLowerCase()) ||
      (a.advertItemDescription || "").toLowerCase().includes(search.toLowerCase())
  );

  const filteredUsers = users.filter(
    (u) =>
      !search ||
      (u.firstName || "").toLowerCase().includes(search.toLowerCase()) ||
      (u.lastName || "").toLowerCase().includes(search.toLowerCase()) ||
      (u.email || "").toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  const tabs: { key: Tab; label: string; icon: any; managerOnly?: boolean }[] = [
    { key: "logistics", label: "Parcel Orders", icon: Truck },
    { key: "accommodation", label: "Bookings List", icon: Building2 },
    { key: "staff", label: "Staff List", icon: UserCheck, managerOnly: true },
    { key: "adverts", label: "Advert List", icon: Megaphone },
    { key: "users", label: "Users", icon: Users },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-gray-900 text-white sticky top-0 z-40">
        <div className="flex items-center justify-between px-6 py-4">
          <div className="flex items-center gap-4">
            <Link
              href="/admin-dashboard"
              className="flex items-center gap-2 text-gray-400 hover:text-white transition-colors"
            >
              <ArrowLeft className="h-5 w-5" />
              <span className="text-sm hidden sm:block">Back to Super Admin</span>
            </Link>
            <div className="w-px h-6 bg-gray-700" />
            <div>
              <h1 className="text-lg font-bold">Company Portal</h1>
              {adminProfile?.companyName && (
                <p className="text-xs text-gray-400">Hi, welcome back! {adminProfile.companyName}</p>
              )}
            </div>
          </div>
          <div className="flex items-center gap-3">
            <button className="p-2 hover:bg-gray-800 rounded-lg">
              <Bell className="h-5 w-5 text-gray-400" />
            </button>
            <div className="flex items-center gap-2">
              <div className="h-9 w-9 rounded-full bg-primary/20 flex items-center justify-center overflow-hidden">
                {adminProfile?.profilePicture ? (
                  <img src={adminProfile.profilePicture} alt="Admin" className="h-full w-full object-cover" />
                ) : (
                  <User className="h-4 w-4 text-primary" />
                )}
              </div>
              <div className="hidden md:block">
                <p className="text-sm font-medium">
                  {adminProfile?.firstName || "Admin"} {adminProfile?.surName || adminProfile?.lastName || ""}
                </p>
                <p className="text-xs text-gray-400">{adminProfile?.adminType || "Company Admin"}</p>
              </div>
            </div>
            <Button
              variant="ghost"
              size="sm"
              className="text-gray-400 hover:text-white hover:bg-gray-800"
              onClick={() => { logout(); router.push("/admin-dashboard/admin-login"); }}
            >
              <LogOut className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </header>

      <main className="p-6 max-w-7xl mx-auto">

        {/* Revenue Statistics Cards (mirrors C# Manager stats) */}
        {orders.length > 0 && (
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-4">
            <Card>
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-1">
                  <TrendingUp className="h-4 w-4 text-green-600" />
                  <p className="text-xs text-gray-500 uppercase font-medium">Revenue</p>
                </div>
                <p className="text-xl font-bold text-gray-900">₦{totalRevenue.toLocaleString()}</p>
                <p className="text-sm text-green-600">{orders.length} orders</p>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-1">
                  <TrendingUp className="h-4 w-4 text-green-600" />
                  <p className="text-xs text-gray-500 uppercase font-medium">Today's Revenue</p>
                </div>
                <p className="text-xl font-bold text-gray-900">₦{todayRevenue.toLocaleString()}</p>
                <p className="text-sm text-green-600">{todayOrderList.length} today</p>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-1">
                  <Package className="h-4 w-4 text-red-500" />
                  <p className="text-xs text-gray-500 uppercase font-medium">Completed</p>
                </div>
                <p className="text-xl font-bold text-gray-900">₦{completedRevenue.toLocaleString()}</p>
                <p className="text-sm text-red-500">{completedOrderList.length} packages</p>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-1">
                  <Truck className="h-4 w-4 text-green-600" />
                  <p className="text-xs text-gray-500 uppercase font-medium">Pending Orders</p>
                </div>
                <p className="text-xl font-bold text-gray-900">₦{pendingRevenue.toLocaleString()}</p>
                <p className="text-sm text-green-600">{pendingOrderList.length} pending</p>
              </CardContent>
            </Card>
          </div>
        )}

        {/* Summary Count Cards */}
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
          <Card
            className="cursor-pointer hover:shadow-md transition-shadow"
            onClick={() => { setActiveTab("logistics"); setSearch(""); }}
          >
            <CardContent className="p-5 flex items-center gap-4">
              <div className={`h-12 w-12 rounded-lg flex items-center justify-center ${activeTab === "logistics" ? "bg-primary text-white" : "bg-primary/10 text-primary"}`}>
                <Truck className="h-6 w-6" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{orders.length}</p>
                <p className="text-sm text-gray-500">Total Orders</p>
              </div>
            </CardContent>
          </Card>
          <Card
            className="cursor-pointer hover:shadow-md transition-shadow"
            onClick={() => { setActiveTab("accommodation"); setSearch(""); }}
          >
            <CardContent className="p-5 flex items-center gap-4">
              <div className={`h-12 w-12 rounded-lg flex items-center justify-center ${activeTab === "accommodation" ? "bg-blue-600 text-white" : "bg-blue-100 text-blue-600"}`}>
                <Building2 className="h-6 w-6" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{reservations.length}</p>
                <p className="text-sm text-gray-500">Total Bookings</p>
              </div>
            </CardContent>
          </Card>
          <Card
            className="cursor-pointer hover:shadow-md transition-shadow"
            onClick={() => { setActiveTab("users"); setSearch(""); }}
          >
            <CardContent className="p-5 flex items-center gap-4">
              <div className={`h-12 w-12 rounded-lg flex items-center justify-center ${activeTab === "users" ? "bg-purple-600 text-white" : "bg-purple-100 text-purple-600"}`}>
                <Users className="h-6 w-6" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{users.length}</p>
                <p className="text-sm text-gray-500">Registered Users</p>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Tab Navigation + Search */}
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 mb-4">
          <div className="flex border-b border-gray-200 overflow-x-auto">
            {tabs
              .filter((t) => !t.managerOnly || isManager)
              .map((tab) => (
                <button
                  key={tab.key}
                  onClick={() => { setActiveTab(tab.key); setSearch(""); }}
                  className={`flex items-center gap-2 px-4 py-3 text-sm font-medium border-b-2 whitespace-nowrap transition-colors ${
                    activeTab === tab.key
                      ? "border-primary text-primary"
                      : "border-transparent text-gray-500 hover:text-gray-700"
                  }`}
                >
                  <tab.icon className="h-4 w-4" />
                  {tab.label}
                </button>
              ))}
          </div>
          <div className="relative w-full sm:w-56 flex-shrink-0">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              type="text"
              placeholder="Search..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
            />
          </div>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
            <AlertCircle className="h-4 w-4" />
            <span>{error}</span>
          </div>
        )}

        {/* Parcel Orders Tab */}
        {activeTab === "logistics" && (
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Order List ({filteredOrders.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {filteredOrders.length === 0 ? (
                <div className="text-center py-12">
                  <Package className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-500">No orders found.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b bg-gray-50">
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Tracking Num</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Item Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Sender Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Receiver Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Shipping Cost</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Item Cost</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Order Status</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Update Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredOrders.map((order) => (
                        <tr key={order.id} className="border-b hover:bg-gray-50">
                          <td className="py-3 px-4 font-mono text-xs">{order.trackingNum || order.id?.slice(0, 8)}</td>
                          <td className="py-3 px-4">{order.itemName}</td>
                          <td className="py-3 px-4">{order.senderName}</td>
                          <td className="py-3 px-4">{order.recieverName}</td>
                          <td className="py-3 px-4">₦{(order.shippingCost || 0).toLocaleString()}</td>
                          <td className="py-3 px-4">₦{(order.itemCost || 0).toLocaleString()}</td>
                          <td className="py-3 px-4">
                            <Badge className={getStatusBadge(order.orderStatus)}>
                              {order.orderStatus || "Open"}
                            </Badge>
                          </td>
                          <td className="py-3 px-4 text-gray-500">
                            {order.createdAt ? new Date(order.createdAt).toLocaleDateString() : "—"}
                          </td>
                          <td className="py-3 px-4">
                            {updatingId === order.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <select
                                value={order.orderStatus || "Open"}
                                onChange={(e) => handleOrderStatusChange(order.id, e.target.value)}
                                className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                              >
                                {ORDER_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                              </select>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}

        {/* Bookings List Tab */}
        {activeTab === "accommodation" && (
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Customer Reservation List ({filteredReservations.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {filteredReservations.length === 0 ? (
                <div className="text-center py-12">
                  <Hotel className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-500">No bookings found.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b bg-gray-50">
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Accommodation Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Customer Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Ticket Num</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Reservation Start Date</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Reservation End Date</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Total Cost</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">No Of Days</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Room Number</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Update Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredReservations.map((res) => (
                        <tr key={res.id} className="border-b hover:bg-gray-50">
                          <td className="py-3 px-4">{res.accomodationName}</td>
                          <td className="py-3 px-4">{res.guestName || res.customerName || "—"}</td>
                          <td className="py-3 px-4 font-mono text-xs">{res.bookingRefNo || res.ticketNum || res.id?.slice(0, 8)}</td>
                          <td className="py-3 px-4">{res.checkInDate ? new Date(res.checkInDate).toLocaleDateString() : res.reservationStartDate || "—"}</td>
                          <td className="py-3 px-4">{res.checkOutDate ? new Date(res.checkOutDate).toLocaleDateString() : res.reservationEndDate || "—"}</td>
                          <td className="py-3 px-4 font-medium text-primary">₦{(res.totalAmount || res.totalCost || 0).toLocaleString()}</td>
                          <td className="py-3 px-4">{res.numberOfNights || res.noOfDays || "—"}</td>
                          <td className="py-3 px-4">
                            <Badge className="bg-gray-100 text-gray-700">{res.roomNumber || "—"}</Badge>
                          </td>
                          <td className="py-3 px-4 text-gray-500">
                            {res.createdAt ? new Date(res.createdAt).toLocaleDateString() : "—"}
                          </td>
                          <td className="py-3 px-4">
                            <Badge className={getStatusBadge(res.bookingStatus)}>
                              {res.bookingStatus || "Pending"}
                            </Badge>
                          </td>
                          <td className="py-3 px-4">
                            {updatingId === res.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <select
                                value={res.bookingStatus || "Pending"}
                                onChange={(e) => handleBookingStatusChange(res.id, e.target.value)}
                                className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                              >
                                {BOOKING_STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                              </select>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}

        {/* Staff List Tab (Manager only) */}
        {activeTab === "staff" && (
          <Card>
            <CardHeader>
              <CardTitle>Staff List ({filteredStaff.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {filteredStaff.length === 0 ? (
                <div className="text-center py-12">
                  <UserCheck className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-500">No staff found.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b bg-gray-50">
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Email</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Phone</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Role</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Company</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Branch</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Joined</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredStaff.map((s) => (
                        <tr key={s.id} className="border-b hover:bg-gray-50">
                          <td className="py-3 px-4 font-medium">{s.firstName} {s.surName}</td>
                          <td className="py-3 px-4 text-gray-500">{s.email}</td>
                          <td className="py-3 px-4">{s.phoneNo || "—"}</td>
                          <td className="py-3 px-4">
                            <Badge className="bg-blue-100 text-blue-800">{s.adminType || "—"}</Badge>
                          </td>
                          <td className="py-3 px-4">{s.companyName || "—"}</td>
                          <td className="py-3 px-4">{s.branch || "—"}</td>
                          <td className="py-3 px-4 text-gray-500">
                            {s.createdAt ? new Date(s.createdAt).toLocaleDateString() : "—"}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}

        {/* Advert List Tab */}
        {activeTab === "adverts" && (
          <Card>
            <CardHeader>
              <CardTitle>Advert List ({filteredAdverts.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {filteredAdverts.length === 0 ? (
                <div className="text-center py-12">
                  <Megaphone className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-500">No adverts found.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b bg-gray-50">
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Advert Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Type</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Description</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Days</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Trans Ref</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredAdverts.map((a) => (
                        <tr key={a.id} className="border-b hover:bg-gray-50">
                          <td className="py-3 px-4 font-medium">{a.advertName}</td>
                          <td className="py-3 px-4">
                            <Badge className="bg-purple-100 text-purple-800">{a.advertType || "—"}</Badge>
                          </td>
                          <td className="py-3 px-4 text-gray-500 max-w-xs truncate">{a.advertItemDescription || "—"}</td>
                          <td className="py-3 px-4">{a.advertDays4 || "—"}</td>
                          <td className="py-3 px-4 font-mono text-xs">{a.transRef || "—"}</td>
                          <td className="py-3 px-4 text-gray-500">
                            {a.createdAt ? new Date(a.createdAt).toLocaleDateString() : "—"}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}

        {/* Users Tab */}
        {activeTab === "users" && (
          <Card>
            <CardHeader>
              <CardTitle>Registered Users ({filteredUsers.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {filteredUsers.length === 0 ? (
                <div className="text-center py-12">
                  <Users className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-500">No users found.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b bg-gray-50">
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Name</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Email</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Phone</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">State</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Joined</th>
                        <th className="text-left py-3 px-4 font-medium text-gray-600">Last Seen</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredUsers.map((u) => (
                        <tr key={u.id} className="border-b hover:bg-gray-50">
                          <td className="py-3 px-4 font-medium">{u.firstName} {u.lastName}</td>
                          <td className="py-3 px-4 text-gray-500">{u.email}</td>
                          <td className="py-3 px-4">{u.phoneNo || "—"}</td>
                          <td className="py-3 px-4">{u.state || "—"}</td>
                          <td className="py-3 px-4">
                            <Badge className={u.userStatus ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-600"}>
                              {u.userStatus ? "Active" : "Inactive"}
                            </Badge>
                          </td>
                          <td className="py-3 px-4 text-gray-500">
                            {u.createdAt ? new Date(u.createdAt).toLocaleDateString() : "—"}
                          </td>
                          <td className="py-3 px-4 text-gray-500">
                            {u.lastSeenAt ? new Date(u.lastSeenAt).toLocaleDateString() : "—"}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        )}
      </main>
    </div>
  );
}
