"use client";

import Link from "next/link";
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
  ArrowDownLeft,
  Mail,
  MessageSquare,
  User
} from "lucide-react";
import { useState } from "react";

// Mock data matching traditional admin dashboard
const statsData = [
  {
    title: "Total Revenue",
    value: "₦12,450,000",
    subValue: "1,234",
    trend: "up",
    color: "success",
  },
  {
    title: "Today's Revenue",
    value: "₦850,000",
    subValue: "85",
    trend: "up",
    color: "success",
  },
  {
    title: "Completed Packages",
    value: "₦8,320,000",
    subValue: "642",
    trend: "down",
    color: "danger",
  },
  {
    title: "Pending Orders",
    value: "₦1,240,000",
    subValue: "156",
    trend: "up",
    color: "success",
  },
];

const recentOrders = [
  {
    id: "GNL-001-2024",
    trackingNum: "TRK-001-ABC",
    itemName: "Electronics Package",
    senderName: "John Doe",
    receiverName: "Jane Smith",
    companyName: "Swift Logistics",
    shippingCost: 3500,
    itemCost: 150000,
    status: "Delivered",
    date: "2024-01-15",
  },
  {
    id: "GNL-002-2024",
    trackingNum: "TRK-002-DEF",
    itemName: "Documents",
    senderName: "Mike Johnson",
    receiverName: "Sarah Williams",
    companyName: "Express Delivery",
    shippingCost: 2000,
    itemCost: 0,
    status: "In Transit",
    date: "2024-01-14",
  },
  {
    id: "GNL-003-2024",
    trackingNum: "TRK-003-GHI",
    itemName: "Fashion Items",
    senderName: "Alice Brown",
    receiverName: "Bob Davis",
    companyName: "Prime Couriers",
    shippingCost: 4500,
    itemCost: 75000,
    status: "Pending",
    date: "2024-01-13",
  },
  {
    id: "GNL-004-2024",
    trackingNum: "TRK-004-JKL",
    itemName: "Food Items",
    senderName: "Charlie Wilson",
    receiverName: "Diana Moore",
    companyName: "Trusty Transport",
    shippingCost: 2800,
    itemCost: 25000,
    status: "Delivered",
    date: "2024-01-12",
  },
];

const recentReservations = [
  {
    id: "RES-001",
    accommodationName: "Grand Hotel",
    customerName: "John Doe",
    ticketNum: "TCK-001",
    startDate: "2024-02-15",
    endDate: "2024-02-20",
    totalCost: 125000,
    noOfDays: 5,
    roomNumber: "101",
    date: "2024-01-10",
  },
  {
    id: "RES-002",
    accommodationName: "Sunset Apartments",
    customerName: "Jane Smith",
    ticketNum: "TCK-002",
    startDate: "2024-03-10",
    endDate: "2024-03-12",
    totalCost: 70000,
    noOfDays: 2,
    roomNumber: "205",
    date: "2024-01-09",
  },
  {
    id: "RES-003",
    accommodationName: "Beach Resort",
    customerName: "Mike Johnson",
    ticketNum: "TCK-003",
    startDate: "2024-02-25",
    endDate: "2024-03-01",
    totalCost: 275000,
    noOfDays: 4,
    roomNumber: "301",
    date: "2024-01-08",
  },
];

const navItems = [
  { icon: LayoutDashboard, label: "Dashboard", href: "/admin-dashboard", active: true },
  { icon: Package, label: "Orders", href: "#" },
  { icon: Hotel, label: "Bookings", href: "#" },
  { icon: Users, label: "Users", href: "#" },
  { icon: Mail, label: "Messages", href: "#" },
  { icon: Settings, label: "Settings", href: "#" },
];

export default function AdminDashboard() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);

  const getStatusBadge = (status: string) => {
    const variants: Record<string, string> = {
      Delivered: "bg-green-100 text-green-800 border-green-200",
      "In Transit": "bg-blue-100 text-blue-800 border-blue-200",
      Pending: "bg-yellow-100 text-yellow-800 border-yellow-200",
    };
    return variants[status] || "bg-gray-100 text-gray-800";
  };

  return (
    <div className="min-h-screen bg-gray-50 flex">
      {/* Sidebar */}
      <aside 
        className={`bg-gray-900 text-white transition-all duration-300 ${
          isSidebarOpen ? "w-64" : "w-20"
        }} fixed h-full z-40`}
      >
        {/* Logo */}
        <div className="h-16 flex items-center justify-center border-b border-gray-800">
          <Link href="/admin-dashboard" className="text-xl font-bold">
            {isSidebarOpen ? "GINILOG Admin" : "GNL"}
          </Link>
        </div>

        {/* Navigation */}
        <nav className="mt-6 px-3">
          {navItems.map((item) => (
            <Link
              key={item.label}
              href={item.href}
              className={`flex items-center gap-3 px-3 py-3 rounded-lg mb-1 transition-colors ${
                item.active 
                  ? "bg-primary text-white" 
                  : "text-gray-400 hover:bg-gray-800 hover:text-white"
              }`}
            >
              <item.icon className="h-5 w-5 flex-shrink-0" />
              {isSidebarOpen && <span className="text-sm font-medium">{item.label}</span>}
            </Link>
          ))}
        </nav>

        {/* Logout */}
        <div className="absolute bottom-0 left-0 right-0 p-3 border-t border-gray-800">
          <Link
            href="/login"
            className="flex items-center gap-3 px-3 py-3 rounded-lg text-gray-400 hover:bg-gray-800 hover:text-white transition-colors"
          >
            <LogOut className="h-5 w-5 flex-shrink-0" />
            {isSidebarOpen && <span className="text-sm font-medium">Logout</span>}
          </Link>
        </div>
      </aside>

      {/* Main Content */}
      <div className={`flex-1 transition-all duration-300 ${isSidebarOpen ? "ml-64" : "ml-20"}`}>
        {/* Top Header */}
        <header className="bg-white border-b sticky top-0 z-30">
          <div className="flex items-center justify-between px-6 py-4">
            <div className="flex items-center gap-4">
              <button
                onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <LayoutDashboard className="h-5 w-5 text-gray-600" />
              </button>
              <div className="relative hidden sm:block">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <input
                  type="text"
                  placeholder="Search..."
                  className="pl-10 pr-4 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-64"
                />
              </div>
            </div>

            <div className="flex items-center gap-4">
              <button className="relative p-2 hover:bg-gray-100 rounded-lg">
                <Bell className="h-5 w-5 text-gray-600" />
                <span className="absolute top-1 right-1 h-2 w-2 bg-primary rounded-full" />
              </button>
              <button className="relative p-2 hover:bg-gray-100 rounded-lg">
                <MessageSquare className="h-5 w-5 text-gray-600" />
                <span className="absolute top-1 right-1 h-2 w-2 bg-primary rounded-full" />
              </button>
              <div className="flex items-center gap-3 pl-4 border-l">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                  <User className="h-5 w-5 text-primary" />
                </div>
                <div className="hidden md:block">
                  <p className="text-sm font-medium text-gray-900">Admin User</p>
                  <Link href="/admin-dashboard/profile" className="text-xs text-gray-500 hover:text-primary">Super Admin</Link>
                </div>
              </div>
            </div>
          </div>
        </header>

        {/* Dashboard Content */}
        <main className="p-6">
          {/* Action Buttons */}
          <div className="flex flex-wrap gap-3 mb-6">
            <Button className="bg-primary hover:bg-primary/90">
              <Mail className="h-4 w-4 mr-2" />
              Send Email
            </Button>
            <Button variant="outline" className="border-green-500 text-green-600 hover:bg-green-50">
              <MessageSquare className="h-4 w-4 mr-2" />
              Feedback
            </Button>
            <Button variant="outline">
              <Users className="h-4 w-4 mr-2" />
              View Staff
            </Button>
          </div>

          {/* Stats Cards */}
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            {statsData.map((stat, index) => (
              <Card key={index} className="hover:shadow-md transition-shadow">
                <CardContent className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-baseline gap-2">
                        <h3 className="text-2xl font-bold text-gray-900">{stat.value}</h3>
                        <span className={`text-sm font-medium ${
                          stat.trend === "up" ? "text-green-600" : "text-red-600"
                        }`}>
                          {stat.subValue}
                        </span>
                      </div>
                      <p className="text-gray-500 text-sm mt-1">{stat.title}</p>
                    </div>
                    <div className={`p-3 rounded-lg ${
                      stat.trend === "up" ? "bg-green-100" : "bg-red-100"
                    }`}>
                      {stat.trend === "up" ? (
                        <ArrowUpRight className={`h-5 w-5 ${
                          stat.trend === "up" ? "text-green-600" : "text-red-600"
                        }`} />
                      ) : (
                        <ArrowDownLeft className="h-5 w-5 text-red-600" />
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {/* Orders Table */}
          <Card className="mb-8">
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Order List</CardTitle>
              <Button variant="outline" size="sm" className="border-green-500 text-green-600 hover:bg-green-50">
                View All Orders
              </Button>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b">
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Tracking Num</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Item Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Sender Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Receiver Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Company Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Shipping Cost</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Item Cost</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Order Status</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Date</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentOrders.map((order) => (
                      <tr key={order.id} className="border-b hover:bg-gray-50">
                        <td className="py-3 px-4 text-sm">{order.trackingNum}</td>
                        <td className="py-3 px-4 text-sm">{order.itemName}</td>
                        <td className="py-3 px-4 text-sm">{order.senderName}</td>
                        <td className="py-3 px-4 text-sm">{order.receiverName}</td>
                        <td className="py-3 px-4 text-sm">{order.companyName}</td>
                        <td className="py-3 px-4 text-sm">₦{order.shippingCost.toLocaleString()}</td>
                        <td className="py-3 px-4 text-sm">₦{order.itemCost.toLocaleString()}</td>
                        <td className="py-3 px-4">
                          <Badge className={getStatusBadge(order.status)}>
                            {order.status}
                          </Badge>
                        </td>
                        <td className="py-3 px-4 text-sm">{order.date}</td>
                        <td className="py-3 px-4">
                          <div className="flex gap-2">
                            <Button variant="outline" size="sm" className="text-blue-600 border-blue-200 hover:bg-blue-50">
                              View
                            </Button>
                            <Button variant="outline" size="sm" className="text-red-600 border-red-200 hover:bg-red-50">
                              Delete
                            </Button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </CardContent>
          </Card>

          {/* Customer Reservations Table */}
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Customer Reservation List</CardTitle>
              <Button variant="outline" size="sm" className="border-green-500 text-green-600 hover:bg-green-50">
                View All Reservations
              </Button>
            </CardHeader>
            <CardContent>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b">
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Accommodation Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Customer Name</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Ticket Num</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Start Date</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">End Date</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Total Cost</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">No Of Days</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Room Number</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Date</th>
                      <th className="text-left py-3 px-4 text-sm font-medium text-gray-600">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentReservations.map((reservation) => (
                      <tr key={reservation.id} className="border-b hover:bg-gray-50">
                        <td className="py-3 px-4 text-sm">{reservation.accommodationName}</td>
                        <td className="py-3 px-4 text-sm">{reservation.customerName}</td>
                        <td className="py-3 px-4 text-sm">{reservation.ticketNum}</td>
                        <td className="py-3 px-4 text-sm">{reservation.startDate}</td>
                        <td className="py-3 px-4 text-sm">{reservation.endDate}</td>
                        <td className="py-3 px-4 text-sm font-medium text-primary">
                          ₦{reservation.totalCost.toLocaleString()}
                        </td>
                        <td className="py-3 px-4 text-sm">{reservation.noOfDays}</td>
                        <td className="py-3 px-4">
                          <Badge variant="outline" className="text-green-600 border-green-200">
                            {reservation.roomNumber}
                          </Badge>
                        </td>
                        <td className="py-3 px-4 text-sm">{reservation.date}</td>
                        <td className="py-3 px-4">
                          <Button variant="outline" size="sm" className="text-blue-600 border-blue-200 hover:bg-blue-50">
                            View
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </CardContent>
          </Card>
        </main>
      </div>
    </div>
  );
}
