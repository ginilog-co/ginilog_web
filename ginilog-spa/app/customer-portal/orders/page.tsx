"use client";

import Link from "next/link";
import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Package, Home, Calendar, MapPin, Search, LogOut, Bell, User } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

const mockUser = {
  id: "1",
  firstName: "John",
  lastName: "Doe",
  email: "john.doe@example.com",
  userType: "logistics", // or "accommodation"
  avatar: null,
};

export default function CustomerOrders() {
  const [searchTerm, setSearchTerm] = useState("");
  const [user] = useState(mockUser);

  const orders = [
    {
      id: "GNL-12345",
      type: "logistics",
      title: "Electronics Delivery",
      status: "in_transit",
      date: "2024-03-20",
      amount: "₦15,000",
      location: "Lagos to Abuja",
    },
    {
      id: "BK-67890",
      type: "accommodation",
      title: "Grand Hotel",
      status: "confirmed",
      date: "2024-03-15",
      amount: "₦45,000",
      location: "Victoria Island, Lagos",
    },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case "confirmed": return "bg-green-100 text-green-800";
      case "in_transit": return "bg-blue-100 text-blue-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 pb-12">
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
        <div className="max-w-5xl mx-auto">
          <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
            <h1 className="text-3xl font-bold text-gray-900">My Orders & Bookings</h1>
            <div className="relative w-full md:w-64">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search by ID..."
                className="pl-10"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
          </div>

          <div className="space-y-4">
            {orders.map((order) => (
              <Card key={order.id} className="hover:shadow-md transition-shadow">
                <CardContent className="p-6">
                  <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-start gap-4">
                      <div className={`p-3 rounded-lg ${order.type === 'logistics' ? 'bg-blue-50 text-blue-600' : 'bg-green-50 text-green-600'}`}>
                        {order.type === 'logistics' ? <Package className="h-6 w-6" /> : <Home className="h-6 w-6" />}
                      </div>
                      <div>
                        <div className="flex items-center gap-2 mb-1">
                          <h3 className="font-bold text-gray-900">{order.title}</h3>
                          <Badge className={getStatusColor(order.status)}>
                            {order.status.replace('_', ' ')}
                          </Badge>
                        </div>
                        <p className="text-sm text-gray-500 font-medium">ID: {order.id}</p>
                        <div className="flex flex-wrap items-center gap-4 mt-2 text-sm text-gray-500">
                          <div className="flex items-center gap-1">
                            <Calendar className="h-4 w-4" />
                            {order.date}
                          </div>
                          <div className="flex items-center gap-1">
                            <MapPin className="h-4 w-4" />
                            {order.location}
                          </div>
                        </div>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="text-2xl font-bold text-primary">{order.amount}</p>
                      <button className="text-sm text-primary hover:underline mt-1 font-medium">
                        View Details
                      </button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </main>
    </div>
  );
}
