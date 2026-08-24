// app/brand-owner/logistics/assign/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import {
  Loader2,
  ArrowLeft,
  Package,
  Truck,
  User,
  MapPin,
  Search,
  CheckCircle,
  Clock,
  XCircle,
  Filter,
  UserCheck,
  Bike,
  Car
} from "lucide-react";
import { getPackageOrders, getRiders, assignOrderToRider } from "@/lib/api";

export default function AssignOrdersPage() {
  const router = useRouter();
  const [orders, setOrders] = useState<any[]>([]);
  const [riders, setRiders] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [assigning, setAssigning] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [selectedOrder, setSelectedOrder] = useState<any>(null);
  const [showAssignModal, setShowAssignModal] = useState(false);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setIsLoading(true);
      const [ordersData, ridersData] = await Promise.all([
        getPackageOrders(),
        getRiders(),
      ]);
      // Filter orders that need assignment (Open or Accepted status)
      const unassignedOrders = (ordersData || []).filter(
        (o: any) => !o.riderId && (o.orderStatus === "Open" || o.orderStatus === "Accepted")
      );
      setOrders(unassignedOrders);
      setRiders(ridersData || []);
    } catch (error) {
      console.error("Failed to fetch data:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleAssign = async (orderId: string, riderId: string) => {
    setAssigning(orderId);
    try {
      await assignOrderToRider(orderId, riderId);
      await fetchData();
    } catch (error) {
      console.error("Failed to assign order:", error);
    } finally {
      setAssigning(null);
      setShowAssignModal(false);
      setSelectedOrder(null);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string }> = {
      open: { label: "Open", className: "bg-blue-100 text-blue-800" },
      accepted: { label: "Accepted", className: "bg-cyan-100 text-cyan-800" },
      picked: { label: "Picked", className: "bg-purple-100 text-purple-800" },
      ongoing: { label: "Ongoing", className: "bg-yellow-100 text-yellow-800" },
      received: { label: "Received", className: "bg-green-100 text-green-800" },
      closed: { label: "Closed", className: "bg-gray-100 text-gray-800" },
      cancelled: { label: "Cancelled", className: "bg-red-100 text-red-800" },
    };
    return statusMap[status?.toLowerCase()] || statusMap.open;
  };

  const filteredOrders = orders.filter((order) =>
    order.trackingNum?.toLowerCase().includes(search.toLowerCase()) ||
    order.itemName?.toLowerCase().includes(search.toLowerCase()) ||
    order.senderName?.toLowerCase().includes(search.toLowerCase()) ||
    order.recieverName?.toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/logistics">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Assign Orders</h1>
          <p className="text-sm text-gray-500">Assign delivery orders to available riders</p>
        </div>
      </div>

      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            placeholder="Search orders..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-9"
          />
        </div>
        <div className="flex items-center gap-2 text-sm text-gray-500">
          <span>Available Riders: {riders.filter(r => r.status === "Available").length}</span>
          <span className="text-gray-300">|</span>
          <span>Pending Orders: {orders.length}</span>
        </div>
      </div>

      {filteredOrders.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Package className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No orders pending assignment</p>
            <Button className="mt-4" variant="outline" onClick={() => router.push("/brand-owner/orders")}>
              View All Orders
            </Button>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0 overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-gray-50">
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Order</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Item</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Sender</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Receiver</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Action</th>
                </tr>
              </thead>
              <tbody>
                {filteredOrders.map((order) => {
                  const status = getStatusBadge(order.orderStatus);
                  return (
                    <tr key={order.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4 font-mono text-xs">
                        {order.trackingNum || order.id.slice(0, 8)}
                      </td>
                      <td className="py-3 px-4">
                        <div>
                          <p className="font-medium">{order.itemName}</p>
                          <p className="text-xs text-gray-500">{order.packageType || "Package"}</p>
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <p className="text-sm">{order.senderName}</p>
                      </td>
                      <td className="py-3 px-4">
                        <p className="text-sm">{order.recieverName}</p>
                      </td>
                      <td className="py-3 px-4">
                        <Badge className={status.className}>
                          {status.label}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <Button
                          size="sm"
                          className="gap-1"
                          onClick={() => {
                            setSelectedOrder(order);
                            setShowAssignModal(true);
                          }}
                          disabled={assigning === order.id}
                        >
                          {assigning === order.id ? (
                            <Loader2 className="h-3 w-3 animate-spin" />
                          ) : (
                            <UserCheck className="h-3 w-3" />
                          )}
                          Assign
                        </Button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}

      {/* Assign Modal */}
      {showAssignModal && selectedOrder && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-2xl p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold">Assign Order to Rider</h3>
              <button
                onClick={() => {
                  setShowAssignModal(false);
                  setSelectedOrder(null);
                }}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>

            <div className="mb-4 p-4 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Order Details</p>
              <p className="font-medium">{selectedOrder.itemName}</p>
              <p className="text-sm text-gray-500">Tracking: {selectedOrder.trackingNum || selectedOrder.id.slice(0, 8)}</p>
              <div className="flex items-center gap-4 mt-2 text-sm">
                <span>From: {selectedOrder.senderName}</span>
                <span>To: {selectedOrder.recieverName}</span>
              </div>
            </div>

            <div className="space-y-2">
              <p className="text-sm font-medium text-gray-700">Available Riders</p>
              {riders.filter(r => r.status === "Available").length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  <Truck className="h-12 w-12 text-gray-300 mx-auto mb-3" />
                  <p>No available riders</p>
                </div>
              ) : (
                <div className="space-y-2">
                  {riders
                    .filter(r => r.status === "Available")
                    .map((rider) => (
                      <div
                        key={rider.id}
                        className="flex items-center justify-between p-3 border rounded-xl hover:bg-gray-50 cursor-pointer"
                        onClick={() => handleAssign(selectedOrder.id, rider.id)}
                      >
                        <div className="flex items-center gap-3">
                          <div className="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                            <span className="text-sm font-medium text-blue-600">
                              {rider.firstName?.[0]}{rider.lastName?.[0]}
                            </span>
                          </div>
                          <div>
                            <p className="font-medium">{rider.firstName} {rider.lastName}</p>
                            <div className="flex items-center gap-2 text-sm text-gray-500">
                              {rider.vehicleType?.toLowerCase().includes("bike") ? (
                                <Bike className="h-3 w-3" />
                              ) : (
                                <Car className="h-3 w-3" />
                              )}
                              {rider.vehicleType || "N/A"}
                            </div>
                          </div>
                        </div>
                        <Badge className="bg-green-100 text-green-800">Available</Badge>
                      </div>
                    ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}