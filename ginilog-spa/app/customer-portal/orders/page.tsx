"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Package, Search, LogOut, Bell, User, Loader2, Truck, Hotel, X } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { getProfile, getStoredUser, UserProfile, getCustomerOrders, getCustomerBookings, cancelCustomerBooking, logout } from "@/lib/api";

export default function CustomerOrders() {
  const router = useRouter();
  const [searchTerm, setSearchTerm] = useState("");
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [orders, setOrders] = useState<any[]>([]);
  const [cancellingId, setCancellingId] = useState<string | null>(null);

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const storedUser = getStoredUser();
        if (!storedUser) {
          router.push("/customer-portal/login");
          return;
        }
        const [profile, logOrders, hotelBookings] = await Promise.all([
          getProfile(),
          getCustomerOrders(),
          getCustomerBookings()
        ]);
        
        setUser(profile);
        
        // Combine and normalize data
        const combined = [
          ...(logOrders || []).map((o: any) => ({ ...o, type: 'logistics', title: o.itemName || 'Package' })),
          ...(hotelBookings || []).map((b: any) => ({ ...b, type: 'accommodation', title: b.accomodationName || 'Hotel' }))
        ].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

        setOrders(combined); 
      } catch (error) {
        console.error("Failed to fetch orders:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchOrders();
  }, [router]);

  const getStatusColor = (status: string) => {
    const s = (status || "").toLowerCase();
    if (s === "confirmed" || s === "delivered") return "bg-green-100 text-green-800";
    if (s === "in_transit" || s === "processing") return "bg-blue-100 text-blue-800";
    if (s === "cancelled") return "bg-red-100 text-red-800";
    return "bg-gray-100 text-gray-800";
  };

  const handleCancelBooking = async (id: string) => {
    setCancellingId(id);
    try {
      await cancelCustomerBooking(id);
      setOrders((prev) => prev.filter((o) => o.id !== id));
    } catch (err) {
      console.error("Failed to cancel booking:", err);
    } finally {
      setCancellingId(null);
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
                  {user?.profilePicture ? (
                    <img src={user.profilePicture} alt={user.firstName} className="h-10 w-10 rounded-full object-cover" />
                  ) : (
                    <User className="h-5 w-5 text-primary" />
                  )}
                </div>
                <div className="hidden sm:block">
                  <p className="text-sm font-medium text-gray-900">
                    {user?.firstName} {user?.lastName}
                  </p>
                  <p className="text-xs text-gray-500">Customer</p>
                </div>
              </div>

              <Button
                variant="ghost"
                size="icon"
                className="text-gray-600"
                onClick={() => { logout(); router.push("/customer-portal/login"); }}
              >
                <LogOut className="h-5 w-5" />
              </Button>
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
            {isLoading ? (
              <div className="flex justify-center py-12">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
              </div>
            ) : orders.filter((o) =>
                !searchTerm ||
                (o.trackingNum || o.bookingRefNo || o.id || "")
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                (o.title || "").toLowerCase().includes(searchTerm.toLowerCase())
              ).length > 0 ? (
              orders
                .filter((o) =>
                  !searchTerm ||
                  (o.trackingNum || o.bookingRefNo || o.id || "")
                    .toLowerCase()
                    .includes(searchTerm.toLowerCase()) ||
                  (o.title || "").toLowerCase().includes(searchTerm.toLowerCase())
                )
                .map((order) => (
                <Card key={order.id} className="hover:shadow-md transition-shadow">
                  <div className="p-5 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div className="flex items-start gap-4">
                      <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        {order.type === "logistics" ? (
                          <Truck className="h-5 w-5 text-primary" />
                        ) : (
                          <Hotel className="h-5 w-5 text-primary" />
                        )}
                      </div>
                      <div>
                        <p className="font-semibold text-gray-900">{order.title}</p>
                        <p className="text-sm text-gray-500">
                          {order.trackingNum
                            ? `Tracking: ${order.trackingNum}`
                            : order.bookingRefNo
                            ? `Ref: ${order.bookingRefNo}`
                            : `ID: ${order.id}`}
                        </p>
                        {order.type === "accommodation" && order.checkInDate && (
                          <p className="text-xs text-gray-400 mt-1">
                            {new Date(order.checkInDate).toLocaleDateString()} →{" "}
                            {new Date(order.checkOutDate).toLocaleDateString()}
                          </p>
                        )}
                        {order.type === "logistics" && order.senderAddress && (
                          <p className="text-xs text-gray-400 mt-1">
                            {order.senderAddress} → {order.recieverAddress}
                          </p>
                        )}
                      </div>
                    </div>
                    <div className="flex items-center gap-3 sm:flex-shrink-0">
                      <Badge className={getStatusColor(order.orderStatus || order.bookingStatus || "")}>
                        {order.orderStatus || order.bookingStatus || "Pending"}
                      </Badge>
                      {order.type === "accommodation" &&
                        (order.bookingStatus || "").toLowerCase() !== "cancelled" && (
                          <Button
                            variant="ghost"
                            size="icon"
                            className="text-red-500 hover:text-red-700 hover:bg-red-50"
                            disabled={cancellingId === order.id}
                            onClick={() => handleCancelBooking(order.id)}
                          >
                            {cancellingId === order.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <X className="h-4 w-4" />
                            )}
                          </Button>
                        )}
                    </div>
                  </div>
                </Card>
              ))
            ) : (
              <div className="text-center py-12 bg-white rounded-lg border border-dashed">
                <Package className="h-12 w-12 text-gray-300 mx-auto mb-4" />
                <p className="text-gray-500">No orders or bookings found.</p>
                <Link href="/customer-portal/dashboard" className="mt-4 inline-block">
                  <Button variant="outline">Start Booking</Button>
                </Link>
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
