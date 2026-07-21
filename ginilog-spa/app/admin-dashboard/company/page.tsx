"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Truck, Hotel, Users, LogOut, Bell, Search, User, Loader2, AlertCircle,
  Package, Building2, Megaphone, UserCheck, TrendingUp, LayoutDashboard,
  Menu, X, ChevronRight, Plus, Edit, Trash2, Eye, Filter,
  Download, RefreshCw, Settings, Home, BarChart3, MessageSquare,
  Shield, UserPlus, Phone, Mail, MapPin, Car, IdCard
} from "lucide-react";
import {
  getStoredUser, logout, adminGetProfile,
  getAllOrders, getAllCustomerReservations, getAllUsers,
  updateOrderStatus, updateCustomerReservation,
  getAllStaff, getAllAdverts,
} from "@/lib/api";

const ORDER_STATUSES = ["Open", "Accepted", "Picked", "Ongoing", "Completed", "Delivered", "Closed", "Cancelled", "Rejected"];
const BOOKING_STATUSES = ["Pending", "Confirmed", "Completed", "Cancelled"];

type Section = "dashboard" | "logistics" | "accommodation" | "staff" | "adverts" | "users" | "drivers" | "add-driver";

// Driver interface
interface Driver {
  id: string;
  firstName: string;
  surName: string;
  email: string;
  phoneNo: string;
  vehicleType: string;
  licenseNumber: string;
  status: "Available" | "On Delivery" | "Off Duty";
  rating: number;
  deliveries: number;
  joined: string;
  address: string;
  emergencyContact: string;
}

export default function CompanyDashboard() {
  const router = useRouter();
  const [activeSection, setActiveSection] = useState<Section>("dashboard");
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [adminProfile, setAdminProfile] = useState<any>(null);
  const [orders, setOrders] = useState<any[]>([]);
  const [reservations, setReservations] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);
  const [staff, setStaff] = useState<any[]>([]);
  const [adverts, setAdverts] = useState<any[]>([]);
  const [drivers, setDrivers] = useState<Driver[]>([
    {
      id: "1",
      firstName: "Mike",
      surName: "Johnson",
      email: "mike@company.com",
      phoneNo: "08012345678",
      vehicleType: "Motorcycle",
      licenseNumber: "DL-2024-001",
      status: "Available",
      rating: 4.7,
      deliveries: 145,
      joined: "2024-01-10",
      address: "123 Main St, Lagos",
      emergencyContact: "08012345679"
    },
    {
      id: "2",
      firstName: "Sara",
      surName: "Chen",
      email: "sara@company.com",
      phoneNo: "08087654321",
      vehicleType: "Van",
      licenseNumber: "DL-2024-002",
      status: "On Delivery",
      rating: 4.9,
      deliveries: 203,
      joined: "2023-11-15",
      address: "456 Oak Ave, Abuja",
      emergencyContact: "08087654322"
    },
    {
      id: "3",
      firstName: "Carlos",
      surName: "Mendez",
      email: "carlos@company.com",
      phoneNo: "08055555555",
      vehicleType: "Bicycle",
      licenseNumber: "DL-2024-003",
      status: "Off Duty",
      rating: 4.3,
      deliveries: 89,
      joined: "2024-02-01",
      address: "789 Pine Rd, Port Harcourt",
      emergencyContact: "08055555556"
    }
  ]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [showAddDriver, setShowAddDriver] = useState(false);
  const [editingDriver, setEditingDriver] = useState<Driver | null>(null);

  // Driver form state
  const [driverForm, setDriverForm] = useState({
    firstName: "",
    surName: "",
    email: "",
    phoneNo: "",
    vehicleType: "",
    licenseNumber: "",
    address: "",
    emergencyContact: "",
    status: "Available" as "Available" | "On Delivery" | "Off Duty"
  });

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
  const isLogistics = adminProfile?.businessType === "Logistics" || adminProfile?.businessType === "logistics";

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

  // Driver CRUD operations
  const handleAddDriver = () => {
    const newDriver: Driver = {
      id: Date.now().toString(),
      ...driverForm,
      rating: 0,
      deliveries: 0,
      joined: new Date().toISOString().split("T")[0]
    };
    setDrivers(prev => [...prev, newDriver]);
    setDriverForm({
      firstName: "",
      surName: "",
      email: "",
      phoneNo: "",
      vehicleType: "",
      licenseNumber: "",
      address: "",
      emergencyContact: "",
      status: "Available"
    });
    setShowAddDriver(false);
    setActiveSection("drivers");
  };

  const handleEditDriver = (driver: Driver) => {
    setEditingDriver(driver);
    setDriverForm({
      firstName: driver.firstName,
      surName: driver.surName,
      email: driver.email,
      phoneNo: driver.phoneNo,
      vehicleType: driver.vehicleType,
      licenseNumber: driver.licenseNumber,
      address: driver.address,
      emergencyContact: driver.emergencyContact,
      status: driver.status
    });
    setShowAddDriver(true);
  };

  const handleUpdateDriver = () => {
    if (!editingDriver) return;
    const updatedDrivers = drivers.map(d => 
      d.id === editingDriver.id ? { ...d, ...driverForm } : d
    );
    setDrivers(updatedDrivers);
    setEditingDriver(null);
    setDriverForm({
      firstName: "",
      surName: "",
      email: "",
      phoneNo: "",
      vehicleType: "",
      licenseNumber: "",
      address: "",
      emergencyContact: "",
      status: "Available"
    });
    setShowAddDriver(false);
    setActiveSection("drivers");
  };

  const handleDeleteDriver = (id: string) => {
    if (confirm("Are you sure you want to delete this driver?")) {
      setDrivers(prev => prev.filter(d => d.id !== id));
    }
  };

  const handleDriverStatusChange = (id: string, newStatus: "Available" | "On Delivery" | "Off Duty") => {
    setDrivers(prev => prev.map(d => 
      d.id === id ? { ...d, status: newStatus } : d
    ));
  };

  const getStatusBadge = (status: string) => {
    const s = (status || "").toLowerCase();
    if (["delivered", "confirmed", "completed", "closed"].includes(s)) return "bg-green-100 text-green-800";
    if (["in_transit", "processing", "ongoing", "picked", "accepted", "available"].includes(s)) return "bg-blue-100 text-blue-800";
    if (s === "cancelled" || s === "rejected" || s === "off duty") return "bg-gray-100 text-gray-600";
    if (s === "on delivery") return "bg-orange-100 text-orange-800";
    return "bg-yellow-100 text-yellow-800";
  };

  const getDriverStatusBadge = (status: string) => {
    const s = (status || "").toLowerCase();
    if (s === "available") return "bg-green-100 text-green-800";
    if (s === "on delivery") return "bg-orange-100 text-orange-800";
    if (s === "off duty") return "bg-gray-100 text-gray-600";
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

  const filteredDrivers = drivers.filter(
    (d) =>
      !search ||
      d.firstName.toLowerCase().includes(search.toLowerCase()) ||
      d.surName.toLowerCase().includes(search.toLowerCase()) ||
      d.email.toLowerCase().includes(search.toLowerCase()) ||
      d.vehicleType.toLowerCase().includes(search.toLowerCase()) ||
      d.licenseNumber.toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-100">
        <Loader2 className="h-8 w-8 animate-spin" style={{ color: "#8B0000" }} />
      </div>
    );
  }

  const navItems: { key: Section; label: string; icon: React.ElementType; managerOnly?: boolean }[] = [
    { key: "dashboard", label: "Dashboard", icon: LayoutDashboard },
    { key: "logistics", label: "Parcel Orders", icon: Truck },
    { key: "accommodation", label: "Bookings List", icon: Building2 },
    { key: "drivers", label: "Drivers/Riders", icon: Users, managerOnly: true },
    { key: "staff", label: "Staff List", icon: UserCheck, managerOnly: true },
    { key: "adverts", label: "Advert List", icon: Megaphone },
    { key: "users", label: "Users", icon: Users },
  ];

  const handleNav = (section: Section) => {
    setActiveSection(section);
    setSearch("");
    setSidebarOpen(false);
    if (section !== "add-driver") {
      setShowAddDriver(false);
      setEditingDriver(null);
    }
  };

  const handleLogout = () => {
    logout();
    router.push("/admin-dashboard/admin-login");
  };

  const SearchBar = () => (
    <div className="relative w-full sm:w-56 flex-shrink-0">
      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
      <input
        type="text"
        placeholder="Search..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 w-full"
        style={{ "--tw-ring-color": "#8B0000" } as React.CSSProperties}
      />
    </div>
  );

  return (
    <div className="min-h-screen flex bg-gray-100">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-50 w-64 flex flex-col transition-transform duration-300 lg:static lg:translate-x-0 ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        }`}
        style={{ backgroundColor: "#8B0000" }}
      >
        {/* Brand */}
        <div className="flex items-center justify-between px-5 py-4 border-b border-red-900">
          <div>
            <h1 className="text-white font-bold text-xl tracking-wide">GINILOG</h1>
            <p className="text-red-300 text-xs mt-0.5">Company Portal</p>
          </div>
          <button
            className="lg:hidden text-red-200 hover:text-white"
            onClick={() => setSidebarOpen(false)}
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* User Profile */}
        <div className="px-5 py-4 border-b border-red-900">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-full bg-red-900 flex items-center justify-center overflow-hidden flex-shrink-0">
              {adminProfile?.profilePicture ? (
                <img src={adminProfile.profilePicture} alt="Admin" className="h-full w-full object-cover" />
              ) : (
                <User className="h-5 w-5 text-red-300" />
              )}
            </div>
            <div className="min-w-0">
              <p className="text-white text-sm font-medium truncate">
                {adminProfile?.firstName || "Admin"} {adminProfile?.surName || adminProfile?.lastName || ""}
              </p>
              <p className="text-red-300 text-xs truncate">
                {adminProfile?.adminType || "Company Admin"}
              </p>
            </div>
          </div>
          {adminProfile?.companyName && (
            <p className="mt-2 text-red-200 text-xs font-medium truncate">{adminProfile.companyName}</p>
          )}
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
          {navItems
            .filter((item) => !item.managerOnly || isManager)
            .map((item) => (
              <button
                key={item.key}
                onClick={() => handleNav(item.key)}
                className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors ${
                  activeSection === item.key
                    ? "bg-white text-red-900"
                    : "text-red-100 hover:bg-red-900 hover:text-white"
                }`}
              >
                <item.icon className="h-5 w-5 flex-shrink-0" />
                <span className="flex-1 text-left">{item.label}</span>
                {activeSection === item.key && <ChevronRight className="h-4 w-4" />}
              </button>
            ))}
        </nav>

        {/* Footer */}
        <div className="px-3 py-4 border-t border-red-900 space-y-1">
          <button
            onClick={handleLogout}
            className="w-full flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium text-red-100 hover:bg-red-900 hover:text-white transition-colors"
          >
            <LogOut className="h-5 w-5" />
            Logout
          </button>
          <Link
            href="/admin-dashboard"
            className="flex items-center gap-2 px-4 py-2 rounded-lg text-xs text-red-300 hover:text-white transition-colors"
          >
            ← Back to Super Admin
          </Link>
        </div>
      </aside>

      {/* Main content */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Top bar */}
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
                  {navItems.find((n) => n.key === activeSection)?.label ?? "Dashboard"}
                </h2>
                {adminProfile?.companyName && (
                  <p className="text-xs text-gray-500">Hi, welcome back! {adminProfile.companyName}</p>
                )}
              </div>
            </div>
            <button className="p-2 hover:bg-gray-100 rounded-lg">
              <Bell className="h-5 w-5 text-gray-500" />
            </button>
          </div>
        </header>

        <main className="flex-1 p-5 overflow-auto">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
              <AlertCircle className="h-4 w-4 flex-shrink-0" />
              <span>{error}</span>
            </div>
          )}

          {/* ── DASHBOARD ── */}
          {activeSection === "dashboard" && (
            <div className="space-y-6">
              {/* Revenue stats */}
              <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center gap-2 mb-1">
                      <TrendingUp className="h-4 w-4 text-green-600" />
                      <p className="text-xs text-gray-500 uppercase font-medium">Total Revenue</p>
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
                      <Package className="h-4 w-4" style={{ color: "#8B0000" }} />
                      <p className="text-xs text-gray-500 uppercase font-medium">Completed</p>
                    </div>
                    <p className="text-xl font-bold text-gray-900">₦{completedRevenue.toLocaleString()}</p>
                    <p className="text-sm" style={{ color: "#8B0000" }}>{completedOrderList.length} packages</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center gap-2 mb-1">
                      <Truck className="h-4 w-4 text-blue-600" />
                      <p className="text-xs text-gray-500 uppercase font-medium">Pending Orders</p>
                    </div>
                    <p className="text-xl font-bold text-gray-900">₦{pendingRevenue.toLocaleString()}</p>
                    <p className="text-sm text-blue-600">{pendingOrderList.length} pending</p>
                  </CardContent>
                </Card>
              </div>

              {/* Quick-nav count cards */}
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <Card className="cursor-pointer hover:shadow-md transition-shadow" onClick={() => handleNav("logistics")}>
                  <CardContent className="p-5 flex items-center gap-4">
                    <div className="h-12 w-12 rounded-lg flex items-center justify-center bg-red-100" style={{ color: "#8B0000" }}>
                      <Truck className="h-6 w-6" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-gray-900">{orders.length}</p>
                      <p className="text-sm text-gray-500">Total Orders</p>
                    </div>
                  </CardContent>
                </Card>
                <Card className="cursor-pointer hover:shadow-md transition-shadow" onClick={() => handleNav("accommodation")}>
                  <CardContent className="p-5 flex items-center gap-4">
                    <div className="h-12 w-12 rounded-lg flex items-center justify-center bg-blue-100 text-blue-600">
                      <Building2 className="h-6 w-6" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-gray-900">{reservations.length}</p>
                      <p className="text-sm text-gray-500">Total Bookings</p>
                    </div>
                  </CardContent>
                </Card>
                <Card className="cursor-pointer hover:shadow-md transition-shadow" onClick={() => handleNav("users")}>
                  <CardContent className="p-5 flex items-center gap-4">
                    <div className="h-12 w-12 rounded-lg flex items-center justify-center bg-purple-100 text-purple-600">
                      <Users className="h-6 w-6" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-gray-900">{users.length}</p>
                      <p className="text-sm text-gray-500">Registered Users</p>
                    </div>
                  </CardContent>
                </Card>
              </div>

              {/* Recent Orders */}
              <Card>
                <CardHeader className="flex flex-row items-center justify-between pb-3">
                  <CardTitle>Recent Orders</CardTitle>
                  <button
                    onClick={() => handleNav("logistics")}
                    className="text-sm font-medium hover:underline"
                    style={{ color: "#8B0000" }}
                  >
                    View all →
                  </button>
                </CardHeader>
                <CardContent>
                  {orders.length === 0 ? (
                    <p className="text-gray-500 text-sm text-center py-6">No orders yet.</p>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Tracking Num</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Item Name</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Sender</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Receiver</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Status</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Date</th>
                          </tr>
                        </thead>
                        <tbody>
                          {orders.slice(0, 5).map((order) => (
                            <tr key={order.id} className="border-b hover:bg-gray-50">
                              <td className="py-2 px-4 font-mono text-xs">{order.trackingNum || order.id?.slice(0, 8)}</td>
                              <td className="py-2 px-4">{order.itemName}</td>
                              <td className="py-2 px-4">{order.senderName}</td>
                              <td className="py-2 px-4">{order.recieverName}</td>
                              <td className="py-2 px-4">
                                <Badge className={getStatusBadge(order.orderStatus)}>{order.orderStatus || "Open"}</Badge>
                              </td>
                              <td className="py-2 px-4 text-gray-500">
                                {order.createdAt ? new Date(order.createdAt).toLocaleDateString() : "—"}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Recent Bookings */}
              <Card>
                <CardHeader className="flex flex-row items-center justify-between pb-3">
                  <CardTitle>Recent Bookings</CardTitle>
                  <button
                    onClick={() => handleNav("accommodation")}
                    className="text-sm font-medium hover:underline"
                    style={{ color: "#8B0000" }}
                  >
                    View all →
                  </button>
                </CardHeader>
                <CardContent>
                  {reservations.length === 0 ? (
                    <p className="text-gray-500 text-sm text-center py-6">No bookings yet.</p>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Accommodation</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Guest</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Check In</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Check Out</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Total</th>
                            <th className="text-left py-2 px-4 font-medium text-gray-600">Status</th>
                          </tr>
                        </thead>
                        <tbody>
                          {reservations.slice(0, 5).map((res) => (
                            <tr key={res.id} className="border-b hover:bg-gray-50">
                              <td className="py-2 px-4">{res.accomodationName}</td>
                              <td className="py-2 px-4">{res.guestName || res.customerName || "—"}</td>
                              <td className="py-2 px-4">{res.checkInDate ? new Date(res.checkInDate).toLocaleDateString() : "—"}</td>
                              <td className="py-2 px-4">{res.checkOutDate ? new Date(res.checkOutDate).toLocaleDateString() : "—"}</td>
                              <td className="py-2 px-4 font-medium">₦{(res.totalAmount || res.totalCost || 0).toLocaleString()}</td>
                              <td className="py-2 px-4">
                                <Badge className={getStatusBadge(res.bookingStatus)}>{res.bookingStatus || "Pending"}</Badge>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </CardContent>
              </Card>
            </div>
          )}

          {/* ── PARCEL ORDERS ── */}
          {activeSection === "logistics" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Order List ({filteredOrders.length})</CardTitle>
                <SearchBar />
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
                              <Badge className={getStatusBadge(order.orderStatus)}>{order.orderStatus || "Open"}</Badge>
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
                                  className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-red-800"
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

          {/* ── BOOKINGS LIST ── */}
          {activeSection === "accommodation" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Customer Reservation List ({filteredReservations.length})</CardTitle>
                <SearchBar />
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
                              <Badge className={getStatusBadge(res.bookingStatus)}>{res.bookingStatus || "Pending"}</Badge>
                            </td>
                            <td className="py-3 px-4">
                              {updatingId === res.id ? (
                                <Loader2 className="h-4 w-4 animate-spin" />
                              ) : (
                                <select
                                  value={res.bookingStatus || "Pending"}
                                  onChange={(e) => handleBookingStatusChange(res.id, e.target.value)}
                                  className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-red-800"
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

          {/* ── DRIVERS / RIDERS MANAGEMENT ── */}
          {activeSection === "drivers" && (
            <>
              <Card>
                <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                  <CardTitle>Drivers / Riders ({filteredDrivers.length})</CardTitle>
                  <div className="flex flex-wrap gap-2">
                    <SearchBar />
                    <Button 
                      style={{ backgroundColor: "#8B0000" }} 
                      className="text-white gap-1"
                      onClick={() => {
                        setEditingDriver(null);
                        setDriverForm({
                          firstName: "",
                          surName: "",
                          email: "",
                          phoneNo: "",
                          vehicleType: "",
                          licenseNumber: "",
                          address: "",
                          emergencyContact: "",
                          status: "Available"
                        });
                        setShowAddDriver(true);
                        setActiveSection("add-driver");
                      }}
                    >
                      <UserPlus className="h-4 w-4" /> Add Driver
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  {/* Driver stats summary */}
                  <div className="grid grid-cols-3 gap-3 mb-4">
                    <div className="bg-green-50 p-3 rounded-lg border border-green-200">
                      <p className="text-sm text-green-600 font-medium">Available</p>
                      <p className="text-xl font-bold">{drivers.filter(d => d.status === "Available").length}</p>
                    </div>
                    <div className="bg-orange-50 p-3 rounded-lg border border-orange-200">
                      <p className="text-sm text-orange-600 font-medium">On Delivery</p>
                      <p className="text-xl font-bold">{drivers.filter(d => d.status === "On Delivery").length}</p>
                    </div>
                    <div className="bg-gray-50 p-3 rounded-lg border border-gray-200">
                      <p className="text-sm text-gray-600 font-medium">Off Duty</p>
                      <p className="text-xl font-bold">{drivers.filter(d => d.status === "Off Duty").length}</p>
                    </div>
                  </div>

                  {filteredDrivers.length === 0 ? (
                    <div className="text-center py-12">
                      <Users className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                      <p className="text-gray-500">No drivers found.</p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                      {filteredDrivers.map((driver) => (
                        <Card key={driver.id} className="hover:shadow-md transition-shadow">
                          <CardContent className="p-4">
                            <div className="flex items-start justify-between">
                              <div className="flex items-center gap-3">
                                <div className="h-12 w-12 rounded-full bg-red-100 flex items-center justify-center text-red-700 font-semibold text-lg">
                                  {driver.firstName[0]}{driver.surName[0]}
                                </div>
                                <div>
                                  <h4 className="font-semibold">{driver.firstName} {driver.surName}</h4>
                                  <p className="text-sm text-gray-500 flex items-center gap-1">
                                    <Car className="h-3 w-3" /> {driver.vehicleType}
                                  </p>
                                </div>
                              </div>
                              <Badge className={getDriverStatusBadge(driver.status)}>{driver.status}</Badge>
                            </div>
                            
                            <div className="mt-3 grid grid-cols-2 gap-2 text-sm">
                              <div>
                                <p className="text-gray-500">Email</p>
                                <p className="font-medium text-xs truncate">{driver.email}</p>
                              </div>
                              <div>
                                <p className="text-gray-500">Phone</p>
                                <p className="font-medium text-xs">{driver.phoneNo}</p>
                              </div>
                              <div>
                                <p className="text-gray-500">License</p>
                                <p className="font-medium text-xs">{driver.licenseNumber}</p>
                              </div>
                              <div>
                                <p className="text-gray-500">Deliveries</p>
                                <p className="font-medium">{driver.deliveries}</p>
                              </div>
                            </div>

                            {driver.rating > 0 && (
                              <div className="mt-2 flex items-center gap-1 text-sm">
                                <span className="text-gray-500">Rating:</span>
                                <span className="font-medium text-yellow-600">★ {driver.rating}</span>
                              </div>
                            )}

                            <div className="mt-3 flex gap-2 flex-wrap">
                              <Button 
                                variant="outline" 
                                size="sm" 
                                className="gap-1"
                                onClick={() => handleEditDriver(driver)}
                              >
                                <Edit className="h-3 w-3" /> Edit
                              </Button>
                              <Button 
                                variant="outline" 
                                size="sm" 
                                className="gap-1 text-red-600 hover:text-red-700"
                                onClick={() => handleDeleteDriver(driver.id)}
                              >
                                <Trash2 className="h-3 w-3" /> Delete
                              </Button>
                              <select
                                value={driver.status}
                                onChange={(e) => handleDriverStatusChange(driver.id, e.target.value as any)}
                                className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-red-800"
                              >
                                <option value="Available">Available</option>
                                <option value="On Delivery">On Delivery</option>
                                <option value="Off Duty">Off Duty</option>
                              </select>
                            </div>
                          </CardContent>
                        </Card>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </>
          )}

          {/* ── ADD / EDIT DRIVER FORM ── */}
          {activeSection === "add-driver" && showAddDriver && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>
                  {editingDriver ? "Edit Driver" : "Add New Driver/Rider"}
                </CardTitle>
                <Button 
                  variant="outline" 
                  onClick={() => {
                    setShowAddDriver(false);
                    setEditingDriver(null);
                    setActiveSection("drivers");
                  }}
                >
                  <X className="h-4 w-4 mr-1" /> Cancel
                </Button>
              </CardHeader>
              <CardContent>
                <form onSubmit={(e) => {
                  e.preventDefault();
                  if (editingDriver) {
                    handleUpdateDriver();
                  } else {
                    handleAddDriver();
                  }
                }}>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium mb-1">First Name *</label>
                      <input
                        type="text"
                        required
                        value={driverForm.firstName}
                        onChange={(e) => setDriverForm({...driverForm, firstName: e.target.value})}
                        className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                        placeholder="Enter first name"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Surname *</label>
                      <input
                        type="text"
                        required
                        value={driverForm.surName}
                        onChange={(e) => setDriverForm({...driverForm, surName: e.target.value})}
                        className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                        placeholder="Enter surname"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Email *</label>
                      <div className="relative">
                        <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <input
                          type="email"
                          required
                          value={driverForm.email}
                          onChange={(e) => setDriverForm({...driverForm, email: e.target.value})}
                          className="w-full pl-9 pr-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                          placeholder="driver@email.com"
                        />
                      </div>
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Phone Number *</label>
                      <div className="relative">
                        <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <input
                          type="tel"
                          required
                          value={driverForm.phoneNo}
                          onChange={(e) => setDriverForm({...driverForm, phoneNo: e.target.value})}
                          className="w-full pl-9 pr-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                          placeholder="08012345678"
                        />
                      </div>
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Vehicle Type *</label>
                      <select
                        required
                        value={driverForm.vehicleType}
                        onChange={(e) => setDriverForm({...driverForm, vehicleType: e.target.value})}
                        className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      >
                        <option value="">Select vehicle type</option>
                        <option value="Motorcycle">Motorcycle</option>
                        <option value="Bicycle">Bicycle</option>
                        <option value="Van">Van</option>
                        <option value="Car">Car</option>
                        <option value="Truck">Truck</option>
                      </select>
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">License Number *</label>
                      <div className="relative">
                        <IdCard className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <input
                          type="text"
                          required
                          value={driverForm.licenseNumber}
                          onChange={(e) => setDriverForm({...driverForm, licenseNumber: e.target.value})}
                          className="w-full pl-9 pr-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                          placeholder="DL-2024-001"
                        />
                      </div>
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Status</label>
                      <select
                        value={driverForm.status}
                        onChange={(e) => setDriverForm({...driverForm, status: e.target.value as any})}
                        className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      >
                        <option value="Available">Available</option>
                        <option value="On Delivery">On Delivery</option>
                        <option value="Off Duty">Off Duty</option>
                      </select>
                    </div>
                    <div className="md:col-span-2">
                      <label className="block text-sm font-medium mb-1">Address</label>
                      <div className="relative">
                        <MapPin className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
                        <input
                          type="text"
                          value={driverForm.address}
                          onChange={(e) => setDriverForm({...driverForm, address: e.target.value})}
                          className="w-full pl-9 pr-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                          placeholder="Enter driver address"
                        />
                      </div>
                    </div>
                    <div className="md:col-span-2">
                      <label className="block text-sm font-medium mb-1">Emergency Contact</label>
                      <input
                        type="text"
                        value={driverForm.emergencyContact}
                        onChange={(e) => setDriverForm({...driverForm, emergencyContact: e.target.value})}
                        className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                        placeholder="Emergency contact phone number"
                      />
                    </div>
                  </div>

                  <div className="mt-6 flex gap-3">
                    <Button 
                      type="submit"
                      style={{ backgroundColor: "#8B0000" }} 
                      className="text-white"
                    >
                      {editingDriver ? "Update Driver" : "Add Driver"}
                    </Button>
                    <Button 
                      type="button"
                      variant="outline"
                      onClick={() => {
                        setShowAddDriver(false);
                        setEditingDriver(null);
                        setActiveSection("drivers");
                      }}
                    >
                      Cancel
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>
          )}

          {/* ── STAFF LIST ── */}
          {activeSection === "staff" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Staff List ({filteredStaff.length})</CardTitle>
                <SearchBar />
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

          {/* ── ADVERT LIST ── */}
          {activeSection === "adverts" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Advert List ({filteredAdverts.length})</CardTitle>
                <SearchBar />
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

          {/* ── USERS ── */}
          {activeSection === "users" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Registered Users ({filteredUsers.length})</CardTitle>
                <SearchBar />
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
    </div>
  );
}