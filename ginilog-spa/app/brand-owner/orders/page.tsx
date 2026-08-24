// app/brand-owner/orders/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import {
  Search,
  Eye,
  Loader2,
  Package,
  Truck,
  MapPin,
  DollarSign,
  Clock,
  CheckCircle,
  XCircle,
  Filter,
  ArrowRight,
  User
} from "lucide-react";
import { getPackageOrders, updatePackageOrder } from "@/lib/api";

export default function OrdersPage() {
  const [orders, setOrders] = useState<any[]>([]);
  const [filteredOrders, setFilteredOrders] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [updatingId, setUpdatingId] = useState<string | null>(null);

  useEffect(() => {
    fetchOrders();
  }, []);

  useEffect(() => {
    let filtered = orders;
    
    if (search.trim()) {
      filtered = filtered.filter((o) =>
        o.trackingNum?.toLowerCase().includes(search.toLowerCase()) ||
        o.itemName?.toLowerCase().includes(search.toLowerCase()) ||
        o.senderName?.toLowerCase().includes(search.toLowerCase()) ||
        o.recieverName?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter !== "all") {
      filtered = filtered.filter((o) => 
        o.orderStatus?.toLowerCase() === filter.toLowerCase()
      );
    }
    
    setFilteredOrders(filtered);
  }, [search, filter, orders]);

  const fetchOrders = async () => {
    try {
      setIsLoading(true);
      const data = await getPackageOrders();
      setOrders(data || []);
      setFilteredOrders(data || []);
    } catch (error) {
      console.error("Failed to fetch orders:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusUpdate = async (id: string, status: string) => {
    setUpdatingId(id);
    try {
      await updatePackageOrder(id, { orderStatus: status });
      await fetchOrders();
    } catch (error) {
      console.error("Failed to update order:", error);
    } finally {
      setUpdatingId(null);
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

  const statusOptions = ["Open", "Accepted", "Picked", "Ongoing", "Received", "Closed", "Cancelled"];

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Orders</h1>
          <p className="text-sm text-gray-500">Manage package orders and deliveries</p>
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
        <select
          className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        >
          <option value="all">All Status</option>
          <option value="open">Open</option>
          <option value="accepted">Accepted</option>
          <option value="picked">Picked</option>
          <option value="ongoing">Ongoing</option>
          <option value="received">Received</option>
          <option value="closed">Closed</option>
          <option value="cancelled">Cancelled</option>
        </select>
      </div>

      {filteredOrders.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Package className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No orders found</p>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0 overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-gray-50">
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Tracking</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Item</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Sender</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Receiver</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Amount</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Actions</th>
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
                        <p className="text-xs text-gray-500">{order.senderPhoneNo || "N/A"}</p>
                      </td>
                      <td className="py-3 px-4">
                        <p className="text-sm">{order.recieverName}</p>
                        <p className="text-xs text-gray-500">{order.recieverPhoneNo || "N/A"}</p>
                      </td>
                      <td className="py-3 px-4 font-medium">
                        ₦{order.shippingCost?.toLocaleString() || 0}
                      </td>
                      <td className="py-3 px-4">
                        <Badge className={status.className}>
                          {status.label}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-2">
                          <select
                            className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                            value={order.orderStatus || "Open"}
                            onChange={(e) => handleStatusUpdate(order.id, e.target.value)}
                            disabled={updatingId === order.id}
                          >
                            {statusOptions.map((s) => (
                              <option key={s} value={s}>{s}</option>
                            ))}
                          </select>
                          <Link href={`/brand-owner/orders/${order.id}`}>
                            <Button size="sm" variant="ghost" className="h-7 w-7 p-0">
                              <Eye className="h-3 w-3" />
                            </Button>
                          </Link>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}
    </div>
  );
}