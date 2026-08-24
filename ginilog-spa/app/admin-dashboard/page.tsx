// app/admin-dashboard/page.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  LayoutDashboard,
  Users,
  Building2,
  ShoppingBag,
  Calendar,
  Wallet,
  Settings,
  Shield,
  Megaphone,
  FileCheck,
  UserCog,
  LogOut,
  Menu,
  X,
  ChevronDown,
  Home,
  Bell,
  Package,
  Truck,
  Hotel,
  DollarSign,
  TrendingUp,
  ArrowUpRight,
  ArrowDownRight,
  Clock,
  CheckCircle,
  AlertCircle,
  BarChart3
} from "lucide-react";
import { 
  adminGetProfile,
  getAllAdmins,
  getBrandOwners, 
  getAllAdverts, 
  getNotifications,
  getPayouts,
  getCompanyApplications,
  getCompanies,
  getPackageOrders,
  getAccommodationReservations,
  getAllUsers,
  isAuthenticated,
  getStoredUser,
  logout,
  validateSession
} from "@/lib/api";

export default function AdminDashboard() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);
  const [adminProfile, setAdminProfile] = useState<any>(null);
  const [usersList, setUsersList] = useState<any[]>([]);
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalCompanies: 0,
    totalOrders: 0,
    totalBookings: 0,
    totalAdmins: 0,
    totalAdverts: 0,
    pendingApplications: 0,
    totalRevenue: 0,
  });
  const [recentActivities, setRecentActivities] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated() || !validateSession()) {
      router.push("/admin-dashboard/login");
      return;
    }
    fetchDashboardData();
  }, [router]);

  const fetchDashboardData = async () => {
    setIsLoading(true);
    setError(null);
    try {
      // Fetch all data in parallel
      const [
        profile,
        admins,
        users,
        companies,
        orders,
        bookings,
        adverts,
        applications,
        notifications,
        payouts
      ] = await Promise.all([
        adminGetProfile().catch(() => null),
        getAllAdmins().catch(() => []),
        getAllUsers().catch(() => []),
        getCompanies().catch(() => []),
        getPackageOrders().catch(() => []),
        getAccommodationReservations().catch(() => []),
        getAllAdverts().catch(() => []),
        getCompanyApplications().catch(() => []),
        getNotifications().catch(() => []),
        getPayouts().catch(() => []),
      ]);

      setAdminProfile(profile);
      setUsersList(users || []);

      // Calculate stats
      const totalRevenue = orders.reduce((sum: number, order: any) => 
        sum + (order.itemCost || 0) + (order.shippingCost || 0), 0
      ) + bookings.reduce((sum: number, booking: any) => 
        sum + (booking.totalAmount || 0), 0
      );

      setStats({
        totalUsers: users?.length || 0,
        totalCompanies: companies?.length || 0,
        totalOrders: orders?.length || 0,
        totalBookings: bookings?.length || 0,
        totalAdmins: admins?.length || 0,
        totalAdverts: adverts?.length || 0,
        pendingApplications: applications?.filter((a: any) => a.status === "pending").length || 0,
        totalRevenue: totalRevenue,
      });

      // Recent activities (combine recent notifications and applications)
      const recentNotifications = (notifications || []).slice(0, 3).map((n: any) => ({
        id: n.id,
        type: "notification",
        title: n.title,
        description: n.body,
        time: n.createdAt,
        icon: Bell,
        iconColor: "text-blue-500",
        bgColor: "bg-blue-50",
      }));

      const recentApplications = (applications || [])
        .filter((a: any) => a.status === "pending")
        .slice(0, 3)
        .map((a: any) => ({
          id: a.id,
          type: "application",
          title: "New Company Application",
          description: `${a.companyName} is requesting approval`,
          time: a.createdAt,
          icon: FileCheck,
          iconColor: "text-yellow-500",
          bgColor: "bg-yellow-50",
        }));

      setRecentActivities([...recentNotifications, ...recentApplications]);

    } catch (err) {
      console.error("❌ Failed to fetch dashboard data:", err);
      setError("Failed to load dashboard data. Please refresh.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = async () => {
    await logout();
    router.push("/admin-dashboard/login");
  };

  const statCards = [
    {
      title: "Total Users",
      value: stats.totalUsers,
      icon: Users,
      color: "bg-blue-50 text-blue-600",
      trend: "+12%",
      trendUp: true,
    },
    {
      title: "Companies",
      value: stats.totalCompanies,
      icon: Building2,
      color: "bg-purple-50 text-purple-600",
      trend: "+5%",
      trendUp: true,
    },
    {
      title: "Orders",
      value: stats.totalOrders,
      icon: ShoppingBag,
      color: "bg-orange-50 text-orange-600",
      trend: "+8%",
      trendUp: true,
    },
    {
      title: "Bookings",
      value: stats.totalBookings,
      icon: Calendar,
      color: "bg-green-50 text-green-600",
      trend: "+3%",
      trendUp: true,
    },
    {
      title: "Revenue",
      value: `₦${(stats.totalRevenue / 1000000).toFixed(1)}M`,
      icon: DollarSign,
      color: "bg-emerald-50 text-emerald-600",
      trend: "+15%",
      trendUp: true,
    },
    {
      title: "Pending Applications",
      value: stats.pendingApplications,
      icon: FileCheck,
      color: "bg-yellow-50 text-yellow-600",
      trend: `${stats.pendingApplications} pending`,
      trendUp: false,
    },
  ];

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <div className="text-center">
          <div className="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="mt-4 text-gray-500">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-sm text-gray-500 mt-1">
            Welcome back, {adminProfile?.firstName || "Admin"}! Here's what's happening.
          </p>
        </div>
        <div className="flex gap-2">
          <Button onClick={fetchDashboardData} variant="outline" size="sm">
            Refresh
          </Button>
        </div>
      </div>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 flex items-start gap-3">
          <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <p className="text-sm">{error}</p>
        </div>
      )}

      {/* Stats Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        {statCards.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index} className="hover:shadow-md transition-shadow">
              <CardContent className="p-4">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-xs text-gray-500 font-medium">{stat.title}</p>
                    <p className="text-xl font-bold text-gray-900 mt-1">{stat.value}</p>
                    {stat.trend && (
                      <div className="flex items-center gap-1 mt-1">
                        {stat.trendUp ? (
                          <ArrowUpRight className="h-3 w-3 text-green-500" />
                        ) : (
                          <ArrowDownRight className="h-3 w-3 text-red-500" />
                        )}
                        <span className={`text-xs ${stat.trendUp ? 'text-green-600' : 'text-red-600'}`}>
                          {stat.trend}
                        </span>
                      </div>
                    )}
                  </div>
                  <div className={`p-2 rounded-lg ${stat.color}`}>
                    <Icon className="h-5 w-5" />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <Link
          href="/admin-dashboard/applications"
          className="p-4 bg-white rounded-xl border border-gray-200 hover:shadow-md transition-all text-center"
        >
          <FileCheck className="h-6 w-6 text-primary mx-auto mb-2" />
          <p className="text-sm font-medium text-gray-900">Applications</p>
          {stats.pendingApplications > 0 && (
            <Badge className="mt-1 bg-yellow-500 text-white">
              {stats.pendingApplications} pending
            </Badge>
          )}
        </Link>
        <Link
          href="/admin-dashboard/admins"
          className="p-4 bg-white rounded-xl border border-gray-200 hover:shadow-md transition-all text-center"
        >
          <Shield className="h-6 w-6 text-primary mx-auto mb-2" />
          <p className="text-sm font-medium text-gray-900">Admins</p>
          <p className="text-xs text-gray-500">{stats.totalAdmins} total</p>
        </Link>
        <Link
          href="/admin-dashboard/users"
          className="p-4 bg-white rounded-xl border border-gray-200 hover:shadow-md transition-all text-center"
        >
          <Users className="h-6 w-6 text-primary mx-auto mb-2" />
          <p className="text-sm font-medium text-gray-900">Users</p>
          <p className="text-xs text-gray-500">{stats.totalUsers} total</p>
        </Link>
        <Link
          href="/admin-dashboard/companies"
          className="p-4 bg-white rounded-xl border border-gray-200 hover:shadow-md transition-all text-center"
        >
          <Building2 className="h-6 w-6 text-primary mx-auto mb-2" />
          <p className="text-sm font-medium text-gray-900">Companies</p>
          <p className="text-xs text-gray-500">{stats.totalCompanies} registered</p>
        </Link>
      </div>

      {/* Recent Users Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="text-base">Recent Users</CardTitle>
            <Link href="/admin-dashboard/users">
              <Button variant="ghost" size="sm">View All</Button>
            </Link>
          </div>
        </CardHeader>
        <CardContent>
          {usersList.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              <Users className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>No users found</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b">
                    <th className="text-left py-3 px-4 font-medium text-gray-500">User</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-500">Email</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-500">Phone</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-500">Type</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-500">Status</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-500">Joined</th>
                  </tr>
                </thead>
                <tbody>
                  {usersList.slice(0, 5).map((user: any) => (
                    <tr key={user.id || user.userId} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-2">
                          {user.profilePicture || user.profileImage ? (
                            <img 
                              src={user.profilePicture || user.profileImage} 
                              alt={user.firstName} 
                              className="w-8 h-8 rounded-full object-cover"
                            />
                          ) : (
                            <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary font-semibold">
                              {(user.firstName?.[0] || user.email?.[0] || 'U').toUpperCase()}
                            </div>
                          )}
                          <span className="font-medium">
                            {user.firstName || ''} {user.lastName || user.surName || ''}
                          </span>
                        </div>
                      </td>
                      <td className="py-3 px-4 text-gray-600">{user.email || 'N/A'}</td>
                      <td className="py-3 px-4 text-gray-600">{user.phoneNo || user.phoneNumber || 'N/A'}</td>
                      <td className="py-3 px-4">
                        <Badge variant="outline">
                          {user.userType || user.staffType || 'User'}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <Badge className={user.userStatus !== false ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}>
                          {user.userStatus !== false ? 'Active' : 'Inactive'}
                        </Badge>
                      </td>
                      <td className="py-3 px-4 text-gray-500 text-xs">
                        {user.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Recent Activity */}
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Recent Activity</CardTitle>
        </CardHeader>
        <CardContent>
          {recentActivities.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              <Bell className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>No recent activity</p>
            </div>
          ) : (
            <div className="space-y-4">
              {recentActivities.map((activity) => {
                const Icon = activity.icon;
                return (
                  <div key={activity.id} className="flex items-start gap-4 p-3 rounded-lg hover:bg-gray-50 transition-colors">
                    <div className={`p-2 rounded-full ${activity.bgColor}`}>
                      <Icon className={`h-4 w-4 ${activity.iconColor}`} />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-900">{activity.title}</p>
                      <p className="text-sm text-gray-500">{activity.description}</p>
                      <p className="text-xs text-gray-400 mt-1">
                        {activity.time ? new Date(activity.time).toLocaleString() : "Just now"}
                      </p>
                    </div>
                    <Badge variant="outline" className="flex-shrink-0">
                      {activity.type === "notification" ? "Info" : "Action Required"}
                    </Badge>
                  </div>
                );
              })}
            </div>
          )}
        </CardContent>
      </Card>

      {/* System Status */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="h-3 w-3 rounded-full bg-green-500 animate-pulse" />
              <div>
                <p className="text-sm font-medium text-gray-900">System Status</p>
                <p className="text-xs text-gray-500">All systems operational</p>
              </div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="h-3 w-3 rounded-full bg-blue-500" />
              <div>
                <p className="text-sm font-medium text-gray-900">API Status</p>
                <p className="text-xs text-gray-500">Connected and healthy</p>
              </div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="h-3 w-3 rounded-full bg-yellow-500" />
              <div>
                <p className="text-sm font-medium text-gray-900">Pending Tasks</p>
                <p className="text-xs text-gray-500">{stats.pendingApplications} applications to review</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}