"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { User, Mail, Phone, MapPin, Camera, Save, Bell, LogOut } from "lucide-react";

const mockUser = {
  id: "1",
  firstName: "John",
  lastName: "Doe",
  email: "john.doe@example.com",
  userType: "logistics", // or "accommodation"
  avatar: null,
};

export default function CustomerProfile() {
  const [formData, setFormData] = useState({
    firstName: "John",
    lastName: "Doe",
    email: "john.doe@example.com",
    phone: "+234 800 000 0000",
    address: "123 Business Way, Lagos, Nigeria",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Profile updated:", formData);
  };

  const [user] = useState(mockUser);

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
              
              <Link href="/customer-portal/profile">
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
              </Link>
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
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold text-gray-900 mb-8">My Profile</h1>
          
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <Card className="lg:col-span-1">
              <CardContent className="p-6 text-center">
                <div className="relative inline-block mb-4">
                  <div className="h-32 w-32 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden border-4 border-white shadow-sm">
                    <User className="h-16 w-16 text-primary" />
                  </div>
                  <button className="absolute bottom-0 right-0 p-2 bg-white rounded-full shadow-md border text-gray-600 hover:text-primary transition-colors">
                    <Camera className="h-4 w-4" />
                  </button>
                </div>
                <h2 className="text-xl font-bold text-gray-900">{formData.firstName} {formData.lastName}</h2>
                <p className="text-gray-500 text-sm">Customer</p>
              </CardContent>
            </Card>

            <Card className="lg:col-span-2">
              <CardHeader>
                <CardTitle>Personal Information</CardTitle>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="firstName">First Name</Label>
                      <div className="relative">
                        <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input
                          id="firstName"
                          className="pl-10"
                          value={formData.firstName}
                          onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                        />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="lastName">Last Name</Label>
                      <div className="relative">
                        <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input
                          id="lastName"
                          className="pl-10"
                          value={formData.lastName}
                          onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                        />
                      </div>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="email">Email Address</Label>
                    <div className="relative">
                      <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input
                        id="email"
                        type="email"
                        className="pl-10"
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      />
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="phone">Phone Number</Label>
                    <div className="relative">
                      <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input
                        id="phone"
                        className="pl-10"
                        value={formData.phone}
                        onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      />
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="address">Address</Label>
                    <div className="relative">
                      <MapPin className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
                      <textarea
                        id="address"
                        className="w-full pl-10 pr-3 py-2 border rounded-md focus:ring-primary focus:border-primary min-h-[100px]"
                        value={formData.address}
                        onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                      />
                    </div>
                  </div>

                  <Button type="submit" className="w-full sm:w-auto">
                    <Save className="h-4 w-4 mr-2" />
                    Save Changes
                  </Button>
                </form>
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
    </div>
  );
}
