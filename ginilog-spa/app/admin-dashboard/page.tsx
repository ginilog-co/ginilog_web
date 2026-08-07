// app/admin-dashboard/page.tsx

"use client";

import { useState, useEffect, useRef } from "react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  LayoutDashboard,
  Package,
  Hotel,
  Users,
  Settings,
  LogOut,
  Bell,
  Search,
  ArrowUpRight,
  MessageSquare,
  User,
  Building2,
  Truck,
  Loader2,
  AlertCircle,
  CheckCircle,
  XCircle,
  Clock,
  UserCheck,
  UserX,
  Shield,
  MapPin,
  Navigation,
  RefreshCw,
  Eye,
  Phone,
  Mail,
  Calendar,
  Star,
  AlertTriangle,
  Plus,
  Save,
  Trash2,
  UserPlus,
} from "lucide-react";
import {
  getStoredUser,
  logout,
  getAllUsers,
  getAllOrders,
  getAllReservations,
  updateOrderStatus,
  updateReservation,
  getAllStaff,
  getDriverStats,
  getCompanyStats,
  getNotifications,
  markNotificationRead,
  updateDriverStatus,
  assignDriverToOrder,
  getAvailableDrivers,
  getAuthStatus,
  setupAutoRefresh,
  getToken,
  getCompanies,
  addCompany,
  updateCompanyStatus,
  deleteCompany,
  getCompanyDrivers,
  registerBrandOwner,
  adminResetPassword,
  type RegisterBrandOwnerRequest,
} from "@/lib/api";
import UsersPanel from "./registered-users/page";
import CompaniesPanel from "./company-panel/page";
import ManagerApprovals from "./manager-approvals/page";

const ORDER_STATUSES = [
  "Pending",
  "Processing",
  "In_Transit",
  "Delivered",
  "Cancelled",
];
const BOOKING_STATUSES = ["Pending", "Confirmed", "Completed", "Cancelled"];
const DRIVER_STATUSES = ["Available", "On Delivery", "Off Duty"];

interface Notification {
  id: string;
  title: string;
  body: string;
  isRead: boolean;
  notificationType: string;
  createdAt: string;
  imageUrl?: string;
}

interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  status: "Available" | "On Delivery" | "Off Duty";
  available: boolean;
  rating: number;
  vehicleType?: string;
  currentLocation?: string;
  latitude?: number;
  longitude?: number;
  deliveries?: number;
}

interface Order {
  id: string;
  trackingNum: string;
  itemName: string;
  senderName: string;
  recieverName: string;
  companyName: string;
  shippingCost: number;
  itemCost: number;
  orderStatus: string;
  createdAt: string;
  riderName?: string;
  currentLocation?: string;
  latitude?: number;
  longitude?: number;
}

interface Company {
  id: string;
  companyName: string;
  companyEmail: string;
  phoneNumber: string;
  companyLogo?: string;
  companyInfo?: string;
  valueCharge: number;
  noOfTrucks?: number;
  nofOfBikes?: number;
  companyAddress?: string;
  state?: string;
  locality?: string;
  companyStatus?: string | boolean;
  rating?: number;
  createdAt?: string;
  bankName?: string;
  accountName?: string;
  accountNumber?: string;
  deliveryTypes?: string[];
  serviceAreas?: string[];
}

export default function AdminDashboard() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [users, setUsers] = useState<any[]>([]);
  const [orders, setOrders] = useState<Order[]>([]);
  const [reservations, setReservations] = useState<any[]>([]);
  const [staff, setStaff] = useState<any[]>([]);
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [availableDrivers, setAvailableDrivers] = useState<Driver[]>([]);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [orderSearch, setOrderSearch] = useState("");
  const [bookingSearch, setBookingSearch] = useState("");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [showNotifications, setShowNotifications] = useState(false);
  const [activeTab, setActiveTab] = useState<
    "overview" | "users" | "logistics" | "managers" | "companies" | "settings"
  >("overview");
  const [resetToken, setResetToken] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [resetError, setResetError] = useState<string | null>(null);
  const [resetSuccess, setResetSuccess] = useState<string | null>(null);
  const [isResetting, setIsResetting] = useState(false);
  const [selectedDriver, setSelectedDriver] = useState<Driver | null>(null);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [stats, setStats] = useState({
    activeUsers: 0,
    deliveriesInProgress: 0,
    currentBookings: 0,
    pendingManagers: 0,
    totalRevenue: 0,
    totalOrders: 0,
    totalBookings: 0,
    availableDrivers: 0,
    onDeliveryDrivers: 0,
  });

  // Company Management State
  const [companies, setCompanies] = useState<Company[]>([]);
  const [filteredCompanies, setFilteredCompanies] = useState<Company[]>([]);
  const [companySearch, setCompanySearch] = useState("");
  const [companyStatusFilter, setCompanyStatusFilter] = useState<string>("all");
  const [showAddCompanyModal, setShowAddCompanyModal] = useState(false);
  const [showCompanyDetailsModal, setShowCompanyDetailsModal] = useState(false);
  const [selectedCompany, setSelectedCompany] = useState<Company | null>(null);
  const [companyDrivers, setCompanyDrivers] = useState<any[]>([]);
  const [companyStatsData, setCompanyStatsData] = useState<any>(null);
  const [isSubmittingCompany, setIsSubmittingCompany] = useState(false);
  
  // Brand Owner Form Data with all required fields
  const [brandOwnerFormData, setBrandOwnerFormData] = useState({
    firstName: "",
    surName: "",
    email: "",
    password: "",
    confirmPassword: "",
    sex: "Male" as "Male" | "Female" | "Other",
    phoneNo: "",
    address: "",
    staffCode: "",
    companyName: "",
    companyUserName: "",
    companyType: [] as string[],
    // Required fields from backend validation
    state: "",
    branch: "",
    locality: "",
  });
  
  const [companyStatsOverview, setCompanyStatsOverview] = useState({
    total: 0,
    active: 0,
    pending: 0,
    suspended: 0,
  });

  // Default permissions for Brand Owner
  const defaultPermissions = [
    "CanCreateStaff",
    "CanManageStaff",
    "CanViewStaff",
    "CanDeleteStaff",
    "CanViewBrands",
    "CanManageBrands",
    "CanDeleteBrands",
    "CanCreateProduct",
    "CanViewProduct",
    "CanManageProduct",
    "CanDeleteProduct",
    "CanViewWallet",
    "CanManageWallet",
    "CanViewOrders",
    "CanManageOrders",
    "CanDeleteOrders",
    "CanViewBookings",
    "CanManageBookings",
    "CanDeleteBookings",
  ];

  const refreshInterval = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    const authStatus = getAuthStatus();
    if (!authStatus.isAuthenticated) {
      router.push("/admin-dashboard/admin-login");
      return;
    }
    setupAutoRefresh();
    fetchData();
    fetchCompaniesData();
    startAutoRefresh();

    return () => {
      if (refreshInterval.current) {
        clearInterval(refreshInterval.current);
      }
    };
  }, [router]);

  useEffect(() => {
    const token =
      searchParams.get("token") || searchParams.get("resetToken") || "";
    if (token) {
      setResetToken(token);
    }
  }, [searchParams]);

  const startAutoRefresh = () => {
    if (refreshInterval.current) {
      clearInterval(refreshInterval.current);
    }
    refreshInterval.current = setInterval(() => {
      fetchData();
      fetchCompaniesData();
    }, 300000);
  };

  const fetchData = async () => {
    try {
      setIsLoading(true);
      setError(null);

      const token = getToken();
      if (!token) {
        router.push("/admin-dashboard/admin-login");
        return;
      }

      const [
        allUsers,
        allOrders,
        allReservations,
        allStaff,
        driverStats,
        companyStats,
        notificationsData,
        availableDriversData,
      ] = await Promise.all([
        getAllUsers().catch(() => []),
        getAllOrders().catch(() => []),
        getAllReservations().catch(() => []),
        getAllStaff().catch(() => []),
        getDriverStats().catch(() => ({
          total: 0,
          available: 0,
          onDelivery: 0,
          offDuty: 0,
        })),
        getCompanyStats().catch(() => ({
          totalOrders: 0,
          totalRevenue: 0,
          activeDrivers: 0,
          pendingBookings: 0,
        })),
        getNotifications().catch(() => []),
        getAvailableDrivers().catch(() => []),
      ]);

      setUsers(allUsers || []);
      setOrders(allOrders || []);
      setReservations(allReservations || []);
      setStaff(allStaff || []);
      setDrivers(availableDriversData || []);
      setAvailableDrivers(availableDriversData || []);

      const notificationsList = notificationsData || [];
      setNotifications(notificationsList);
      setUnreadCount(
        notificationsList.filter((n: Notification) => !n.isRead).length
      );

      const activeUsers = (allUsers || []).filter(
        (u: any) => u.userStatus !== false
      ).length;

      const deliveriesInProgress = (allOrders || []).filter(
        (o: any) =>
          o.orderStatus === "Processing" || o.orderStatus === "In_Transit"
      ).length;

      const currentBookings = (allReservations || []).filter(
        (r: any) => r.bookingStatus === "Pending" || r.bookingStatus === "Confirmed"
      ).length;

      const pendingManagers = (allStaff || []).filter(
        (s: any) => s.adminType === "Manager" && s.userStatus === false
      ).length;

      const totalRevenue =
        (allOrders || []).reduce(
          (sum: number, o: any) => sum + (o.shippingCost || 0) + (o.itemCost || 0),
          0
        ) +
        (allReservations || []).reduce(
          (sum: number, r: any) => sum + (r.totalAmount || 0),
          0
        );

      setStats({
        activeUsers,
        deliveriesInProgress,
        currentBookings,
        pendingManagers,
        totalRevenue,
        totalOrders: (allOrders || []).length,
        totalBookings: (allReservations || []).length,
        availableDrivers: driverStats?.available || 0,
        onDeliveryDrivers: driverStats?.onDelivery || 0,
      });
    } catch (err) {
      console.error("Failed to fetch data:", err);
      setError("Failed to load dashboard data. Please refresh.");
    } finally {
      setIsLoading(false);
    }
  };

  // ============ COMPANY MANAGEMENT FUNCTIONS ============

  const fetchCompaniesData = async () => {
    try {
      const allCompanies = await getCompanies();
      setCompanies(allCompanies);
      filterCompaniesList(allCompanies, companySearch, companyStatusFilter);

      const normalizedCompanyStatus = (status: any) =>
        status === true ? "active" :
        status === false ? "suspended" :
        typeof status === "string" && status ? status : "pending";

      const active = allCompanies.filter(
        (c: any) => normalizedCompanyStatus(c.companyStatus) === "active"
      ).length;
      const pending = allCompanies.filter(
        (c: any) => normalizedCompanyStatus(c.companyStatus) === "pending"
      ).length;
      const suspended = allCompanies.filter(
        (c: any) => normalizedCompanyStatus(c.companyStatus) === "suspended"
      ).length;

      setCompanyStatsOverview({
        total: allCompanies.length,
        active,
        pending,
        suspended,
      });
    } catch (err) {
      console.error("Failed to fetch companies:", err);
    }
  };

  const filterCompaniesList = (
    data: Company[],
    search: string,
    status: string
  ) => {
    let filtered = data;

    if (status !== "all") {
      filtered = filtered.filter((c) => {
        const companyStatus = c.companyStatus;
        const normalizedStatus =
          companyStatus === true ? "active" :
          companyStatus === false ? "suspended" :
          typeof companyStatus === "string" && companyStatus ? companyStatus : "pending";

        return normalizedStatus === status;
      });
    }

    if (search) {
      const lower = search.toLowerCase();
      filtered = filtered.filter(
        (c) =>
          (c.companyName || "").toLowerCase().includes(lower) ||
          (c.companyInfo || "").toLowerCase().includes(lower) ||
          (c.companyEmail || "").toLowerCase().includes(lower)
      );
    }

    setFilteredCompanies(filtered);
  };

  const handleCompanySearch = (term: string) => {
    setCompanySearch(term);
    filterCompaniesList(companies, term, companyStatusFilter);
  };

  const handleCompanyStatusFilter = (status: string) => {
    setCompanyStatusFilter(status);
    filterCompaniesList(companies, companySearch, status);
  };

  const handleViewCompany = async (companyId: string) => {
    try {
      const company = companies.find((c) => c.id === companyId);
      setSelectedCompany(company || null);

      try {
        const stats = await getCompanyStats(companyId);
        setCompanyStatsData(stats || null);
      } catch (e) {
        setCompanyStatsData(null);
      }

      try {
        const drivers = await getCompanyDrivers(companyId);
        setCompanyDrivers(drivers || []);
      } catch (e) {
        setCompanyDrivers([]);
      }

      setShowCompanyDetailsModal(true);
    } catch (err) {
      console.error("Failed to fetch company details:", err);
    }
  };

  const handleUpdateCompanyStatus = async (companyId: string, status: string) => {
    setUpdatingId(companyId);
    try {
      await updateCompanyStatus(companyId, { CompanyStatus: status });
      await fetchCompaniesData();
    } catch (err) {
      console.error("Failed to update company status:", err);
      setError("Failed to update company status.");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleDeleteCompany = async (companyId: string) => {
    if (
      !confirm(
        "Are you sure you want to delete this company? This action cannot be undone."
      )
    ) {
      return;
    }
    setUpdatingId(companyId);
    try {
      await deleteCompany(companyId);
      await fetchCompaniesData();
    } catch (err) {
      console.error("Failed to delete company:", err);
      setError("Failed to delete company.");
    } finally {
      setUpdatingId(null);
    }
  };

  // ============ BRAND OWNER INPUT HANDLERS ============
  
  const handleBrandOwnerInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setBrandOwnerFormData({
      ...brandOwnerFormData,
      [name]: value,
    });
  };

  const handleCompanyTypeToggle = (type: string) => {
    setBrandOwnerFormData((prev) => {
      const current = prev.companyType;
      if (current.includes(type)) {
        return { ...prev, companyType: current.filter((t) => t !== type) };
      } else {
        return { ...prev, companyType: [...current, type] };
      }
    });
  };

  // ============ ADD COMPANY - Brand Owner Registration ============
  
  const handleAddCompany = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmittingCompany(true);
    try {
      // Validate Brand Owner form
      if (brandOwnerFormData.password !== brandOwnerFormData.confirmPassword) {
        setError("Passwords do not match");
        setIsSubmittingCompany(false);
        return;
      }
      
      if (brandOwnerFormData.password.length < 8) {
        setError("Password must be at least 8 characters long");
        setIsSubmittingCompany(false);
        return;
      }
      
      const requiredFields = [
        'firstName', 
        'surName', 
        'email', 
        'password', 
        'phoneNo', 
        'companyName',
        'state',
        'branch',
        'locality'
      ];
      const missingFields = requiredFields.filter(field => !brandOwnerFormData[field as keyof typeof brandOwnerFormData]);
      
      if (missingFields.length > 0) {
        setError(`Please fill in: ${missingFields.join(', ')}`);
        setIsSubmittingCompany(false);
        return;
      }
      
      if (brandOwnerFormData.companyType.length === 0) {
        setError("Please select at least one company type");
        setIsSubmittingCompany(false);
        return;
      }
      
      // Brand Owner payload with all required fields
      const payload: RegisterBrandOwnerRequest = {
        staffType: "BrandOwner",
        firstName: brandOwnerFormData.firstName,
        surName: brandOwnerFormData.surName,
        email: brandOwnerFormData.email,
        password: brandOwnerFormData.password,
        sex: brandOwnerFormData.sex,
        staffCode: brandOwnerFormData.staffCode || `${brandOwnerFormData.firstName}${Date.now().toString().slice(-4)}`,
        phoneNo: brandOwnerFormData.phoneNo,
        address: brandOwnerFormData.address || "N/A",
        companyName: brandOwnerFormData.companyName,
        companyUserName: brandOwnerFormData.companyUserName || brandOwnerFormData.companyName.toLowerCase().replace(/\s/g, ""),
        companyType: brandOwnerFormData.companyType,
        // Required fields from backend validation
        state: brandOwnerFormData.state,
        branch: brandOwnerFormData.branch,
        locality: brandOwnerFormData.locality,
        roles: ["BrandOwner"],
        permissions: defaultPermissions,
      };

      console.log("📤 Registering Brand Owner:", JSON.stringify(payload, null, 2));
      await registerBrandOwner(payload);
      
      setShowAddCompanyModal(false);
      setBrandOwnerFormData({
        firstName: "",
        surName: "",
        email: "",
        password: "",
        confirmPassword: "",
        sex: "Male",
        phoneNo: "",
        address: "",
        staffCode: "",
        companyName: "",
        companyUserName: "",
        companyType: [],
        state: "",
        branch: "",
        locality: "",
      });
      await fetchCompaniesData();
      setError(null);
    } catch (err: any) {
      console.error("Failed to register brand owner:", err);
      let errorMessage = "Failed to register brand owner. Please try again.";
      try {
        if (err.message) {
          const parsed = JSON.parse(err.message);
          if (parsed.errors) {
            const errorList = Object.values(parsed.errors).flat().join(", ");
            errorMessage = errorList || parsed.title || errorMessage;
          } else {
            errorMessage = parsed.title || parsed.message || errorMessage;
          }
        }
      } catch (e) {
        errorMessage = err.message || errorMessage;
      }
      setError(errorMessage);
    } finally {
      setIsSubmittingCompany(false);
    }
  };

  const normalizeCompanyStatus = (
    status: string | boolean | undefined,
  ): "active" | "suspended" | "pending" => {
    if (status === true) return "active";
    if (status === false) return "suspended";
    if (typeof status === "string") {
      const normalized = status.toLowerCase();
      if (normalized === "active") return "active";
      if (normalized === "suspended") return "suspended";
      if (normalized === "pending") return "pending";
    }
    return "pending";
  };

  const getCompanyStatusBadge = (status: string | boolean | undefined) => {
    const s = normalizeCompanyStatus(status);
    if (s === "active") {
      return <Badge className="bg-green-100 text-green-800">Active</Badge>;
    }
    if (s === "pending") {
      return <Badge className="bg-yellow-100 text-yellow-800">Pending</Badge>;
    }
    return <Badge className="bg-red-100 text-red-800">Suspended</Badge>;
  };

  const getCompanyStatusActions = (company: Company) => {
    const status = normalizeCompanyStatus(company.companyStatus);
    if (status === "pending") {
      return (
        <div className="flex gap-2">
          <Button
            size="sm"
            className="bg-green-600 hover:bg-green-700 text-white"
            onClick={() => handleUpdateCompanyStatus(company.id, "active")}
            disabled={updatingId === company.id}
          >
            {updatingId === company.id ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <CheckCircle className="h-4 w-4 mr-1" />
            )}
            Approve
          </Button>
          <Button
            size="sm"
            variant="destructive"
            onClick={() => handleUpdateCompanyStatus(company.id, "suspended")}
            disabled={updatingId === company.id}
          >
            {updatingId === company.id ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <XCircle className="h-4 w-4 mr-1" />
            )}
            Reject
          </Button>
        </div>
      );
    }
    if (status === "active") {
      return (
        <Button
          size="sm"
          variant="outline"
          className="text-red-600 border-red-200 hover:bg-red-50"
          onClick={() => handleUpdateCompanyStatus(company.id, "suspended")}
          disabled={updatingId === company.id}
        >
          {updatingId === company.id ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <XCircle className="h-4 w-4 mr-1" />
          )}
          Suspend
        </Button>
      );
    }
    return (
      <Button
        size="sm"
        className="bg-green-600 hover:bg-green-700 text-white"
        onClick={() => handleUpdateCompanyStatus(company.id, "active")}
        disabled={updatingId === company.id}
      >
        {updatingId === company.id ? (
          <Loader2 className="h-4 w-4 animate-spin" />
        ) : (
          <CheckCircle className="h-4 w-4 mr-1" />
        )}
        Reactivate
      </Button>
    );
  };

  const handleOrderStatusChange = async (orderId: string, newStatus: string) => {
    setUpdatingId(orderId);
    try {
      await updateOrderStatus(orderId, { OrderStatus: newStatus });
      setOrders((prev) =>
        prev.map((o) =>
          o.id === orderId ? { ...o, orderStatus: newStatus } : o
        )
      );
    } catch (err) {
      console.error("Failed to update order:", err);
      setError("Failed to update order status");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleBookingStatusChange = async (
    bookingId: string,
    newStatus: string
  ) => {
    setUpdatingId(bookingId);
    try {
      await updateReservation(bookingId, { BookingStatus: newStatus });
      setReservations((prev) =>
        prev.map((r) =>
          r.id === bookingId ? { ...r, bookingStatus: newStatus } : r
        )
      );
    } catch (err) {
      console.error("Failed to update booking:", err);
      setError("Failed to update booking status");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleDriverStatusChange = async (
    driverId: string,
    newStatus: string
  ) => {
    setUpdatingId(driverId);
    try {
      await updateDriverStatus(driverId, newStatus as any);
      setDrivers((prev) =>
        prev.map((d) =>
          d.id === driverId ? { ...d, status: newStatus as any } : d
        )
      );
      setAvailableDrivers((prev) =>
        prev.map((d) =>
          d.id === driverId ? { ...d, status: newStatus as any } : d
        )
      );
    } catch (err) {
      console.error("Failed to update driver status:", err);
      setError("Failed to update driver status");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleAssignDriver = async (orderId: string, driverId: string) => {
    setUpdatingId(orderId);
    try {
      await assignDriverToOrder(orderId, driverId);
      const driver = drivers.find((d) => d.id === driverId);
      setOrders((prev) =>
        prev.map((o) =>
          o.id === orderId
            ? { ...o, riderName: driver?.firstName + " " + driver?.lastName }
            : o
        )
      );
      setShowAssignModal(false);
      setSelectedOrderId(null);
      setSelectedDriver(null);
      fetchData();
    } catch (err) {
      console.error("Failed to assign driver:", err);
      setError("Failed to assign driver to order");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleMarkNotificationRead = async (id: string) => {
    try {
      await markNotificationRead(id, { IsRead: true });
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch (err) {
      console.error("Failed to mark notification read:", err);
    }
  };

  const markAllNotificationsRead = async () => {
    const unreadNotifications = notifications.filter((n) => !n.isRead);
    for (const notification of unreadNotifications) {
      await handleMarkNotificationRead(notification.id);
    }
  };

  const getStatusBadge = (status: string) => {
    const s = (status || "").toLowerCase();
    if (s === "delivered" || s === "confirmed" || s === "completed")
      return "bg-green-100 text-green-800";
    if (s === "in_transit" || s === "processing" || s === "on delivery")
      return "bg-blue-100 text-blue-800";
    if (s === "cancelled") return "bg-red-100 text-red-800";
    if (s === "off duty") return "bg-gray-100 text-gray-800";
    return "bg-yellow-100 text-yellow-800";
  };

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case "Escalation":
      case "FraudAlert":
        return <AlertTriangle className="h-5 w-5 text-red-500" />;
      case "Dispute":
        return <AlertCircle className="h-5 w-5 text-orange-500" />;
      case "ManagerApproval":
        return <UserCheck className="h-5 w-5 text-green-500" />;
      default:
        return <Bell className="h-5 w-5 text-blue-500" />;
    }
  };

  const navItems = [
    {
      icon: LayoutDashboard,
      label: "Overview",
      href: "#overview",
      id: "overview" as const,
    },
    {
      icon: Users,
      label: "User Management",
      href: "#users",
      id: "users" as const,
    },
    {
      icon: Truck,
      label: "Logistics Oversight",
      href: "#logistics",
      id: "logistics" as const,
    },
    {
      icon: Shield,
      label: "Manager Approvals",
      href: "#managers",
      id: "managers" as const,
    },
    {
      icon: Building2,
      label: "Companies",
      href: "#companies",
      id: "companies" as const,
    },
    { icon: Package, label: "Orders", href: "#orders" },
    { icon: Hotel, label: "Bookings", href: "#bookings" },
    {
      icon: Building2,
      label: "Company Portal",
      href: "/admin-dashboard/company",
    },
    { icon: Settings, label: "Settings", id: "settings" as const, href: "#settings" },
  ];

  const activeDeliveries = orders.filter(
    (o) => o.orderStatus === "In_Transit" || o.orderStatus === "Processing"
  );

  const filteredOrders = orders.filter(
    (o) =>
      !orderSearch ||
      (o.trackingNum || "")
        .toLowerCase()
        .includes(orderSearch.toLowerCase()) ||
      (o.itemName || "").toLowerCase().includes(orderSearch.toLowerCase()) ||
      (o.senderName || "").toLowerCase().includes(orderSearch.toLowerCase())
  );

  const filteredReservations = reservations.filter(
    (r) =>
      !bookingSearch ||
      (r.bookingRefNo || "")
        .toLowerCase()
        .includes(bookingSearch.toLowerCase()) ||
      (r.accomodationName || "")
        .toLowerCase()
        .includes(bookingSearch.toLowerCase()) ||
      (r.guestName || "").toLowerCase().includes(bookingSearch.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <Loader2 className="h-10 w-10 animate-spin text-primary mx-auto" />
          <p className="mt-4 text-gray-600">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 flex">
      {/* Sidebar */}
      <aside
        className={`bg-gray-900 text-white transition-all duration-300 ${
          isSidebarOpen ? "w-64" : "w-20"
        } fixed h-full z-40`}
      >
        <div className="h-16 flex items-center justify-center border-b border-gray-800">
          <Link href="/admin-dashboard" className="text-xl font-bold">
            {isSidebarOpen ? "GINILOG Admin" : "GNL"}
          </Link>
        </div>
        <nav className="mt-6 px-3">
          {navItems.map((item) => (
            <button
              key={item.label}
              onClick={() => {
                if (item.id) {
                  setActiveTab(item.id);
                }
                if (item.href.startsWith("/")) {
                  router.push(item.href);
                }
              }}
              className={`flex items-center gap-3 px-3 py-3 rounded-lg mb-1 transition-colors w-full text-left ${
                activeTab === item.id
                  ? "bg-primary text-white"
                  : "text-gray-400 hover:bg-gray-800 hover:text-white"
              }`}
            >
              <item.icon className="h-5 w-5 flex-shrink-0" />
              {isSidebarOpen && (
                <span className="text-sm font-medium">{item.label}</span>
              )}
            </button>
          ))}
        </nav>
        <div className="absolute bottom-0 left-0 right-0 p-3 border-t border-gray-800">
          <button
            onClick={() => {
              logout();
              router.push("/admin-dashboard/admin-login");
            }}
            className="flex items-center gap-3 px-3 py-3 rounded-lg text-gray-400 hover:bg-gray-800 hover:text-white transition-colors w-full"
          >
            <LogOut className="h-5 w-5 flex-shrink-0" />
            {isSidebarOpen && (
              <span className="text-sm font-medium">Logout</span>
            )}
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div
        className={`flex-1 transition-all duration-300 ${
          isSidebarOpen ? "ml-64" : "ml-20"
        }`}
      >
        {/* Header */}
        <header className="bg-white border-b sticky top-0 z-30">
          <div className="flex items-center justify-between px-6 py-4">
            <div className="flex items-center gap-4">
              <button
                onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <LayoutDashboard className="h-5 w-5 text-gray-600" />
              </button>
              <h1 className="text-xl font-semibold text-gray-900">
                {activeTab === "overview" && "Dashboard Overview"}
                {activeTab === "users" && "User Management"}
                {activeTab === "logistics" && "Logistics Oversight"}
                {activeTab === "managers" && "Manager Approvals"}
                {activeTab === "companies" && "Company Management"}
                {activeTab === "settings" && "Settings"}
              </h1>
            </div>
            <div className="flex items-center gap-4">
              <button
                onClick={() => {
                  fetchData();
                  fetchCompaniesData();
                }}
                className="p-2 hover:bg-gray-100 rounded-lg"
                title="Refresh data"
              >
                <RefreshCw className="h-5 w-5 text-gray-600" />
              </button>

              {/* Notifications */}
              <div className="relative">
                <button
                  onClick={() => setShowNotifications(!showNotifications)}
                  className="relative p-2 hover:bg-gray-100 rounded-lg"
                >
                  <Bell className="h-5 w-5 text-gray-600" />
                  {unreadCount > 0 && (
                    <span className="absolute top-1 right-1 h-4 w-4 bg-primary text-white text-xs rounded-full flex items-center justify-center">
                      {unreadCount}
                    </span>
                  )}
                </button>
                {showNotifications && (
                  <div className="absolute right-0 mt-2 w-96 bg-white rounded-lg shadow-lg border z-50 max-h-96 overflow-y-auto">
                    <div className="p-4 border-b flex items-center justify-between sticky top-0 bg-white">
                      <h3 className="font-semibold">Notifications</h3>
                      {unreadCount > 0 && (
                        <button
                          onClick={markAllNotificationsRead}
                          className="text-xs text-primary hover:underline"
                        >
                          Mark all read
                        </button>
                      )}
                    </div>
                    {notifications.length === 0 ? (
                      <div className="p-4 text-center text-gray-500">
                        No notifications
                      </div>
                    ) : (
                      notifications.map((notification) => (
                        <div
                          key={notification.id}
                          className={`p-4 border-b hover:bg-gray-50 cursor-pointer transition-colors ${
                            !notification.isRead ? "bg-blue-50" : ""
                          }`}
                          onClick={() => handleMarkNotificationRead(notification.id)}
                        >
                          <div className="flex items-start gap-3">
                            <div className="mt-1">
                              {getNotificationIcon(notification.notificationType)}
                            </div>
                            <div className="flex-1 min-w-0">
                              <p className="text-sm font-medium text-gray-900">
                                {notification.title}
                              </p>
                              <p className="text-sm text-gray-600 line-clamp-2">
                                {notification.body}
                              </p>
                              <p className="text-xs text-gray-400 mt-1">
                                {new Date(
                                  notification.createdAt
                                ).toLocaleString()}
                              </p>
                            </div>
                            {!notification.isRead && (
                              <div className="h-2 w-2 bg-primary rounded-full flex-shrink-0 mt-2" />
                            )}
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                )}
              </div>

              <div className="flex items-center gap-3 pl-4 border-l">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden">
                  <User className="h-5 w-5 text-primary" />
                </div>
                <div className="hidden md:block">
                  <p className="text-sm font-medium text-gray-900">
                    Administrator
                  </p>
                  <p className="text-xs text-gray-500">Super Admin</p>
                </div>
              </div>
            </div>
          </div>
        </header>

        <main className="p-6">
          {error && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg flex items-center justify-between text-red-600 text-sm">
              <div className="flex items-center gap-2">
                <AlertCircle className="h-4 w-4" />
                <span>{error}</span>
              </div>
              <button
                onClick={() => setError(null)}
                className="text-red-400 hover:text-red-600"
              >
                <XCircle className="h-4 w-4" />
              </button>
            </div>
          )}

          {activeTab === "settings" && (
            <div className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle>Admin Password Reset</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-sm text-gray-600 mb-4">
                    Use a valid reset token to change the admin password.
                  </p>

                  {resetSuccess && (
                    <div className="rounded-lg bg-green-50 border border-green-200 p-4 text-green-800">
                      {resetSuccess}
                    </div>
                  )}
                  {resetError && (
                    <div className="rounded-lg bg-red-50 border border-red-200 p-4 text-red-800">
                      {resetError}
                    </div>
                  )}

                  <div className="grid gap-4 md:grid-cols-2">
                    <div className="md:col-span-2">
                      <label className="block text-sm font-medium text-gray-700">
                        Reset Token
                      </label>
                      <input
                        type="text"
                        value={resetToken}
                        onChange={(e) => setResetToken(e.target.value)}
                        className="mt-1 block w-full rounded-lg border px-3 py-2 text-sm focus:border-primary focus:ring-primary"
                        placeholder="Enter reset token"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700">
                        New Password
                      </label>
                      <input
                        type="password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                        className="mt-1 block w-full rounded-lg border px-3 py-2 text-sm focus:border-primary focus:ring-primary"
                        placeholder="New password"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">
                        Confirm Password
                      </label>
                      <input
                        type="password"
                        value={confirmNewPassword}
                        onChange={(e) => setConfirmNewPassword(e.target.value)}
                        className="mt-1 block w-full rounded-lg border px-3 py-2 text-sm focus:border-primary focus:ring-primary"
                        placeholder="Confirm new password"
                      />
                    </div>
                  </div>

                  <div className="flex justify-end">
                    <button
                      type="button"
                      onClick={async () => {
                        setResetError(null);
                        setResetSuccess(null);
                        if (!resetToken.trim()) {
                          setResetError("Reset token is required.");
                          return;
                        }
                        if (newPassword.length < 8) {
                          setResetError("Password must be at least 8 characters.");
                          return;
                        }
                        if (newPassword !== confirmNewPassword) {
                          setResetError("Passwords do not match.");
                          return;
                        }

                        setIsResetting(true);
                        try {
                          await adminResetPassword({
                            Token: resetToken.trim(),
                            Password: newPassword,
                          });
                          setResetSuccess("Password reset successfully.");
                          setResetToken("");
                          setNewPassword("");
                          setConfirmNewPassword("");
                        } catch (err) {
                          setResetError(
                            err instanceof Error
                              ? err.message
                              : "Failed to reset password."
                          );
                        } finally {
                          setIsResetting(false);
                        }
                      }}
                      className="inline-flex items-center justify-center rounded-lg bg-primary px-5 py-2.5 text-sm font-semibold text-white hover:bg-primary/90"
                      disabled={isResetting}
                    >
                      {isResetting ? "Resetting..." : "Reset Password"}
                    </button>
                  </div>
                </CardContent>
              </Card>
            </div>
          )}

          {/* ============================ */}
          {/* OVERVIEW TAB */}
          {/* ============================ */}
          {activeTab === "overview" && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                <Card className="hover:shadow-md transition-shadow">
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between">
                      <div>
                        <p className="text-gray-500 text-sm">Active Users</p>
                        <h3 className="text-2xl font-bold text-gray-900 mt-1">
                          {stats.activeUsers}
                        </h3>
                        <p className="text-xs text-green-600 mt-1">
                          <Users className="h-3 w-3 inline mr-1" />
                          {users.length} total users
                        </p>
                      </div>
                      <div className="p-3 rounded-lg bg-blue-100 text-blue-600">
                        <Users className="h-5 w-5" />
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="hover:shadow-md transition-shadow">
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between">
                      <div>
                        <p className="text-gray-500 text-sm">
                          Deliveries In Progress
                        </p>
                        <h3 className="text-2xl font-bold text-gray-900 mt-1">
                          {stats.deliveriesInProgress}
                        </h3>
                        <p className="text-xs text-gray-500 mt-1">
                          <Truck className="h-3 w-3 inline mr-1" />
                          {stats.availableDrivers} drivers available
                        </p>
                      </div>
                      <div className="p-3 rounded-lg bg-primary/10 text-primary">
                        <Truck className="h-5 w-5" />
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="hover:shadow-md transition-shadow">
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between">
                      <div>
                        <p className="text-gray-500 text-sm">Current Bookings</p>
                        <h3 className="text-2xl font-bold text-gray-900 mt-1">
                          {stats.currentBookings}
                        </h3>
                        <p className="text-xs text-gray-500 mt-1">
                          <Hotel className="h-3 w-3 inline mr-1" />
                          {stats.totalBookings} total bookings
                        </p>
                      </div>
                      <div className="p-3 rounded-lg bg-purple-100 text-purple-600">
                        <Hotel className="h-5 w-5" />
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="hover:shadow-md transition-shadow">
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between">
                      <div>
                        <p className="text-gray-500 text-sm">Total Revenue</p>
                        <h3 className="text-2xl font-bold text-green-600 mt-1">
                          ₦{stats.totalRevenue.toLocaleString()}
                        </h3>
                        <p className="text-xs text-gray-500 mt-1">
                          <ArrowUpRight className="h-3 w-3 inline mr-1" />
                          From {stats.totalOrders} orders
                        </p>
                      </div>
                      <div className="p-3 rounded-lg bg-green-100 text-green-600">
                        <ArrowUpRight className="h-5 w-5" />
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg flex items-center gap-2">
                      <AlertCircle className="h-5 w-5 text-orange-500" />
                      Recent Alerts
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    {notifications.length === 0 ? (
                      <p className="text-gray-500 text-sm">No alerts</p>
                    ) : (
                      <div className="space-y-3">
                        {notifications.slice(0, 5).map((n) => (
                          <div
                            key={n.id}
                            className={`flex items-start gap-3 p-3 rounded-lg ${
                              !n.isRead ? "bg-orange-50" : "bg-gray-50"
                            }`}
                          >
                            {getNotificationIcon(n.notificationType)}
                            <div className="flex-1">
                              <p className="text-sm font-medium">{n.title}</p>
                              <p className="text-xs text-gray-500 line-clamp-1">
                                {n.body}
                              </p>
                              <p className="text-xs text-gray-400 mt-1">
                                {new Date(n.createdAt).toLocaleString()}
                              </p>
                            </div>
                          </div>
                        ))}
                      </div>
                    )}
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg flex items-center gap-2">
                      <Clock className="h-5 w-5 text-blue-500" />
                      Pending Actions
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-3">
                    <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                      <div className="flex items-center gap-2">
                        <UserCheck className="h-4 w-4 text-green-500" />
                        <span className="text-sm">Manager approvals</span>
                      </div>
                      <Badge className="bg-yellow-100 text-yellow-800">
                        {stats.pendingManagers} pending
                      </Badge>
                    </div>
                    <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                      <div className="flex items-center gap-2">
                        <Truck className="h-4 w-4 text-blue-500" />
                        <span className="text-sm">Unassigned deliveries</span>
                      </div>
                      <Badge className="bg-red-100 text-red-800">
                        {stats.deliveriesInProgress - stats.availableDrivers > 0
                          ? stats.deliveriesInProgress - stats.availableDrivers
                          : 0}
                      </Badge>
                    </div>
                    <div className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                      <div className="flex items-center gap-2">
                        <AlertTriangle className="h-4 w-4 text-orange-500" />
                        <span className="text-sm">Disputes/Fraud alerts</span>
                      </div>
                      <Badge className="bg-red-100 text-red-800">
                        {
                          notifications.filter(
                            (n) =>
                              n.notificationType === "Dispute" ||
                              n.notificationType === "FraudAlert"
                          ).length
                        }
                      </Badge>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mb-8">
                <button
                  onClick={() => setActiveTab("users")}
                  className="p-4 bg-white rounded-lg border hover:shadow-md transition-shadow text-left"
                >
                  <Users className="h-6 w-6 text-blue-500 mb-2" />
                  <p className="font-medium">User Management</p>
                  <p className="text-xs text-gray-500">
                    Manage users & verification
                  </p>
                </button>
                <button
                  onClick={() => setActiveTab("logistics")}
                  className="p-4 bg-white rounded-lg border hover:shadow-md transition-shadow text-left"
                >
                  <Truck className="h-6 w-6 text-primary mb-2" />
                  <p className="font-medium">Logistics Oversight</p>
                  <p className="text-xs text-gray-500">
                    Monitor deliveries & assign drivers
                  </p>
                </button>
                <button
                  onClick={() => setActiveTab("managers")}
                  className="p-4 bg-white rounded-lg border hover:shadow-md transition-shadow text-left"
                >
                  <Shield className="h-6 w-6 text-green-500 mb-2" />
                  <p className="font-medium">Manager Approvals</p>
                  <p className="text-xs text-gray-500">
                    Approve/reject applications
                  </p>
                </button>
                <button
                  onClick={() => setActiveTab("companies")}
                  className="p-4 bg-white rounded-lg border hover:shadow-md transition-shadow text-left"
                >
                  <Building2 className="h-6 w-6 text-purple-500 mb-2" />
                  <p className="font-medium">Company Management</p>
                  <p className="text-xs text-gray-500">Manage all companies</p>
                </button>
                <Link
                  href="/admin-dashboard/company"
                  className="p-4 bg-white rounded-lg border hover:shadow-md transition-shadow text-left"
                >
                  <Building2 className="h-6 w-6 text-indigo-500 mb-2" />
                  <p className="font-medium">Company Portal</p>
                  <p className="text-xs text-gray-500">Company dashboard</p>
                </Link>
              </div>
            </>
          )}

          {/* ============================ */}
          {/* USER MANAGEMENT TAB */}
          {/* ============================ */}
          {activeTab === "users" && (
            <>
              <UsersPanel />

              <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6">
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Total Users</p>
                        <p className="text-2xl font-bold">{users.length}</p>
                      </div>
                      <Users className="h-8 w-8 text-blue-500" />
                    </div>
                    <div className="mt-2 flex gap-2">
                      <Badge className="bg-green-100 text-green-800">
                        {
                          users.filter((u: any) => u.userStatus !== false)
                            .length
                        }{" "}
                        Active
                      </Badge>
                      <Badge className="bg-red-100 text-red-800">
                        {users.filter((u: any) => u.userStatus === false).length}{" "}
                        Suspended
                      </Badge>
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">
                          Verification Status
                        </p>
                        <p className="text-2xl font-bold">
                          {users.filter((u: any) => u.emailVerified).length}
                        </p>
                      </div>
                      <CheckCircle className="h-8 w-8 text-green-500" />
                    </div>
                    <p className="text-xs text-gray-500 mt-2">
                      {
                        users.filter((u: any) => !u.emailVerified).length
                      }{" "}
                      pending verification
                    </p>
                  </CardContent>
                </Card>

                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">User Types</p>
                        <p className="text-2xl font-bold">
                          {
                            users.filter((u: any) => u.userType === "Customer")
                              .length
                          }
                        </p>
                      </div>
                      <User className="h-8 w-8 text-purple-500" />
                    </div>
                    <p className="text-xs text-gray-500 mt-2">
                      {users.filter((u: any) => u.userType === "Staff").length}{" "}
                      Staff members
                    </p>
                  </CardContent>
                </Card>
              </div>
            </>
          )}

          {/* ============================ */}
          {/* LOGISTICS OVERSIGHT TAB */}
          {/* ============================ */}
          {activeTab === "logistics" && (
            <>
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Active Deliveries</p>
                        <p className="text-2xl font-bold">
                          {activeDeliveries.length}
                        </p>
                      </div>
                      <Truck className="h-6 w-6 text-primary" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Available Drivers</p>
                        <p className="text-2xl font-bold text-green-600">
                          {
                            drivers.filter((d) => d.status === "Available")
                              .length
                          }
                        </p>
                      </div>
                      <UserCheck className="h-6 w-6 text-green-500" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">On Delivery</p>
                        <p className="text-2xl font-bold text-blue-600">
                          {
                            drivers.filter((d) => d.status === "On Delivery")
                              .length
                          }
                        </p>
                      </div>
                      <Navigation className="h-6 w-6 text-blue-500" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Total Orders</p>
                        <p className="text-2xl font-bold">{stats.totalOrders}</p>
                      </div>
                      <Package className="h-6 w-6 text-purple-500" />
                    </div>
                  </CardContent>
                </Card>
              </div>

              <Card className="mb-6">
                <CardHeader>
                  <CardTitle className="text-lg flex items-center gap-2">
                    <MapPin className="h-5 w-5 text-primary" />
                    Active Deliveries Map
                    <Badge className="ml-2 bg-blue-100 text-blue-800">
                      {activeDeliveries.length} active
                    </Badge>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="bg-gray-100 rounded-lg p-4 min-h-[200px] relative">
                    <div className="absolute inset-0 flex items-center justify-center">
                      <div className="text-center">
                        <MapPin className="h-12 w-12 text-primary mx-auto opacity-50" />
                        <p className="text-gray-500 text-sm mt-2">
                          {activeDeliveries.length === 0
                            ? "No active deliveries"
                            : `${activeDeliveries.length} delivery(ies) in progress`}
                        </p>
                        {activeDeliveries.length > 0 && (
                          <div className="mt-2 flex flex-wrap gap-2 justify-center">
                            {activeDeliveries.slice(0, 5).map((order) => (
                              <Badge
                                key={order.id}
                                className="bg-blue-100 text-blue-800"
                              >
                                {order.trackingNum || order.id.slice(0, 8)}
                              </Badge>
                            ))}
                            {activeDeliveries.length > 5 && (
                              <Badge className="bg-gray-200 text-gray-800">
                                +{activeDeliveries.length - 5} more
                              </Badge>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              <Card className="mb-6">
                <CardHeader>
                  <CardTitle className="text-lg flex items-center gap-2">
                    <Users className="h-5 w-5" />
                    Driver Management
                    <Badge className="ml-2 bg-green-100 text-green-800">
                      {
                        drivers.filter((d) => d.status === "Available").length
                      }{" "}
                      available
                    </Badge>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {drivers.length === 0 ? (
                    <p className="text-center text-gray-500 py-4">
                      No drivers available
                    </p>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Driver
                            </th>
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Vehicle
                            </th>
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Status
                            </th>
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Rating
                            </th>
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Deliveries
                            </th>
                            <th className="text-left py-2 px-3 font-medium text-gray-600">
                              Actions
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                          {drivers.map((driver) => (
                            <tr
                              key={driver.id}
                              className="border-b hover:bg-gray-50"
                            >
                              <td className="py-2 px-3">
                                <div>
                                  <p className="font-medium">
                                    {driver.firstName} {driver.lastName}
                                  </p>
                                  <p className="text-xs text-gray-500">
                                    {driver.email}
                                  </p>
                                </div>
                              </td>
                              <td className="py-2 px-3">
                                {driver.vehicleType || "N/A"}
                              </td>
                              <td className="py-2 px-3">
                                <Badge
                                  className={getStatusBadge(driver.status)}
                                >
                                  {driver.status || "Off Duty"}
                                </Badge>
                              </td>
                              <td className="py-2 px-3">
                                <div className="flex items-center gap-1">
                                  <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />
                                  {driver.rating || 0}
                                </div>
                              </td>
                              <td className="py-2 px-3">
                                {driver.deliveries || 0}
                              </td>
                              <td className="py-2 px-3">
                                {updatingId === driver.id ? (
                                  <Loader2 className="h-4 w-4 animate-spin text-gray-400" />
                                ) : (
                                  <select
                                    value={driver.status || "Off Duty"}
                                    onChange={(e) =>
                                      handleDriverStatusChange(
                                        driver.id,
                                        e.target.value
                                      )
                                    }
                                    className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                                  >
                                    {DRIVER_STATUSES.map((s) => (
                                      <option key={s} value={s}>
                                        {s}
                                      </option>
                                    ))}
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

              <Card>
                <CardHeader>
                  <CardTitle className="text-lg flex items-center gap-2">
                    <Package className="h-5 w-5" />
                    Assign Drivers to Orders
                    <Badge className="ml-2 bg-yellow-100 text-yellow-800">
                      {
                        orders.filter(
                          (o) =>
                            !o.riderName && o.orderStatus !== "Delivered"
                        ).length
                      }{" "}
                      unassigned
                    </Badge>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b bg-gray-50">
                          <th className="text-left py-2 px-3 font-medium text-gray-600">
                            Order
                          </th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">
                            Item
                          </th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">
                            Status
                          </th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">
                            Driver
                          </th>
                          <th className="text-left py-2 px-3 font-medium text-gray-600">
                            Action
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {orders
                          .filter(
                            (o) =>
                              o.orderStatus !== "Delivered" &&
                              o.orderStatus !== "Cancelled"
                          )
                          .slice(0, 10)
                          .map((order) => (
                            <tr
                              key={order.id}
                              className="border-b hover:bg-gray-50"
                            >
                              <td className="py-2 px-3 font-mono text-xs">
                                {order.trackingNum || order.id.slice(0, 8)}
                              </td>
                              <td className="py-2 px-3">{order.itemName}</td>
                              <td className="py-2 px-3">
                                <Badge
                                  className={getStatusBadge(order.orderStatus)}
                                >
                                  {order.orderStatus}
                                </Badge>
                              </td>
                              <td className="py-2 px-3">
                                {order.riderName || (
                                  <span className="text-gray-400 text-xs">
                                    Unassigned
                                  </span>
                                )}
                              </td>
                              <td className="py-2 px-3">
                                {updatingId === order.id ? (
                                  <Loader2 className="h-4 w-4 animate-spin text-gray-400" />
                                ) : order.riderName ? (
                                  <Badge className="bg-green-100 text-green-800">
                                    Assigned
                                  </Badge>
                                ) : (
                                  <button
                                    onClick={() => {
                                      setSelectedOrderId(order.id);
                                      setShowAssignModal(true);
                                    }}
                                    className="text-primary hover:underline text-xs"
                                  >
                                    Assign
                                  </button>
                                )}
                              </td>
                            </tr>
                          ))}
                      </tbody>
                    </table>
                  </div>
                </CardContent>
              </Card>
            </>
          )}

          {/* ============================ */}
          {/* MANAGER APPROVALS TAB */}
          {/* ============================ */}
          {activeTab === "managers" && <ManagerApprovals />}

          {/* ============================ */}
          {/* COMPANIES TAB */}
          {/* ============================ */}
          {activeTab === "companies" && (
            <div className="space-y-6">
              {/* Company Stats */}
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Total Companies</p>
                        <p className="text-2xl font-bold">
                          {companyStatsOverview.total}
                        </p>
                      </div>
                      <Building2 className="h-8 w-8 text-blue-500" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Active</p>
                        <p className="text-2xl font-bold text-green-600">
                          {companyStatsOverview.active}
                        </p>
                      </div>
                      <CheckCircle className="h-8 w-8 text-green-500" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Pending</p>
                        <p className="text-2xl font-bold text-yellow-600">
                          {companyStatsOverview.pending}
                        </p>
                      </div>
                      <Clock className="h-8 w-8 text-yellow-500" />
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-500">Suspended</p>
                        <p className="text-2xl font-bold text-red-600">
                          {companyStatsOverview.suspended}
                        </p>
                      </div>
                      <XCircle className="h-8 w-8 text-red-500" />
                    </div>
                  </CardContent>
                </Card>
              </div>

              {/* Company Filters */}
              <div className="flex flex-col sm:flex-row gap-3">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <input
                    type="text"
                    placeholder="Search companies by name, email, or info..."
                    value={companySearch}
                    onChange={(e) => handleCompanySearch(e.target.value)}
                    className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
                  />
                </div>
                <select
                  value={companyStatusFilter}
                  onChange={(e) => handleCompanyStatusFilter(e.target.value)}
                  className="px-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                >
                  <option value="all">All Status</option>
                  <option value="active">Active</option>
                  <option value="pending">Pending</option>
                  <option value="suspended">Suspended</option>
                </select>
                <Button
                  className="gap-2 bg-primary hover:bg-primary/90"
                  onClick={() => setShowAddCompanyModal(true)}
                >
                  <Plus className="h-4 w-4" />
                  Register Brand Owner
                </Button>
              </div>

              {/* Companies Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {filteredCompanies.length === 0 ? (
                  <div className="col-span-full text-center py-12">
                    <Building2 className="h-12 w-12 text-gray-300 mx-auto mb-4" />
                    <p className="text-gray-500">No companies found</p>
                    <Button
                      variant="outline"
                      className="mt-4"
                      onClick={() => setShowAddCompanyModal(true)}
                    >
                      <Plus className="h-4 w-4 mr-2" />
                      Register Your First Brand Owner
                    </Button>
                  </div>
                ) : (
                  filteredCompanies.map((company) => (
                    <Card
                      key={company.id}
                      className="hover:shadow-md transition-shadow"
                    >
                      <CardContent className="p-4">
                        <div className="flex items-start justify-between">
                          <div className="flex items-center gap-3">
                            <div className="h-14 w-14 rounded-lg bg-primary/10 flex items-center justify-center overflow-hidden flex-shrink-0">
                              {company.companyLogo ? (
                                <img
                                  src={company.companyLogo}
                                  alt={company.companyName}
                                  className="h-full w-full object-cover"
                                />
                              ) : (
                                <Building2 className="h-8 w-8 text-primary" />
                              )}
                            </div>
                            <div>
                              <h3 className="font-semibold text-gray-900">
                                {company.companyName}
                              </h3>
                              <div className="flex items-center gap-2 mt-1">
                                {getCompanyStatusBadge(company.companyStatus)}
                                {company.rating && (
                                  <span className="flex items-center text-yellow-600 text-sm">
                                    <Star className="h-3 w-3 fill-yellow-500 text-yellow-500 mr-1" />
                                    {company.rating}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                          <div className="flex gap-1">
                            <button
                              onClick={() => handleViewCompany(company.id)}
                              className="p-1.5 hover:bg-gray-100 rounded"
                              title="View details"
                            >
                              <Eye className="h-4 w-4 text-gray-500" />
                            </button>
                            <button
                              onClick={() => handleDeleteCompany(company.id)}
                              className="p-1.5 hover:bg-red-100 rounded text-red-500"
                              title="Delete company"
                              disabled={updatingId === company.id}
                            >
                              {updatingId === company.id ? (
                                <Loader2 className="h-4 w-4 animate-spin" />
                              ) : (
                                <Trash2 className="h-4 w-4" />
                              )}
                            </button>
                          </div>
                        </div>

                        <div className="mt-3 space-y-1 text-sm">
                          {company.companyEmail && (
                            <p className="text-gray-600 flex items-center gap-1">
                              <Mail className="h-3.5 w-3.5 text-gray-400" />
                              {company.companyEmail}
                            </p>
                          )}
                          {company.phoneNumber && (
                            <p className="text-gray-600 flex items-center gap-1">
                              <Phone className="h-3.5 w-3.5 text-gray-400" />
                              {company.phoneNumber}
                            </p>
                          )}
                          <p className="text-gray-600 flex items-center gap-1">
                            <Truck className="h-3.5 w-3.5 text-gray-400" />
                            Charge: ₦
                            {company.valueCharge?.toLocaleString() || "0"}
                          </p>
                          {(company.noOfTrucks || company.nofOfBikes) && (
                            <p className="text-gray-600 text-xs">
                              🚛 {company.noOfTrucks || 0} trucks · 🏍️{" "}
                              {company.nofOfBikes || 0} bikes
                            </p>
                          )}
                        </div>

                        <div className="mt-3 pt-3 border-t flex justify-between items-center">
                          <div className="flex gap-2">
                            {getCompanyStatusActions(company)}
                          </div>
                          <button
                            onClick={() => handleViewCompany(company.id)}
                            className="text-primary text-sm hover:underline"
                          >
                            View Details
                          </button>
                        </div>
                      </CardContent>
                    </Card>
                  ))
                )}
              </div>
            </div>
          )}

          {/* ============================ */}
          {/* ORDERS SECTION (visible in overview) */}
          {/* ============================ */}
          {activeTab === "overview" && (
            <>
              <Card className="mb-8" id="orders">
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>Recent Orders ({orders.length})</CardTitle>
                  <div className="relative w-52">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <input
                      type="text"
                      placeholder="Search orders..."
                      value={orderSearch}
                      onChange={(e) => setOrderSearch(e.target.value)}
                      className="pl-9 pr-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
                    />
                  </div>
                </CardHeader>
                <CardContent>
                  {filteredOrders.length === 0 ? (
                    <p className="text-center text-gray-500 py-8">
                      No orders found.
                    </p>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Tracking #
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Item
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Sender
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Receiver
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Status
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Amount
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Update
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                          {filteredOrders.slice(0, 10).map((order) => (
                            <tr
                              key={order.id}
                              className="border-b hover:bg-gray-50"
                            >
                              <td className="py-3 px-4 font-mono text-xs">
                                {order.trackingNum || order.id?.slice(0, 8)}
                              </td>
                              <td className="py-3 px-4">{order.itemName}</td>
                              <td className="py-3 px-4">{order.senderName}</td>
                              <td className="py-3 px-4">{order.recieverName}</td>
                              <td className="py-3 px-4">
                                <Badge
                                  className={getStatusBadge(order.orderStatus)}
                                >
                                  {order.orderStatus || "Pending"}
                                </Badge>
                              </td>
                              <td className="py-3 px-4 font-medium">
                                ₦
                                {(
                                  (order.shippingCost || 0) +
                                  (order.itemCost || 0)
                                ).toLocaleString()}
                              </td>
                              <td className="py-3 px-4">
                                {updatingId === order.id ? (
                                  <Loader2 className="h-4 w-4 animate-spin text-gray-400" />
                                ) : (
                                  <select
                                    value={order.orderStatus || "Pending"}
                                    onChange={(e) =>
                                      handleOrderStatusChange(
                                        order.id,
                                        e.target.value
                                      )
                                    }
                                    className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                                  >
                                    {ORDER_STATUSES.map((s) => (
                                      <option key={s} value={s}>
                                        {s}
                                      </option>
                                    ))}
                                  </select>
                                )}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                      {filteredOrders.length > 10 && (
                        <p className="text-center text-gray-400 text-xs mt-4">
                          Showing 10 of {filteredOrders.length} orders
                        </p>
                      )}
                    </div>
                  )}
                </CardContent>
              </Card>

              <Card id="bookings">
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>Recent Bookings ({reservations.length})</CardTitle>
                  <div className="relative w-52">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <input
                      type="text"
                      placeholder="Search bookings..."
                      value={bookingSearch}
                      onChange={(e) => setBookingSearch(e.target.value)}
                      className="pl-9 pr-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
                    />
                  </div>
                </CardHeader>
                <CardContent>
                  {filteredReservations.length === 0 ? (
                    <p className="text-center text-gray-500 py-8">
                      No reservations found.
                    </p>
                  ) : (
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead>
                          <tr className="border-b bg-gray-50">
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Accommodation
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Guest
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Ref #
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Check-in
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Status
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Amount
                            </th>
                            <th className="text-left py-3 px-4 font-medium text-gray-600">
                              Update
                            </th>
                          </tr>
                        </thead>
                        <tbody>
                          {filteredReservations.slice(0, 10).map((res) => (
                            <tr
                              key={res.id}
                              className="border-b hover:bg-gray-50"
                            >
                              <td className="py-3 px-4">
                                {res.accomodationName}
                              </td>
                              <td className="py-3 px-4">{res.guestName}</td>
                              <td className="py-3 px-4 font-mono text-xs">
                                {res.bookingRefNo || res.id?.slice(0, 8)}
                              </td>
                              <td className="py-3 px-4">
                                {res.checkInDate
                                  ? new Date(
                                      res.checkInDate
                                    ).toLocaleDateString()
                                  : "—"}
                              </td>
                              <td className="py-3 px-4">
                                <Badge
                                  className={getStatusBadge(res.bookingStatus)}
                                >
                                  {res.bookingStatus || "Pending"}
                                </Badge>
                              </td>
                              <td className="py-3 px-4 font-medium text-primary">
                                ₦{(res.totalAmount || 0).toLocaleString()}
                              </td>
                              <td className="py-3 px-4">
                                {updatingId === res.id ? (
                                  <Loader2 className="h-4 w-4 animate-spin text-gray-400" />
                                ) : (
                                  <select
                                    value={res.bookingStatus || "Pending"}
                                    onChange={(e) =>
                                      handleBookingStatusChange(
                                        res.id,
                                        e.target.value
                                      )
                                    }
                                    className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                                  >
                                    {BOOKING_STATUSES.map((s) => (
                                      <option key={s} value={s}>
                                        {s}
                                      </option>
                                    ))}
                                  </select>
                                )}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                      {filteredReservations.length > 10 && (
                        <p className="text-center text-gray-400 text-xs mt-4">
                          Showing 10 of {filteredReservations.length} bookings
                        </p>
                      )}
                    </div>
                  )}
                </CardContent>
              </Card>
            </>
          )}
        </main>
      </div>

      {/* Assign Driver Modal */}
      {showAssignModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
            <h3 className="text-lg font-semibold mb-4">Assign Driver</h3>
            <p className="text-sm text-gray-500 mb-4">
              Select a driver to assign to this order
            </p>
            <div className="max-h-60 overflow-y-auto space-y-2">
              {availableDrivers.length === 0 ? (
                <p className="text-gray-500 text-sm">No available drivers</p>
              ) : (
                availableDrivers.map((driver) => (
                  <button
                    key={driver.id}
                    onClick={() => {
                      if (selectedOrderId) {
                        handleAssignDriver(selectedOrderId, driver.id);
                      }
                    }}
                    className="w-full text-left p-3 hover:bg-gray-50 rounded-lg border flex items-center justify-between"
                  >
                    <div>
                      <p className="font-medium">
                        {driver.firstName} {driver.lastName}
                      </p>
                      <p className="text-xs text-gray-500">
                        {driver.vehicleType || "N/A"}
                      </p>
                    </div>
                    <Badge className="bg-green-100 text-green-800">
                      {driver.status}
                    </Badge>
                  </button>
                ))
              )}
            </div>
            <button
              onClick={() => {
                setShowAssignModal(false);
                setSelectedOrderId(null);
              }}
              className="mt-4 w-full px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      {/* Add Company Modal - Brand Owner Registration Only with all required fields */}
      {showAddCompanyModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="text-xl font-semibold">Register Brand Owner</h3>
                <p className="text-sm text-gray-500">
                  Create a new brand owner account with company
                </p>
              </div>
              <button
                onClick={() => {
                  setShowAddCompanyModal(false);
                  setBrandOwnerFormData({
                    firstName: "",
                    surName: "",
                    email: "",
                    password: "",
                    confirmPassword: "",
                    sex: "Male",
                    phoneNo: "",
                    address: "",
                    staffCode: "",
                    companyName: "",
                    companyUserName: "",
                    companyType: [],
                    state: "",
                    branch: "",
                    locality: "",
                  });
                }}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>

            <form onSubmit={handleAddCompany}>
              <div className="space-y-4">
                <div className="border-b pb-2">
                  <h4 className="font-medium text-gray-900">Personal Information</h4>
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="text-sm font-medium text-gray-700">First Name *</label>
                    <input
                      type="text"
                      name="firstName"
                      value={brandOwnerFormData.firstName}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter first name"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Surname *</label>
                    <input
                      type="text"
                      name="surName"
                      value={brandOwnerFormData.surName}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter surname"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Email *</label>
                    <input
                      type="email"
                      name="email"
                      value={brandOwnerFormData.email}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="email@example.com"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Phone Number *</label>
                    <input
                      type="tel"
                      name="phoneNo"
                      value={brandOwnerFormData.phoneNo}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="+2348012345678"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Password *</label>
                    <input
                      type="password"
                      name="password"
                      value={brandOwnerFormData.password}
                      onChange={handleBrandOwnerInputChange}
                      required
                      minLength={8}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Min 8 characters"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Confirm Password *</label>
                    <input
                      type="password"
                      name="confirmPassword"
                      value={brandOwnerFormData.confirmPassword}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Confirm password"
                    />
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Sex</label>
                    <select
                      name="sex"
                      value={brandOwnerFormData.sex}
                      onChange={handleBrandOwnerInputChange}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                    >
                      <option value="Male">Male</option>
                      <option value="Female">Female</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>
                  
                  <div>
                    <label className="text-sm font-medium text-gray-700">Staff Code</label>
                    <input
                      type="text"
                      name="staffCode"
                      value={brandOwnerFormData.staffCode}
                      onChange={handleBrandOwnerInputChange}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Optional staff code"
                    />
                  </div>
                </div>
                
                <div className="border-b pb-2 mt-4">
                  <h4 className="font-medium text-gray-900">Company Information</h4>
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-gray-700">Company Name *</label>
                    <input
                      type="text"
                      name="companyName"
                      value={brandOwnerFormData.companyName}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter company name"
                    />
                  </div>
                  
                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-gray-700">Company Username</label>
                    <input
                      type="text"
                      name="companyUserName"
                      value={brandOwnerFormData.companyUserName}
                      onChange={handleBrandOwnerInputChange}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Company username (optional)"
                    />
                  </div>
                  
                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-gray-700">Address</label>
                    <input
                      type="text"
                      name="address"
                      value={brandOwnerFormData.address}
                      onChange={handleBrandOwnerInputChange}
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Company address"
                    />
                  </div>

                  {/* New Required Fields */}
                  <div>
                    <label className="text-sm font-medium text-gray-700">State *</label>
                    <input
                      type="text"
                      name="state"
                      value={brandOwnerFormData.state}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter state"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-gray-700">Branch *</label>
                    <input
                      type="text"
                      name="branch"
                      value={brandOwnerFormData.branch}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter branch number"
                    />
                  </div>

                  <div>
                    <label className="text-sm font-medium text-gray-700">Locality *</label>
                    <input
                      type="text"
                      name="locality"
                      value={brandOwnerFormData.locality}
                      onChange={handleBrandOwnerInputChange}
                      required
                      className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                      placeholder="Enter locality"
                    />
                  </div>
                  
                  <div className="md:col-span-2">
                    <label className="text-sm font-medium text-gray-700 block mb-2">Company Type *</label>
                    <div className="flex flex-wrap gap-2">
                      {["Accomodations", "Logistics", "Restaurant", "Retail", "E-commerce", "Transport"].map((type) => (
                        <button
                          key={type}
                          type="button"
                          onClick={() => handleCompanyTypeToggle(type)}
                          className={`px-4 py-2 rounded-lg border text-sm transition-colors ${
                            brandOwnerFormData.companyType.includes(type)
                              ? "bg-primary text-white border-primary"
                              : "bg-white text-gray-700 border-gray-300 hover:bg-gray-50"
                          }`}
                        >
                          {type}
                        </button>
                      ))}
                    </div>
                    <p className="text-xs text-gray-500 mt-1">
                      Selected: {brandOwnerFormData.companyType.join(", ") || "None"}
                    </p>
                  </div>
                </div>
              </div>

              <div className="mt-6 flex gap-3">
                <Button
                  type="submit"
                  className="flex-1 gap-2"
                  disabled={isSubmittingCompany}
                >
                  {isSubmittingCompany ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <UserPlus className="h-4 w-4" />
                  )}
                  {isSubmittingCompany ? "Registering..." : "Register Brand Owner"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setShowAddCompanyModal(false);
                    setBrandOwnerFormData({
                      firstName: "",
                      surName: "",
                      email: "",
                      password: "",
                      confirmPassword: "",
                      sex: "Male",
                      phoneNo: "",
                      address: "",
                      staffCode: "",
                      companyName: "",
                      companyUserName: "",
                      companyType: [],
                      state: "",
                      branch: "",
                      locality: "",
                    });
                  }}
                  className="flex-1"
                >
                  Cancel
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Company Details Modal */}
      {showCompanyDetailsModal && selectedCompany && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="h-16 w-16 rounded-lg bg-primary/10 flex items-center justify-center overflow-hidden">
                  {selectedCompany.companyLogo ? (
                    <img
                      src={selectedCompany.companyLogo}
                      alt={selectedCompany.companyName}
                      className="h-full w-full object-cover"
                    />
                  ) : (
                    <Building2 className="h-8 w-8 text-primary" />
                  )}
                </div>
                <div>
                  <h3 className="text-xl font-semibold">
                    {selectedCompany.companyName}
                  </h3>
                  <div className="flex items-center gap-2 mt-1">
                    {getCompanyStatusBadge(selectedCompany.companyStatus)}
                    {selectedCompany.rating && (
                      <span className="flex items-center text-yellow-600">
                        <Star className="h-4 w-4 fill-yellow-500 text-yellow-500 mr-1" />
                        {selectedCompany.rating}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <button
                onClick={() => {
                  setShowCompanyDetailsModal(false);
                  setSelectedCompany(null);
                }}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
              <div>
                <h4 className="font-medium text-gray-900 mb-2">
                  Company Information
                </h4>
                <div className="space-y-2 text-sm">
                  <p>
                    <span className="text-gray-500">Name:</span>{" "}
                    {selectedCompany.companyName}
                  </p>
                  <p>
                    <span className="text-gray-500">Email:</span>{" "}
                    {selectedCompany.companyEmail || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">Phone:</span>{" "}
                    {selectedCompany.phoneNumber || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">Address:</span>{" "}
                    {selectedCompany.companyAddress || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">State:</span>{" "}
                    {selectedCompany.state || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">Locality:</span>{" "}
                    {selectedCompany.locality || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">Value Charge:</span> ₦
                    {selectedCompany.valueCharge?.toLocaleString() || "0"}
                  </p>
                  <p>
                    <span className="text-gray-500">Info:</span>{" "}
                    {selectedCompany.companyInfo || "N/A"}
                  </p>
                  <p>
                    <span className="text-gray-500">Trucks:</span>{" "}
                    {selectedCompany.noOfTrucks || 0}
                  </p>
                  <p>
                    <span className="text-gray-500">Bikes:</span>{" "}
                    {selectedCompany.nofOfBikes || 0}
                  </p>
                  {selectedCompany.bankName && (
                    <>
                      <p>
                        <span className="text-gray-500">Bank:</span>{" "}
                        {selectedCompany.bankName}
                      </p>
                      <p>
                        <span className="text-gray-500">Account Name:</span>{" "}
                        {selectedCompany.accountName}
                      </p>
                      <p>
                        <span className="text-gray-500">Account Number:</span>{" "}
                        {selectedCompany.accountNumber}
                      </p>
                    </>
                  )}
                  <p>
                    <span className="text-gray-500">Joined:</span>{" "}
                    {selectedCompany.createdAt
                      ? new Date(
                          selectedCompany.createdAt
                        ).toLocaleDateString()
                      : "N/A"}
                  </p>
                </div>
              </div>

              <div>
                <h4 className="font-medium text-gray-900 mb-2">
                  Company Statistics
                </h4>
                {companyStatsData ? (
                  <div className="grid grid-cols-2 gap-3">
                    <Card>
                      <CardContent className="p-3 text-center">
                        <p className="text-2xl font-bold text-primary">
                          {companyStatsData.totalOrders || 0}
                        </p>
                        <p className="text-xs text-gray-500">Total Orders</p>
                      </CardContent>
                    </Card>
                    <Card>
                      <CardContent className="p-3 text-center">
                        <p className="text-2xl font-bold text-green-600">
                          ₦
                          {companyStatsData.totalRevenue?.toLocaleString() ||
                            0}
                        </p>
                        <p className="text-xs text-gray-500">Total Revenue</p>
                      </CardContent>
                    </Card>
                    <Card>
                      <CardContent className="p-3 text-center">
                        <p className="text-2xl font-bold text-blue-600">
                          {companyStatsData.activeDrivers || 0}
                        </p>
                        <p className="text-xs text-gray-500">Active Drivers</p>
                      </CardContent>
                    </Card>
                    <Card>
                      <CardContent className="p-3 text-center">
                        <p className="text-2xl font-bold text-purple-600">
                          {companyStatsData.pendingBookings || 0}
                        </p>
                        <p className="text-xs text-gray-500">Pending Bookings</p>
                      </CardContent>
                    </Card>
                  </div>
                ) : (
                  <p className="text-gray-500 text-sm">No stats available</p>
                )}

                {selectedCompany.deliveryTypes &&
                  selectedCompany.deliveryTypes.length > 0 && (
                    <div className="mt-4">
                      <p className="text-sm font-medium text-gray-700">
                        Delivery Types:
                      </p>
                      <div className="flex flex-wrap gap-1 mt-1">
                        {selectedCompany.deliveryTypes.map((type, idx) => (
                          <Badge
                            key={idx}
                            className="bg-blue-100 text-blue-800"
                          >
                            {type}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}
                {selectedCompany.serviceAreas &&
                  selectedCompany.serviceAreas.length > 0 && (
                    <div className="mt-2">
                      <p className="text-sm font-medium text-gray-700">
                        Service Areas:
                      </p>
                      <div className="flex flex-wrap gap-1 mt-1">
                        {selectedCompany.serviceAreas.map((area, idx) => (
                          <Badge
                            key={idx}
                            className="bg-green-100 text-green-800"
                          >
                            {area}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  )}

                <div className="mt-4">
                  <h5 className="font-medium text-gray-900 mb-2">
                    Company Drivers
                  </h5>
                  {companyDrivers.length === 0 ? (
                    <p className="text-gray-500 text-sm">No drivers assigned</p>
                  ) : (
                    <div className="space-y-2 max-h-40 overflow-y-auto">
                      {companyDrivers.map((driver) => (
                        <div
                          key={driver.id}
                          className="flex items-center justify-between p-2 bg-gray-50 rounded-lg"
                        >
                          <div>
                            <p className="text-sm font-medium">
                              {driver.firstName} {driver.lastName}
                            </p>
                            <p className="text-xs text-gray-500">
                              {driver.email}
                            </p>
                          </div>
                          <Badge
                            className={
                              driver.status === "Available"
                                ? "bg-green-100 text-green-800"
                                : "bg-blue-100 text-blue-800"
                            }
                          >
                            {driver.status}
                          </Badge>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="mt-6 pt-4 border-t flex gap-3 flex-wrap">
              {getCompanyStatusActions(selectedCompany)}
              <Button
                variant="outline"
                onClick={() => {
                  setShowCompanyDetailsModal(false);
                  setSelectedCompany(null);
                }}
                className="flex-1"
              >
                Close
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}