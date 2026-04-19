"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { 
  Search, 
  Package, 
  Home, 
  Truck, 
  User, 
  LogOut, 
  Bell, 
  MapPin, 
  Calendar,
  CreditCard,
  ChevronRight,
  Star,
  Clock,
  CheckCircle,
  TruckIcon,
  Building2
} from "lucide-react";
import { Label } from "@/components/ui/label";

// Mock user data - in real app, this would come from authentication
const mockUser = {
  id: "1",
  firstName: "John",
  lastName: "Doe",
  email: "john.doe@example.com",
  userType: "User",
  avatar: null,
};

// Mock data for accommodation customers
const accommodationBookings = [
  {
    id: "1",
    accommodationName: "Grand Hotel",
    roomType: "Deluxe Suite",
    checkIn: "2024-02-15",
    checkOut: "2024-02-20",
    status: "confirmed",
    amount: 125000,
    location: "Victoria Island, Lagos",
    image: "/service-1.jpg",
  },
  {
    id: "2",
    accommodationName: "Sunset Apartments",
    roomType: "Standard Room",
    checkIn: "2024-03-10",
    checkOut: "2024-03-12",
    status: "pending",
    amount: 70000,
    location: "Maitama, Abuja",
    image: "/service-2.jpg",
  },
];

const recommendedAccommodations = [
  {
    id: "1",
    name: "Beach Resort",
    type: "Resort",
    location: "Calabar, Nigeria",
    price: 55000,
    rating: 4.8,
    image: "/service-3.jpg",
  },
  {
    id: "2",
    name: "City Lodge",
    type: "Hotel",
    location: "Ikeja, Lagos",
    price: 35000,
    rating: 4.2,
    image: "/service-4.jpg",
  },
  {
    id: "3",
    name: "Mountain View",
    type: "Resort",
    location: "Jos, Plateau",
    price: 45000,
    rating: 4.5,
    image: "/service-5.jpg",
  },
];

// Mock data for logistics customers
const logisticsOrders = [
  {
    id: "GNL-001-2024",
    sender: "John Doe",
    receiver: "Jane Smith",
    from: "Lagos, Nigeria",
    to: "Abuja, Nigeria",
    status: "in_transit",
    date: "2024-01-15",
    amount: 3500,
    company: "Swift Logistics",
  },
  {
    id: "GNL-002-2024",
    sender: "John Doe",
    receiver: "Mike Johnson",
    from: "Lagos, Nigeria",
    to: "Port Harcourt, Nigeria",
    status: "delivered",
    date: "2024-01-10",
    amount: 2800,
    company: "Express Delivery",
  },
  {
    id: "GNL-003-2024",
    sender: "John Doe",
    receiver: "Sarah Williams",
    from: "Lagos, Nigeria",
    to: "Kano, Nigeria",
    status: "pending",
    date: "2024-01-20",
    amount: 4500,
    company: "Prime Couriers",
  },
];

const logisticsCompanies = [
  { id: "1", name: "Swift Logistics", rating: 4.7, price: 1500 },
  { id: "2", name: "Express Delivery", rating: 4.5, price: 2000 },
  { id: "3", name: "Prime Couriers", rating: 4.8, price: 3000 },
  { id: "4", name: "Trusty Transport", rating: 4.3, price: 1800 },
];

export default function CustomerDashboard() {
  const [user] = useState(mockUser);
  const [trackingNumber, setTrackingNumber] = useState("");

  const getStatusColor = (status: string) => {
    switch (status) {
      case "confirmed":
      case "delivered":
        return "bg-green-100 text-green-800";
      case "pending":
        return "bg-yellow-100 text-yellow-800";
      case "in_transit":
        return "bg-blue-100 text-blue-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case "in_transit":
        return "In Transit";
      default:
        return status.charAt(0).toUpperCase() + status.slice(1);
    }
  };

  const handleTracking = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Tracking:", trackingNumber);
  };

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
            <Link href="/customer-portal" className="text-gray-900 font-medium">
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
                  <User className="h-5 w-5 text-primary" />
                </div>
                <div className="hidden sm:block">
                  <p className="text-sm font-medium text-gray-900">
                    {user.firstName} {user.lastName}
                  </p>
                  <p className="text-xs text-gray-500">GINILOG Customer</p>
                </div>
              </div>

              <Link href="/customer-portal/login">
                <Button variant="ghost" size="icon" className="text-gray-600">
                  <LogOut className="h-5 w-5" />
                </Button>
              </Link>
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
                  <p className="text-white/80 text-sm">Active Bookings</p>
                  <p className="text-3xl font-bold mt-1">2</p>
                </div>
                <Building2 className="h-8 w-8 text-white/80" />
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">Active Orders</p>
                  <p className="text-3xl font-bold mt-1 text-gray-900">3</p>
                </div>
                <Package className="h-8 w-8 text-primary" />
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">Total Spent</p>
                  <p className="text-3xl font-bold mt-1 text-gray-900">₦225,000</p>
                </div>
                <CreditCard className="h-8 w-8 text-green-600" />
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">In Transit</p>
                  <p className="text-3xl font-bold mt-1 text-gray-900">1</p>
                </div>
                <TruckIcon className="h-8 w-8 text-primary" />
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
                  {accommodationBookings.slice(0, 2).map((booking) => (
                    <div key={booking.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50">
                      <div className="h-16 w-16 rounded-lg overflow-hidden flex-shrink-0">
                        <img src={booking.image} alt={booking.accommodationName} className="h-full w-full object-cover" />
                      </div>
                      <div className="flex-1">
                        <h4 className="font-semibold text-gray-900">{booking.accommodationName}</h4>
                        <p className="text-sm text-gray-500">{booking.location}</p>
                        <Badge className={getStatusColor(booking.status)}>{getStatusLabel(booking.status)}</Badge>
                      </div>
                    </div>
                  ))}
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
                  {logisticsOrders.slice(0, 2).map((order) => (
                    <div key={order.id} className="flex items-start gap-3 p-3 border rounded-lg hover:bg-gray-50">
                      <div className="h-12 w-12 bg-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                        <Package className="h-6 w-6 text-primary" />
                      </div>
                      <div className="flex-1">
                        <div className="flex items-center gap-2">
                          <h4 className="font-semibold text-gray-900">{order.id}</h4>
                          <Badge className={getStatusColor(order.status)}>{getStatusLabel(order.status)}</Badge>
                        </div>
                        <p className="text-sm text-gray-500">{order.from} → {order.to}</p>
                      </div>
                    </div>
                  ))}
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
              <div className="space-y-3">
                <div className="flex items-center gap-3 text-sm">
                  <div className="h-8 w-8 rounded-full bg-green-100 flex items-center justify-center">
                    <CheckCircle className="h-4 w-4 text-green-600" />
                  </div>
                  <div>
                    <p className="text-gray-900">Booking confirmed - Grand Hotel</p>
                    <p className="text-gray-500 text-xs">2 hours ago</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 text-sm">
                  <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                    <TruckIcon className="h-4 w-4 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-gray-900">Package in transit - Order #GNL-001</p>
                    <p className="text-gray-500 text-xs">5 hours ago</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 text-sm">
                  <div className="h-8 w-8 rounded-full bg-yellow-100 flex items-center justify-center">
                    <Clock className="h-4 w-4 text-yellow-600" />
                  </div>
                  <div>
                    <p className="text-gray-900">New order pending - Order #GNL-003</p>
                    <p className="text-gray-500 text-xs">1 day ago</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
