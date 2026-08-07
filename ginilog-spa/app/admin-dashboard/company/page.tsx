// app/admin-dashboard/company/page.tsx

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
  Menu, X, ChevronRight, Plus, Edit, Trash2, Eye, Calendar, Star,
  MapPin, Phone, Mail, Clock, DollarSign, MessageSquare, FileText,
  Download, Filter, ChevronDown, CheckCircle, XCircle, HelpCircle,
  Settings, CreditCard, Home, Car, Bike, Navigation, Award,
} from "lucide-react";
import {
  getStoredUser,
  logout,
  adminGetProfile,
  getAllOrders,
  getAllCustomerReservations,
  getAllUsers,
  updateOrderStatus,
  updateCustomerReservation,
  getAllStaff,
  getAllAdverts,
  getAllDrivers,
  getAvailableDrivers,
  getDriverStats,
  getCompanyStats,
  addRider,
  deleteRider,
  updateRider,
  assignOrderToRider,
  getPackageOrders,
  trackPackageOrder,
  getAccommodations,
  addAccommodation,
  updateAccommodation,
  deleteAccommodation,
  getAccommodationReservations,
  addAccommodationReservation,
  updateAccommodationReservation,
  deleteAccommodationReservation,
  getNotifications,
  getFeedback,
  sendFeedback,
  getPayouts,
  getPayoutStatistics,
  getReservationDates,
  uploadImage,
  getCompanyDrivers,  
} from "@/lib/api";

const ORDER_STATUSES = ["Open", "Accepted", "Picked", "Ongoing", "Completed", "Delivered", "Closed", "Cancelled", "Rejected"];
const BOOKING_STATUSES = ["Pending", "Confirmed", "Completed", "Cancelled"];
const DRIVER_STATUSES = ["Available", "On Delivery", "Off Duty"];

type Section = 
  | "dashboard" 
  | "logistics" 
  | "logistics-drivers" 
  | "logistics-assign" 
  | "logistics-track" 
  | "accommodation" 
  | "accommodation-properties" 
  | "accommodation-bookings" 
  | "staff" 
  | "adverts" 
  | "users"
  | "reports"
  | "notifications"
  | "chat";

interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  vehicleType?: string;
  status: "Available" | "On Delivery" | "Off Duty";
  rating: number;
  deliveries?: number;
  available: boolean;
  licenseNumber?: string;
  emergencyContact?: string;
  joined?: string;
}

interface Property {
  id: string;
  name: string;
  type: string;
  location: string;
  price: number;
  rating: number;
  images: string[];
  amenities: string[];
  available: boolean;
  description: string;
}

interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  message: string;
  timestamp: string;
  isRead: boolean;
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
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [availableDrivers, setAvailableDrivers] = useState<Driver[]>([]);
  const [driverStats, setDriverStats] = useState<any>(null);
  const [companyStats, setCompanyStats] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [showAddDriverModal, setShowAddDriverModal] = useState(false);
  const [showAddPropertyModal, setShowAddPropertyModal] = useState(false);
  const [propertyForm, setPropertyForm] = useState({
    roomNumber: "",
    maximumNoOfGuest: "",
    roomPrice: "",
    roomType: "",
    roomImages: "https://thumbs.dreamstime.com/b/hotel-room-27254386.jpg?w=768, https://thumbs.dreamstime.com/b/luxury-hotel-room-18929948.jpg?w=768",
    roomFeatures: "Desk, Television, Wi-Fi, Refrigerator",
    isBooked: false,
  });
  const [propertyFormError, setPropertyFormError] = useState<string | null>(null);
  const [propertyFormSuccess, setPropertyFormSuccess] = useState<string | null>(null);
  const [isSubmittingProperty, setIsSubmittingProperty] = useState(false);
  const [showChatModal, setShowChatModal] = useState(false);
  const [selectedChat, setSelectedChat] = useState<any>(null);
  const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState("");
  const [activeTab, setActiveTab] = useState<string>("overview");
  const [notifications, setNotifications] = useState<any[]>([]);
  const [feedback, setFeedback] = useState<any[]>([]);
  const [payouts, setPayouts] = useState<any[]>([]);
  const [payoutStats, setPayoutStats] = useState<any>(null);
  const [reservationDates, setReservationDates] = useState<any[]>([]);

  const isManager = adminProfile?.adminType === "Manager";
  const userType = adminProfile?.userType || adminProfile?.adminType || "Admin";

  useEffect(() => {
    const stored = getStoredUser();
    if (!stored) {
      router.push("/admin-dashboard/admin-login");
      return;
    }
    const fetchData = async () => {
      try {
        const [
          profile,
          allOrders,
          allReservations,
          allUsers,
          allStaff,
          allAdverts,
          allDrivers,
          availableDriversData,
          driverStatsData,
          companyStatsData,
          notificationsData,
          feedbackData,
          payoutsData,
          payoutStatsData,
          reservationDatesData,
        ] = await Promise.all([
          adminGetProfile().catch(() => null),
          getAllOrders().catch(() => []),
          getAllCustomerReservations().catch(() => []),
          getAllUsers().catch(() => []),
          getAllStaff().catch(() => []),
          getAllAdverts().catch(() => []),
          getAllDrivers().catch(() => []),
          getAvailableDrivers().catch(() => []),
          getDriverStats().catch(() => null),
          getCompanyStats().catch(() => null),
          getNotifications().catch(() => []),
          getFeedback().catch(() => []),
          getPayouts().catch(() => []),
          getPayoutStatistics().catch(() => null),
          getReservationDates().catch(() => []),
        ]);
        setAdminProfile(profile);
        setOrders(allOrders || []);
        setReservations(allReservations || []);
        setUsers(allUsers || []);
        setStaff(allStaff || []);
        setAdverts(allAdverts || []);
        setDrivers(allDrivers || []);
        setAvailableDrivers(availableDriversData || []);
        setDriverStats(driverStatsData);
        setCompanyStats(companyStatsData);
        setNotifications(notificationsData || []);
        setFeedback(feedbackData || []);
        setPayouts(payoutsData || []);
        setPayoutStats(payoutStatsData);
        setReservationDates(reservationDatesData || []);
      } catch (err) {
        setError("Failed to load company data.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [router]);

  const today = new Date().toISOString().split("T")[0];
  const totalRevenue = orders.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const todayOrderList = orders.filter((o) => o.createdAt && o.createdAt.startsWith(today));
  const todayRevenue = todayOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const completedOrderList = orders.filter((o) => ["Completed", "Delivered", "Closed"].includes(o.orderStatus || ""));
  const completedRevenue = completedOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);
  const pendingOrderList = orders.filter((o) => ["Open", "Picked", "Ongoing", "Accepted"].includes(o.orderStatus || ""));
  const pendingRevenue = pendingOrderList.reduce((s, o) => s + (o.shippingCost || 0), 0);

  // Accommodation stats
  const pendingBookings = reservations.filter((r) => r.bookingStatus === "Pending").length;
  const activeBookings = reservations.filter((r) => r.bookingStatus === "Confirmed").length;
  const completedBookings = reservations.filter((r) => r.bookingStatus === "Completed").length;
  const totalBookingRevenue = reservations.reduce((s, r) => s + (r.totalAmount || 0), 0);

  // Logistics stats
  const activeDeliveries = orders.filter((o) => ["Accepted", "Picked", "Ongoing"].includes(o.orderStatus || ""));
  const totalDrivers = drivers.length;
  const availableDriversCount = drivers.filter((d) => d.status === "Available").length;
  const onDeliveryDrivers = drivers.filter((d) => d.status === "On Delivery").length;

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

  const handleAddProperty = async (e: React.FormEvent) => {
    e.preventDefault();
    setPropertyFormError(null);
    setPropertyFormSuccess(null);

    const roomNumber = Number(propertyForm.roomNumber);
    const maximumNoOfGuest = Number(propertyForm.maximumNoOfGuest);
    const roomPrice = Number(propertyForm.roomPrice);
    const roomType = propertyForm.roomType.trim();
    const roomImages = propertyForm.roomImages
      .split(/\s*,\s*/)
      .filter(Boolean);
    const roomFeatures = propertyForm.roomFeatures
      .split(/\s*,\s*/)
      .filter(Boolean);
    const isBooked = propertyForm.isBooked;

    if (!roomNumber || !maximumNoOfGuest || !roomPrice || !roomType) {
      setPropertyFormError("Please fill in room number, guest count, price, and room type.");
      return;
    }

    setIsSubmittingProperty(true);
    try {
      await addAccommodationReservation({
        roomNumber,
        maximumNoOfGuest,
        roomPrice,
        roomType,
        roomImages,
        roomFeatures,
        isBooked,
      });

      setPropertyFormSuccess("Property reservation payload submitted successfully.");
      setPropertyForm({
        roomNumber: "",
        maximumNoOfGuest: "",
        roomPrice: "",
        roomType: "",
        roomImages: "",
        roomFeatures: "",
        isBooked: false,
      });
      setShowAddPropertyModal(false);
    } catch (err) {
      setPropertyFormError(err instanceof Error ? err.message : "Failed to submit property data.");
    } finally {
      setIsSubmittingProperty(false);
    }
  };

  const handleAddDriver = async (e: React.FormEvent) => {
    e.preventDefault();
    const form = e.target as HTMLFormElement;
    const formData = new FormData(form);
    try {
      const driverData = {
        firstName: formData.get("firstName") as string,
        lastName: formData.get("lastName") as string,
        email: formData.get("email") as string,
        password: formData.get("password") as string || "Password123!",
        phoneNumber: formData.get("phoneNumber") as string,
        vehicleType: formData.get("vehicleType") as string,
        licenseNumber: formData.get("licenseNumber") as string,
        status: "Available" as const,
        companyId: adminProfile?.companyId || undefined,
      };
      await addRider(driverData);
      setShowAddDriverModal(false);
      // Refresh drivers
      const allDrivers = await getAllDrivers();
      setDrivers(allDrivers);
      const available = await getAvailableDrivers();
      setAvailableDrivers(available);
      const stats = await getDriverStats();
      setDriverStats(stats);
    } catch (err) {
      console.error("Failed to add driver:", err);
      setError("Failed to add driver. Please try again.");
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

  const filteredDrivers = drivers.filter(
    (d) =>
      !search ||
      (d.firstName || "").toLowerCase().includes(search.toLowerCase()) ||
      (d.lastName || "").toLowerCase().includes(search.toLowerCase()) ||
      (d.email || "").toLowerCase().includes(search.toLowerCase()) ||
      (d.vehicleType || "").toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-100">
        <Loader2 className="h-8 w-8 animate-spin" style={{ color: "#8B0000" }} />
      </div>
    );
  }

  // Navigation items with sub-items
  const navItems: { key: Section; label: string; icon: React.ElementType; managerOnly?: boolean; subItems?: { key: Section; label: string }[] }[] = [
    { key: "dashboard", label: "Dashboard", icon: LayoutDashboard },
    { 
      key: "logistics", 
      label: "Logistics", 
      icon: Truck,
      subItems: [
        { key: "logistics", label: "Orders" },
        { key: "logistics-drivers", label: "Drivers" },
        { key: "logistics-assign", label: "Assign Vehicles" },
        { key: "logistics-track", label: "Track Deliveries" },
      ]
    },
    { 
      key: "accommodation", 
      label: "Accommodation", 
      icon: Building2,
      subItems: [
        { key: "accommodation", label: "Bookings" },
        { key: "accommodation-properties", label: "Properties" },
      ]
    },
    { key: "staff", label: "Staff", icon: UserCheck, managerOnly: true },
    { key: "adverts", label: "Adverts", icon: Megaphone },
    { key: "users", label: "Users", icon: Users },
    { key: "reports", label: "Reports", icon: FileText },
    { key: "notifications", label: "Notifications", icon: Bell },
    { key: "chat", label: "Chat", icon: MessageSquare },
  ];

  const handleNav = (section: Section) => {
    setActiveSection(section);
    setSearch("");
    setSidebarOpen(false);
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

  // Sample properties data (will be replaced with real API data)
  const properties: Property[] = [
    {
      id: "1",
      name: "Luxury Villa",
      type: "Villa",
      location: "Lagos, Nigeria",
      price: 150000,
      rating: 4.8,
      images: ["/villa1.jpg"],
      amenities: ["WiFi", "Pool", "Gym", "Parking"],
      available: true,
      description: "A beautiful luxury villa with ocean view"
    },
    {
      id: "2",
      name: "City Apartment",
      type: "Apartment",
      location: "Abuja, Nigeria",
      price: 85000,
      rating: 4.5,
      images: ["/apt1.jpg"],
      amenities: ["WiFi", "Parking", "Security"],
      available: true,
      description: "Modern apartment in the heart of the city"
    },
  ];

  // Sample chat messages
  const sampleChats = [
    { id: "1", customerName: "John Doe", lastMessage: "When is my booking confirmed?", timestamp: "2 min ago", unread: 2 },
    { id: "2", customerName: "Jane Smith", lastMessage: "Thanks for the quick response!", timestamp: "1 hour ago", unread: 0 },
  ];

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
              <div key={item.key}>
                <button
                  onClick={() => {
                    if (item.subItems) {
                      handleNav(item.subItems[0].key);
                    } else {
                      handleNav(item.key);
                    }
                  }}
                  className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors ${
                    activeSection === item.key || item.subItems?.some((s) => s.key === activeSection)
                      ? "bg-white text-red-900"
                      : "text-red-100 hover:bg-red-900 hover:text-white"
                  }`}
                >
                  <item.icon className="h-5 w-5 flex-shrink-0" />
                  <span className="flex-1 text-left">{item.label}</span>
                  {item.subItems && (
                    <ChevronRight className={`h-4 w-4 transition-transform ${
                      item.subItems.some((s) => s.key === activeSection) ? "rotate-90" : ""
                    }`} />
                  )}
                </button>
                {/* Sub-items */}
                {item.subItems && (
                  <div className="ml-4 space-y-1 mt-1">
                    {item.subItems.map((sub) => (
                      <button
                        key={sub.key}
                        onClick={() => handleNav(sub.key)}
                        className={`w-full flex items-center gap-3 px-4 py-2 rounded-lg text-sm transition-colors ${
                          activeSection === sub.key
                            ? "bg-white/20 text-white"
                            : "text-red-300 hover:bg-red-900 hover:text-white"
                        }`}
                      >
                        <span className="text-left">{sub.label}</span>
                      </button>
                    ))}
                  </div>
                )}
              </div>
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
            <div className="flex items-center gap-2">
              <button className="p-2 hover:bg-gray-100 rounded-lg">
                <Bell className="h-5 w-5 text-gray-500" />
              </button>
            </div>
          </div>
        </header>

        <main className="flex-1 p-5 overflow-auto">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
              <AlertCircle className="h-4 w-4 flex-shrink-0" />
              <span>{error}</span>
              <button onClick={() => setError(null)} className="ml-auto text-red-400 hover:text-red-600">
                <XCircle className="h-4 w-4" />
              </button>
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

              {/* Role-specific Quick Stats */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Logistics Stats */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg flex items-center gap-2">
                      <Truck className="h-5 w-5 text-blue-600" />
                      Logistics Overview
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-3 gap-4">
                      <div className="text-center">
                        <p className="text-2xl font-bold">{activeDeliveries.length}</p>
                        <p className="text-xs text-gray-500">Active Deliveries</p>
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold text-green-600">{availableDriversCount}</p>
                        <p className="text-xs text-gray-500">Available Drivers</p>
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold text-blue-600">{onDeliveryDrivers}</p>
                        <p className="text-xs text-gray-500">On Delivery</p>
                      </div>
                    </div>
                    <div className="mt-3 flex gap-2">
                      <Button size="sm" variant="outline" onClick={() => handleNav("logistics")}>
                        View Orders
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => handleNav("logistics-drivers")}>
                        Manage Drivers
                      </Button>
                    </div>
                  </CardContent>
                </Card>

                {/* Accommodation Stats */}
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg flex items-center gap-2">
                      <Building2 className="h-5 w-5 text-purple-600" />
                      Accommodation Overview
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-3 gap-4">
                      <div className="text-center">
                        <p className="text-2xl font-bold text-yellow-600">{pendingBookings}</p>
                        <p className="text-xs text-gray-500">Pending Bookings</p>
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold text-blue-600">{activeBookings}</p>
                        <p className="text-xs text-gray-500">Active Bookings</p>
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold text-green-600">₦{totalBookingRevenue.toLocaleString()}</p>
                        <p className="text-xs text-gray-500">Total Revenue</p>
                      </div>
                    </div>
                    <div className="mt-3 flex gap-2">
                      <Button size="sm" variant="outline" onClick={() => handleNav("accommodation")}>
                        View Bookings
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => handleNav("accommodation-properties")}>
                        Manage Properties
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              </div>

              {/* Quick-nav count cards */}
              <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
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
                <Card className="cursor-pointer hover:shadow-md transition-shadow" onClick={() => handleNav("logistics-drivers")}>
                  <CardContent className="p-5 flex items-center gap-4">
                    <div className="h-12 w-12 rounded-lg flex items-center justify-center bg-green-100 text-green-600">
                      <UserCheck className="h-6 w-6" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-gray-900">{totalDrivers}</p>
                      <p className="text-sm text-gray-500">Total Drivers</p>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </div>
          )}

          {/* ── LOGISTICS - ORDERS ── */}
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
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Sender</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Receiver</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Shipping Cost</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Update</th>
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

          {/* ── LOGISTICS - DRIVERS ── */}
          {activeSection === "logistics-drivers" && (
            <div className="space-y-6">
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">Driver Management</h3>
                  <p className="text-sm text-gray-500">Manage drivers and riders</p>
                </div>
                <Button 
                  className="gap-2" 
                  style={{ backgroundColor: "#8B0000" }}
                  onClick={() => setShowAddDriverModal(true)}
                >
                  <Plus className="h-4 w-4" />
                  Add Driver
                </Button>
              </div>

              {/* Driver Stats */}
              <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Total Drivers</p>
                    <p className="text-2xl font-bold">{totalDrivers}</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Available</p>
                    <p className="text-2xl font-bold text-green-600">{availableDriversCount}</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">On Delivery</p>
                    <p className="text-2xl font-bold text-blue-600">{onDeliveryDrivers}</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Off Duty</p>
                    <p className="text-2xl font-bold text-gray-600">
                      {totalDrivers - availableDriversCount - onDeliveryDrivers}
                    </p>
                  </CardContent>
                </Card>
              </div>

              {/* Drivers Table */}
              <Card>
                <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                  <CardTitle>All Drivers ({filteredDrivers.length})</CardTitle>
                  <SearchBar />
                </CardHeader>
                <CardContent>
                  {filteredDrivers.length === 0 ? (
                    <div className="text-center py-12">
                      <UserCheck className="h-12 w-12 text-gray-200 mx-auto mb-3" />
                      <p className="text-gray-500">No drivers found.</p>
                      <Button 
                        variant="outline" 
                        className="mt-3"
                        onClick={() => setShowAddDriverModal(true)}
                      >
                        <Plus className="h-4 w-4 mr-2" />
                        Add First Driver
                      </Button>
                    </div>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Driver</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Vehicle</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">License</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Rating</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Deliveries</th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">Actions</th>
                          </tr>
                        </thead>
                        <tbody>
                          {filteredDrivers.map((driver) => (
                            <tr key={driver.id} className="border-b hover:bg-gray-50">
                              <td className="py-3 px-4">
                                <div>
                                  <p className="font-medium">{driver.firstName} {driver.lastName}</p>
                                  <p className="text-xs text-gray-500">{driver.email}</p>
                                </div>
                              </td>
                              <td className="py-3 px-4">
                                <div className="flex items-center gap-1">
                                  {driver.vehicleType?.toLowerCase().includes("bike") ? (
                                    <Bike className="h-4 w-4 text-gray-500" />
                                  ) : (
                                    <Car className="h-4 w-4 text-gray-500" />
                                  )}
                                  {driver.vehicleType || "N/A"}
                                </div>
                              </td>
                              <td className="py-3 px-4 font-mono text-xs">{driver.licenseNumber || "—"}</td>
                              <td className="py-3 px-4">
                                <Badge className={getStatusBadge(driver.status)}>{driver.status || "Off Duty"}</Badge>
                              </td>
                              <td className="py-3 px-4">
                                <div className="flex items-center gap-1">
                                  <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />
                                  {driver.rating || 0}
                                </div>
                              </td>
                              <td className="py-3 px-4">{driver.deliveries || 0}</td>
                              <td className="py-3 px-4">
                                <div className="flex gap-2">
                                  <Button size="sm" variant="outline" className="h-7 px-2">
                                    <Edit className="h-3 w-3" />
                                  </Button>
                                  <Button 
                                    size="sm" 
                                    variant="outline" 
                                    className="h-7 px-2 text-red-500 border-red-200 hover:bg-red-50"
                                    onClick={async () => {
                                      if (confirm("Are you sure you want to delete this driver?")) {
                                        await deleteRider(driver.id);
                                        const allDrivers = await getAllDrivers();
                                        setDrivers(allDrivers);
                                      }
                                    }}
                                  >
                                    <Trash2 className="h-3 w-3" />
                                  </Button>
                                </div>
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

          {/* ── LOGISTICS - ASSIGN VEHICLES ── */}
          {activeSection === "logistics-assign" && (
            <Card>
              <CardHeader>
                <CardTitle>Assign Vehicles to Drivers</CardTitle>
                <p className="text-sm text-gray-500">Assign vehicles to available drivers</p>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="border rounded-lg p-4">
                    <h4 className="font-medium mb-3">Available Drivers</h4>
                    {availableDrivers.length === 0 ? (
                      <p className="text-gray-500 text-sm">No available drivers</p>
                    ) : (
                      <div className="space-y-2">
                        {availableDrivers.map((driver) => (
                          <div key={driver.id} className="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                            <div>
                              <p className="font-medium">{driver.firstName} {driver.lastName}</p>
                              <p className="text-xs text-gray-500">{driver.vehicleType || "No vehicle assigned"}</p>
                            </div>
                            <Button 
                              size="sm" 
                              className="bg-green-600 hover:bg-green-700 text-white"
                              onClick={async () => {
                                // This would open a vehicle assignment modal
                                alert("Vehicle assignment feature coming soon!");
                              }}
                            >
                              Assign Vehicle
                            </Button>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                  <div className="border rounded-lg p-4">
                    <h4 className="font-medium mb-3">Available Vehicles</h4>
                    <div className="space-y-2">
                      <div className="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                        <div>
                          <p className="font-medium">Toyota Hiace</p>
                          <p className="text-xs text-gray-500">License: ABC-123</p>
                        </div>
                        <Badge className="bg-green-100 text-green-800">Available</Badge>
                      </div>
                      <div className="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                        <div>
                          <p className="font-medium">Honda Motorcycle</p>
                          <p className="text-xs text-gray-500">License: XYZ-789</p>
                        </div>
                        <Badge className="bg-green-100 text-green-800">Available</Badge>
                      </div>
                      <div className="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                        <div>
                          <p className="font-medium">Mercedes Sprinter</p>
                          <p className="text-xs text-gray-500">License: DEF-456</p>
                        </div>
                        <Badge className="bg-yellow-100 text-yellow-800">In Service</Badge>
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* ── LOGISTICS - TRACK DELIVERIES ── */}
          {activeSection === "logistics-track" && (
            <Card>
              <CardHeader>
                <CardTitle>Active Deliveries Tracking</CardTitle>
                <p className="text-sm text-gray-500">Monitor active deliveries in real-time</p>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {/* Map placeholder */}
                  <div className="bg-gray-100 rounded-lg p-8 text-center min-h-[300px] flex flex-col items-center justify-center">
                    <Navigation className="h-16 w-16 text-gray-400 mb-4" />
                    <p className="text-gray-500 font-medium">Live Map View</p>
                    <p className="text-sm text-gray-400">Track delivery locations in real-time</p>
                    {activeDeliveries.length > 0 ? (
                      <div className="mt-4 flex flex-wrap gap-2 justify-center">
                        {activeDeliveries.slice(0, 5).map((order) => (
                          <Badge key={order.id} className="bg-blue-100 text-blue-800">
                            <MapPin className="h-3 w-3 mr-1" />
                            {order.trackingNum || order.id.slice(0, 8)}
                          </Badge>
                        ))}
                        {activeDeliveries.length > 5 && (
                          <Badge className="bg-gray-200 text-gray-800">
                            +{activeDeliveries.length - 5} more
                          </Badge>
                        )}
                      </div>
                    ) : (
                      <p className="text-sm text-gray-400 mt-2">No active deliveries</p>
                    )}
                  </div>

                  {/* Active deliveries list */}
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b bg-gray-50">
                          <th className="text-left py-2 px-3 font-medium text-gray-600">Order</th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">Driver</th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">Status</th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">Location</th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">ETA</th>
                        </tr>
                      </thead>
                      <tbody>
                        {activeDeliveries.slice(0, 10).map((order) => (
                          <tr key={order.id} className="border-b hover:bg-gray-50">
                            <td className="py-2 px-3 font-mono text-xs">{order.trackingNum || order.id.slice(0, 8)}</td>
                            <td className="py-2 px-3">{order.riderName || "Unassigned"}</td>
                            <td className="py-2 px-3">
                              <Badge className={getStatusBadge(order.orderStatus)}>{order.orderStatus}</Badge>
                            </td>
                            <td className="py-2 px-3 text-gray-500">{order.currentLocation || "—"}</td>
                            <td className="py-2 px-3 text-gray-500">~30 min</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* ── ACCOMMODATION - BOOKINGS ── */}
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
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Accommodation</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Guest</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Check In</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Check Out</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Amount</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Update</th>
                        </tr>
                      </thead>
                      <tbody>
                        {filteredReservations.map((res) => (
                          <tr key={res.id} className="border-b hover:bg-gray-50">
                            <td className="py-3 px-4">{res.accomodationName}</td>
                            <td className="py-3 px-4">{res.guestName || res.customerName || "—"}</td>
                            <td className="py-3 px-4">{res.checkInDate ? new Date(res.checkInDate).toLocaleDateString() : "—"}</td>
                            <td className="py-3 px-4">{res.checkOutDate ? new Date(res.checkOutDate).toLocaleDateString() : "—"}</td>
                            <td className="py-3 px-4 font-medium">₦{(res.totalAmount || 0).toLocaleString()}</td>
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

          {/* ── ACCOMMODATION - PROPERTIES ── */}
          {activeSection === "accommodation-properties" && (
            <div className="space-y-6">
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">Manage Properties</h3>
                  <p className="text-sm text-gray-500">Add, edit, and manage your properties</p>
                </div>
                <Button 
                  className="gap-2" 
                  style={{ backgroundColor: "#8B0000" }}
                  onClick={() => setShowAddPropertyModal(true)}
                >
                  <Plus className="h-4 w-4" />
                  Add Property
                </Button>
              </div>

              {/* Properties Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {properties.map((property) => (
                  <Card key={property.id} className="hover:shadow-md transition-shadow">
                    <div className="relative h-48 bg-gray-200 rounded-t-lg overflow-hidden">
                      <div className="absolute inset-0 flex items-center justify-center text-gray-400">
                        <Building2 className="h-12 w-12" />
                      </div>
                      {property.available ? (
                        <Badge className="absolute top-2 right-2 bg-green-100 text-green-800">Available</Badge>
                      ) : (
                        <Badge className="absolute top-2 right-2 bg-red-100 text-red-800">Booked</Badge>
                      )}
                    </div>
                    <CardContent className="p-4">
                      <div className="flex items-start justify-between">
                        <div>
                          <h4 className="font-semibold text-gray-900">{property.name}</h4>
                          <p className="text-sm text-gray-500">{property.type}</p>
                        </div>
                        <div className="flex items-center gap-1">
                          <Star className="h-4 w-4 text-yellow-500 fill-yellow-500" />
                          <span className="text-sm font-medium">{property.rating}</span>
                        </div>
                      </div>
                      <p className="text-sm text-gray-500 mt-2">{property.location}</p>
                      <p className="text-lg font-bold" style={{ color: "#8B0000" }}>
                        ₦{property.price.toLocaleString()}
                        <span className="text-sm font-normal text-gray-500">/night</span>
                      </p>
                      <div className="mt-2 flex flex-wrap gap-1">
                        {property.amenities.slice(0, 3).map((amenity, idx) => (
                          <Badge key={idx} className="bg-gray-100 text-gray-600 text-xs">{amenity}</Badge>
                        ))}
                        {property.amenities.length > 3 && (
                          <Badge className="bg-gray-100 text-gray-600 text-xs">+{property.amenities.length - 3}</Badge>
                        )}
                      </div>
                      <div className="mt-3 flex gap-2">
                        <Button size="sm" variant="outline" className="flex-1">
                          <Eye className="h-3 w-3 mr-1" /> View
                        </Button>
                        <Button size="sm" variant="outline" className="flex-1">
                          <Edit className="h-3 w-3 mr-1" /> Edit
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </div>
          )}

          {/* ── STAFF ── */}
          {activeSection === "staff" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Staff List ({staff.length})</CardTitle>
                <SearchBar />
              </CardHeader>
              <CardContent>
                {staff.length === 0 ? (
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
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Joined</th>
                        </tr>
                      </thead>
                      <tbody>
                        {staff.map((s) => (
                          <tr key={s.id} className="border-b hover:bg-gray-50">
                            <td className="py-3 px-4 font-medium">{s.firstName} {s.surName}</td>
                            <td className="py-3 px-4 text-gray-500">{s.email}</td>
                            <td className="py-3 px-4">{s.phoneNo || "—"}</td>
                            <td className="py-3 px-4">
                              <Badge className="bg-blue-100 text-blue-800">{s.adminType || "—"}</Badge>
                            </td>
                            <td className="py-3 px-4">{s.companyName || "—"}</td>
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

          {/* ── ADVERTS ── */}
          {activeSection === "adverts" && (
            <Card>
              <CardHeader className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
                <CardTitle>Advert List ({adverts.length})</CardTitle>
                <SearchBar />
              </CardHeader>
              <CardContent>
                {adverts.length === 0 ? (
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
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                        </tr>
                      </thead>
                      <tbody>
                        {adverts.map((a) => (
                          <tr key={a.id} className="border-b hover:bg-gray-50">
                            <td className="py-3 px-4 font-medium">{a.advertName}</td>
                            <td className="py-3 px-4">
                              <Badge className="bg-purple-100 text-purple-800">{a.advertType || "—"}</Badge>
                            </td>
                            <td className="py-3 px-4 text-gray-500 max-w-xs truncate">{a.advertItemDescription || "—"}</td>
                            <td className="py-3 px-4">{a.advertDays4 || "—"}</td>
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
                <CardTitle>Registered Users ({users.length})</CardTitle>
                <SearchBar />
              </CardHeader>
              <CardContent>
                {users.length === 0 ? (
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
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                          <th className="text-left py-3 px-4 font-medium text-gray-600">Joined</th>
                        </tr>
                      </thead>
                      <tbody>
                        {users.map((u) => (
                          <tr key={u.id} className="border-b hover:bg-gray-50">
                            <td className="py-3 px-4 font-medium">{u.firstName} {u.lastName}</td>
                            <td className="py-3 px-4 text-gray-500">{u.email}</td>
                            <td className="py-3 px-4">{u.phoneNo || "—"}</td>
                            <td className="py-3 px-4">
                              <Badge className={u.userStatus ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-600"}>
                                {u.userStatus ? "Active" : "Inactive"}
                              </Badge>
                            </td>
                            <td className="py-3 px-4 text-gray-500">
                              {u.createdAt ? new Date(u.createdAt).toLocaleDateString() : "—"}
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

          {/* ── REPORTS ── */}
          {activeSection === "reports" && (
            <div className="space-y-6">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">Performance Reports</h3>
                <p className="text-sm text-gray-500">Daily, weekly, and monthly performance analytics</p>
              </div>

              {/* Report filters */}
              <div className="flex flex-wrap gap-3">
                <select className="px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-800">
                  <option>Daily</option>
                  <option>Weekly</option>
                  <option>Monthly</option>
                </select>
                <select className="px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-800">
                  <option>This Week</option>
                  <option>Last Week</option>
                  <option>This Month</option>
                  <option>Last Month</option>
                </select>
                <Button variant="outline" className="gap-2">
                  <Download className="h-4 w-4" />
                  Export Report
                </Button>
              </div>

              {/* Report cards */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Total Orders</p>
                    <p className="text-2xl font-bold">{orders.length}</p>
                    <p className="text-xs text-green-600">↑ 12% from last week</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Revenue</p>
                    <p className="text-2xl font-bold text-green-600">₦{totalRevenue.toLocaleString()}</p>
                    <p className="text-xs text-green-600">↑ 8% from last week</p>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <p className="text-sm text-gray-500">Bookings</p>
                    <p className="text-2xl font-bold">{reservations.length}</p>
                    <p className="text-xs text-green-600">↑ 5% from last week</p>
                  </CardContent>
                </Card>
              </div>

              {/* Revenue breakdown */}
              <Card>
                <CardHeader>
                  <CardTitle>Revenue Breakdown</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div>
                      <div className="flex justify-between text-sm">
                        <span>Logistics Revenue</span>
                        <span>₦{totalRevenue.toLocaleString()}</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-2 mt-1">
                        <div className="h-2 rounded-full" style={{ width: "60%", backgroundColor: "#8B0000" }}></div>
                      </div>
                    </div>
                    <div>
                      <div className="flex justify-between text-sm">
                        <span>Accommodation Revenue</span>
                        <span>₦{totalBookingRevenue.toLocaleString()}</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-2 mt-1">
                        <div className="h-2 rounded-full bg-blue-600" style={{ width: "40%" }}></div>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          )}

          {/* ── NOTIFICATIONS ── */}
          {activeSection === "notifications" && (
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>Notifications</CardTitle>
                <Badge className="bg-red-100 text-red-800">{notifications.filter((n) => !n.isRead).length} new</Badge>
              </CardHeader>
              <CardContent>
                {notifications.length === 0 ? (
                  <div className="text-center py-8">
                    <Bell className="h-12 w-12 text-gray-300 mx-auto mb-3" />
                    <p className="text-gray-500">No notifications</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {notifications.slice(0, 10).map((notif) => (
                      <div key={notif.id} className={`flex items-start gap-3 p-3 rounded-lg ${!notif.isRead ? "bg-blue-50" : "bg-gray-50"}`}>
                        <div className={`p-2 rounded-full ${notif.notificationType === "booking" ? "bg-blue-100 text-blue-600" :
                          notif.notificationType === "payment" ? "bg-green-100 text-green-600" :
                          notif.notificationType === "feedback" ? "bg-yellow-100 text-yellow-600" :
                          "bg-purple-100 text-purple-600"
                        }`}>
                          {notif.notificationType === "booking" && <Hotel className="h-4 w-4" />}
                          {notif.notificationType === "payment" && <CreditCard className="h-4 w-4" />}
                          {notif.notificationType === "feedback" && <Star className="h-4 w-4" />}
                          {notif.notificationType === "delivery" && <Truck className="h-4 w-4" />}
                          {notif.notificationType === "issue" && <AlertCircle className="h-4 w-4" />}
                        </div>
                        <div className="flex-1">
                          <p className="text-sm font-medium">{notif.title}</p>
                          <p className="text-sm text-gray-500">{notif.body}</p>
                          <p className="text-xs text-gray-400 mt-1">{new Date(notif.createdAt).toLocaleString()}</p>
                        </div>
                        {!notif.isRead && (
                          <button className="text-xs text-red-600 hover:underline">Mark read</button>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>
          )}

          {/* ── CHAT ── */}
          {activeSection === "chat" && (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 h-[600px]">
              {/* Chat list */}
              <Card className="md:col-span-1 overflow-hidden">
                <CardHeader className="border-b">
                  <CardTitle className="text-lg">Conversations</CardTitle>
                </CardHeader>
                <CardContent className="p-0 overflow-y-auto h-[calc(600px-70px)]">
                  {sampleChats.map((chat) => (
                    <div
                      key={chat.id}
                      className={`p-4 border-b hover:bg-gray-50 cursor-pointer transition-colors ${
                        selectedChat?.id === chat.id ? "bg-gray-50" : ""
                      }`}
                      onClick={() => setSelectedChat(chat)}
                    >
                      <div className="flex items-center gap-3">
                        <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                          <User className="h-5 w-5 text-primary" />
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="font-medium text-sm">{chat.customerName}</p>
                          <p className="text-xs text-gray-500 truncate">{chat.lastMessage}</p>
                        </div>
                        <div className="text-right flex-shrink-0">
                          <p className="text-xs text-gray-400">{chat.timestamp}</p>
                          {chat.unread > 0 && (
                            <Badge className="bg-red-600 text-white text-xs px-2 py-0.5">{chat.unread}</Badge>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </CardContent>
              </Card>

              {/* Chat messages */}
              <Card className="md:col-span-2 flex flex-col">
                {selectedChat ? (
                  <>
                    <CardHeader className="border-b">
                      <div className="flex items-center gap-3">
                        <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                          <User className="h-5 w-5 text-primary" />
                        </div>
                        <div>
                          <CardTitle className="text-base">{selectedChat.customerName}</CardTitle>
                          <p className="text-xs text-gray-500">Online</p>
                        </div>
                      </div>
                    </CardHeader>
                    <CardContent className="flex-1 overflow-y-auto p-4 space-y-3">
                      <div className="flex justify-start">
                        <div className="max-w-[70%] bg-gray-100 rounded-lg p-3">
                          <p className="text-sm">Hi, I'd like to know about my booking status.</p>
                          <p className="text-xs text-gray-400 mt-1">10:30 AM</p>
                        </div>
                      </div>
                      <div className="flex justify-end">
                        <div className="max-w-[70%] bg-red-800 text-white rounded-lg p-3">
                          <p className="text-sm">Hello! I'll check that for you right away.</p>
                          <p className="text-xs text-red-200 mt-1">10:32 AM</p>
                        </div>
                      </div>
                      <div className="flex justify-start">
                        <div className="max-w-[70%] bg-gray-100 rounded-lg p-3">
                          <p className="text-sm">Thank you! Please let me know as soon as possible.</p>
                          <p className="text-xs text-gray-400 mt-1">10:35 AM</p>
                        </div>
                      </div>
                    </CardContent>
                    <div className="p-3 border-t flex gap-2">
                      <input
                        type="text"
                        placeholder="Type a message..."
                        className="flex-1 px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-800"
                      />
                      <Button className="gap-2" style={{ backgroundColor: "#8B0000" }}>
                        <MessageSquare className="h-4 w-4" />
                        Send
                      </Button>
                    </div>
                  </>
                ) : (
                  <div className="flex-1 flex items-center justify-center text-center p-8">
                    <div>
                      <MessageSquare className="h-16 w-16 text-gray-300 mx-auto mb-4" />
                      <p className="text-gray-500 font-medium">Select a conversation</p>
                      <p className="text-sm text-gray-400">Choose a customer to start chatting</p>
                    </div>
                  </div>
                )}
              </Card>
            </div>
          )}
        </main>
      </div>

      {/* ── MODALS ── */}

      {/* Add Driver Modal */}
      {showAddDriverModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold">Add New Driver</h3>
              <button onClick={() => setShowAddDriverModal(false)} className="p-2 hover:bg-gray-100 rounded-lg">
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleAddDriver}>
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium text-gray-700">First Name *</label>
                    <input type="text" name="firstName" required className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="John" />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-gray-700">Last Name *</label>
                    <input type="text" name="lastName" required className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="Doe" />
                  </div>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Email *</label>
                  <input type="email" name="email" required className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="driver@email.com" />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Password *</label>
                  <input type="password" name="password" required className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="Password123!" />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Phone *</label>
                  <input type="tel" name="phoneNumber" required className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="08012345678" />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Vehicle Type</label>
                  <select name="vehicleType" className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800">
                    <option value="Motorcycle">Motorcycle</option>
                    <option value="Van">Van</option>
                    <option value="Truck">Truck</option>
                    <option value="Car">Car</option>
                  </select>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">License Number</label>
                  <input type="text" name="licenseNumber" className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800" placeholder="DL-2024-001" />
                </div>
              </div>
              <div className="mt-6 flex gap-3">
                <Button type="submit" className="flex-1" style={{ backgroundColor: "#8B0000" }}>Add Driver</Button>
                <Button type="button" variant="outline" onClick={() => setShowAddDriverModal(false)} className="flex-1">Cancel</Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Add Property Modal */}
      {showAddPropertyModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold">Add New Property</h3>
              <button onClick={() => setShowAddPropertyModal(false)} className="p-2 hover:bg-gray-100 rounded-lg">
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleAddProperty}>
              {propertyFormError && (
                <div className="mb-4 rounded-lg bg-red-50 border border-red-200 p-3 text-sm text-red-700">
                  {propertyFormError}
                </div>
              )}
              {propertyFormSuccess && (
                <div className="mb-4 rounded-lg bg-green-50 border border-green-200 p-3 text-sm text-green-700">
                  {propertyFormSuccess}
                </div>
              )}
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Room Number *</label>
                    <input
                      type="number"
                      required
                      value={propertyForm.roomNumber}
                      onChange={(e) => setPropertyForm((prev) => ({ ...prev, roomNumber: e.target.value }))}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      placeholder="130"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-gray-700">Maximum Guests *</label>
                    <input
                      type="number"
                      required
                      value={propertyForm.maximumNoOfGuest}
                      onChange={(e) => setPropertyForm((prev) => ({ ...prev, maximumNoOfGuest: e.target.value }))}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      placeholder="8"
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="text-sm font-medium text-gray-700">Room Price *</label>
                    <input
                      type="number"
                      required
                      value={propertyForm.roomPrice}
                      onChange={(e) => setPropertyForm((prev) => ({ ...prev, roomPrice: e.target.value }))}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      placeholder="658000"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-gray-700">Room Type *</label>
                    <input
                      type="text"
                      required
                      value={propertyForm.roomType}
                      onChange={(e) => setPropertyForm((prev) => ({ ...prev, roomType: e.target.value }))}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                      placeholder="Flat"
                    />
                  </div>
                </div>

                <div>
                  <label className="text-sm font-medium text-gray-700">Room Images *</label>
                  <input
                    type="text"
                    required
                    value={propertyForm.roomImages}
                    onChange={(e) => setPropertyForm((prev) => ({ ...prev, roomImages: e.target.value }))}
                    className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                    placeholder="https://example.com/image1.jpg, https://example.com/image2.jpg"
                  />
                  <p className="text-xs text-gray-500 mt-1">Comma-separated URLs</p>
                </div>

                <div>
                  <label className="text-sm font-medium text-gray-700">Room Features *</label>
                  <input
                    type="text"
                    required
                    value={propertyForm.roomFeatures}
                    onChange={(e) => setPropertyForm((prev) => ({ ...prev, roomFeatures: e.target.value }))}
                    className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-red-800"
                    placeholder="Desk, Television, Wi-Fi, Refrigerator"
                  />
                  <p className="text-xs text-gray-500 mt-1">Comma-separated features</p>
                </div>

                <div className="flex items-center gap-3">
                  <input
                    id="isBooked"
                    type="checkbox"
                    checked={propertyForm.isBooked}
                    onChange={() => setPropertyForm((prev) => ({ ...prev, isBooked: !prev.isBooked }))}
                    className="h-4 w-4 text-red-800 border-gray-300 rounded"
                  />
                  <label htmlFor="isBooked" className="text-sm text-gray-700">
                    Mark room as booked
                  </label>
                </div>
              </div>

              <div className="mt-6 flex gap-3">
                <Button type="submit" disabled={isSubmittingProperty} className="flex-1" style={{ backgroundColor: "#8B0000" }}>
                  {isSubmittingProperty ? "Submitting..." : "Add Property"}
                </Button>
                <Button type="button" variant="outline" onClick={() => setShowAddPropertyModal(false)} className="flex-1">
                  Cancel
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}