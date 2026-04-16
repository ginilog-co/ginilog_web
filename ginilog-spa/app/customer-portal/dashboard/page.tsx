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
  userType: "logistics", // or "accommodation"
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
            {user.userType === "accommodation" ? (
              <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900">
                My Bookings
              </Link>
            ) : (
              <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900">
                My Orders
              </Link>
            )}
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
                  <p className="text-xs text-gray-500 capitalize">{user.userType} Customer</p>
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
            {user.userType === "accommodation" 
              ? "Manage your bookings and find new accommodations."
              : "Track your packages and manage your deliveries."}
          </p>
        </div>

        {/* Quick Actions */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
          {user.userType === "accommodation" ? (
            <>
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
                      <p className="text-gray-500 text-sm">Total Spent</p>
                      <p className="text-3xl font-bold mt-1 text-gray-900">₦195,000</p>
                    </div>
                    <CreditCard className="h-8 w-8 text-primary" />
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-gray-500 text-sm">Upcoming Check-in</p>
                      <p className="text-lg font-bold mt-1 text-gray-900">Feb 15, 2024</p>
                    </div>
                    <Calendar className="h-8 w-8 text-primary" />
                  </div>
                </CardContent>
              </Card>
            </>
          ) : (
            <>
              <Card className="bg-primary text-white">
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-white/80 text-sm">Active Orders</p>
                      <p className="text-3xl font-bold mt-1">3</p>
                    </div>
                    <Package className="h-8 w-8 text-white/80" />
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-gray-500 text-sm">Total Delivered</p>
                      <p className="text-3xl font-bold mt-1 text-gray-900">24</p>
                    </div>
                    <CheckCircle className="h-8 w-8 text-green-600" />
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
            </>
          )}
        </div>

        {user.userType === "accommodation" ? (
          // Accommodation Customer Dashboard
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Main Content */}
            <div className="lg:col-span-2 space-y-8">
              {/* My Bookings */}
              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>My Bookings</CardTitle>
                  <Link href="/customer-portal/orders">
                    <Button variant="outline" size="sm">
                      View All
                    </Button>
                  </Link>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {accommodationBookings.map((booking) => (
                      <div
                        key={booking.id}
                        className="flex items-start gap-4 p-4 border rounded-lg hover:bg-gray-50 transition-colors"
                      >
                        <div className="h-20 w-20 rounded-lg overflow-hidden flex-shrink-0">
                          <img 
                            src={booking.image} 
                            alt={booking.accommodationName} 
                            className="h-full w-full object-cover"
                          />
                        </div>
                        <div className="flex-1">
                          <div className="flex items-start justify-between">
                            <div>
                              <h3 className="font-semibold text-gray-900">{booking.accommodationName}</h3>
                              <p className="text-sm text-gray-500">{booking.roomType}</p>
                              <div className="flex items-center gap-2 mt-1 text-sm text-gray-500">
                                <MapPin className="h-4 w-4" />
                                {booking.location}
                              </div>
                            </div>
                            <Badge className={getStatusColor(booking.status)}>
                              {getStatusLabel(booking.status)}
                            </Badge>
                          </div>
                          <div className="flex items-center justify-between mt-3">
                            <div className="text-sm text-gray-500">
                              <span className="font-medium text-gray-900">
                                {new Date(booking.checkIn).toLocaleDateString()}
                              </span>
                              {" → "}
                              <span className="font-medium text-gray-900">
                                {new Date(booking.checkOut).toLocaleDateString()}
                              </span>
                            </div>
                            <p className="font-semibold text-primary">
                              ₦{booking.amount.toLocaleString()}
                            </p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              {/* Recommended Accommodations */}
              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>Recommended For You</CardTitle>
                  <Link href="#">
                    <Button variant="outline" size="sm">
                      Explore All
                    </Button>
                  </Link>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                    {recommendedAccommodations.map((acc) => (
                      <div
                        key={acc.id}
                        className="border rounded-lg overflow-hidden hover:shadow-md transition-shadow cursor-pointer"
                      >
                        <div className="h-32 overflow-hidden">
                          <img 
                            src={acc.image} 
                            alt={acc.name} 
                            className="h-full w-full object-cover group-hover:scale-110 transition-transform duration-300"
                          />
                        </div>
                        <div className="p-4">
                          <div className="flex items-center gap-1 mb-1">
                            <Star className="h-4 w-4 text-yellow-400 fill-current" />
                            <span className="text-sm font-medium">{acc.rating}</span>
                          </div>
                          <h3 className="font-semibold text-gray-900">{acc.name}</h3>
                          <p className="text-sm text-gray-500">{acc.location}</p>
                          <div className="flex items-center justify-between mt-2">
                            <Badge variant="secondary">{acc.type}</Badge>
                            <p className="text-primary font-semibold">
                              ₦{acc.price.toLocaleString()}
                            </p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Sidebar */}
            <div className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle>Quick Book</CardTitle>
                </CardHeader>
                <CardContent>
                  <form className="space-y-4">
                    <div>
                      <Label className="text-sm text-gray-600">Location</Label>
                      <div className="relative mt-1">
                        <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input placeholder="Where to?" className="pl-10" />
                      </div>
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <Label className="text-sm text-gray-600">Check-in</Label>
                        <Input type="date" />
                      </div>
                      <div>
                        <Label className="text-sm text-gray-600">Check-out</Label>
                        <Input type="date" />
                      </div>
                    </div>
                    <Button className="w-full bg-primary hover:bg-primary/90">
                      Search Accommodations
                    </Button>
                  </form>
                </CardContent>
              </Card>

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
                        <p className="text-gray-900">Booking confirmed</p>
                        <p className="text-gray-500 text-xs">2 hours ago</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 text-sm">
                      <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                        <Star className="h-4 w-4 text-blue-600" />
                      </div>
                      <div>
                        <p className="text-gray-900">Left a review</p>
                        <p className="text-gray-500 text-xs">1 day ago</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        ) : (
          // Logistics Customer Dashboard
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Main Content */}
            <div className="lg:col-span-2 space-y-8">
              {/* Track Package */}
              <Card className="bg-gradient-to-r from-primary to-red-800 text-white">
                <CardContent className="p-6">
                  <h2 className="text-xl font-semibold mb-4">Track Your Package</h2>
                  <form onSubmit={handleTracking} className="flex gap-3">
                    <div className="flex-1 relative">
                      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                      <Input
                        type="text"
                        placeholder="Enter tracking number (e.g., GNL-001-2024)"
                        className="pl-10 h-12 bg-white text-gray-900 border-0"
                        value={trackingNumber}
                        onChange={(e) => setTrackingNumber(e.target.value)}
                      />
                    </div>
                    <Button 
                      type="submit" 
                      className="h-12 px-6 bg-white text-primary hover:bg-gray-100"
                    >
                      Track
                    </Button>
                  </form>
                </CardContent>
              </Card>

              {/* My Orders */}
              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>My Orders</CardTitle>
                  <Link href="/customer-portal/orders">
                    <Button variant="outline" size="sm">
                      View All
                    </Button>
                  </Link>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {logisticsOrders.map((order) => (
                      <div
                        key={order.id}
                        className="flex items-start gap-4 p-4 border rounded-lg hover:bg-gray-50 transition-colors"
                      >
                        <div className="h-12 w-12 bg-primary/10 rounded-lg flex items-center justify-center flex-shrink-0">
                          <Package className="h-6 w-6 text-primary" />
                        </div>
                        <div className="flex-1">
                          <div className="flex items-start justify-between">
                            <div>
                              <div className="flex items-center gap-2">
                                <h3 className="font-semibold text-gray-900">{order.id}</h3>
                                <Badge className={getStatusColor(order.status)}>
                                  {getStatusLabel(order.status)}
                                </Badge>
                              </div>
                              <p className="text-sm text-gray-500">{order.company}</p>
                            </div>
                            <p className="font-semibold text-primary">
                              ₦{order.amount.toLocaleString()}
                            </p>
                          </div>
                          <div className="flex items-center gap-6 mt-2 text-sm text-gray-500">
                            <div className="flex items-center gap-1">
                              <MapPin className="h-4 w-4" />
                              {order.from} → {order.to}
                            </div>
                            <div className="flex items-center gap-1">
                              <Calendar className="h-4 w-4" />
                              {new Date(order.date).toLocaleDateString()}
                            </div>
                          </div>
                          <div className="flex items-center justify-between mt-3">
                            <div className="text-sm text-gray-500">
                              To: <span className="font-medium text-gray-900">{order.receiver}</span>
                            </div>
                            <Link href="#" className="text-primary text-sm hover:underline flex items-center gap-1">
                              Details <ChevronRight className="h-4 w-4" />
                            </Link>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>

              {/* Send New Package */}
              <Card>
                <CardHeader>
                  <CardTitle>Send a New Package</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
                    {logisticsCompanies.map((company) => (
                      <div
                        key={company.id}
                        className="border rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
                      >
                        <div className="h-16 bg-gray-100 rounded-lg flex items-center justify-center mb-3">
                          <Truck className="h-8 w-8 text-primary" />
                        </div>
                        <h3 className="font-semibold text-gray-900 text-center">{company.name}</h3>
                        <div className="flex items-center justify-center gap-1 mt-1">
                          <Star className="h-4 w-4 text-yellow-400 fill-current" />
                          <span className="text-sm">{company.rating}</span>
                        </div>
                        <p className="text-center text-primary font-semibold mt-2">
                          From ₦{company.price.toLocaleString()}
                        </p>
                        <Button className="w-full mt-3 bg-primary hover:bg-primary/90" size="sm">
                          Send
                        </Button>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Sidebar */}
            <div className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle>Quick Send</CardTitle>
                </CardHeader>
                <CardContent>
                  <form className="space-y-4">
                    <div>
                      <Label className="text-sm text-gray-600">From</Label>
                      <div className="relative mt-1">
                        <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input placeholder="Pickup location" className="pl-10" />
                      </div>
                    </div>
                    <div>
                      <Label className="text-sm text-gray-600">To</Label>
                      <div className="relative mt-1">
                        <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input placeholder="Delivery location" className="pl-10" />
                      </div>
                    </div>
                    <div>
                      <Label className="text-sm text-gray-600">Package Weight</Label>
                      <Input type="number" placeholder="Weight in kg" />
                    </div>
                    <Button className="w-full bg-primary hover:bg-primary/90">
                      Get Quote
                    </Button>
                  </form>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Delivery Stats</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <span className="text-gray-600">Success Rate</span>
                      <span className="font-semibold text-gray-900">98.5%</span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-2">
                      <div className="bg-green-500 h-2 rounded-full" style={{ width: "98.5%" }} />
                    </div>
                    
                    <div className="flex items-center justify-between mt-4">
                      <span className="text-gray-600">Avg. Delivery Time</span>
                      <span className="font-semibold text-gray-900">2.3 days</span>
                    </div>
                    
                    <div className="flex items-center justify-between mt-4">
                      <span className="text-gray-600">Total Spent</span>
                      <span className="font-semibold text-primary">₦108,300</span>
                    </div>
                  </div>
                </CardContent>
              </Card>

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
                        <p className="text-gray-900">Package delivered</p>
                        <p className="text-gray-500 text-xs">Order #GNL-002-2024</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 text-sm">
                      <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                        <TruckIcon className="h-4 w-4 text-blue-600" />
                      </div>
                      <div>
                        <p className="text-gray-900">Package in transit</p>
                        <p className="text-gray-500 text-xs">Order #GNL-001-2024</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 text-sm">
                      <div className="h-8 w-8 rounded-full bg-yellow-100 flex items-center justify-center">
                        <Clock className="h-4 w-4 text-yellow-600" />
                      </div>
                      <div>
                        <p className="text-gray-900">New order pending</p>
                        <p className="text-gray-500 text-xs">Order #GNL-003-2024</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
