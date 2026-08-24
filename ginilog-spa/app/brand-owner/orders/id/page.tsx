// app/brand-owner/orders/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Loader2,
  ArrowLeft,
  Package,
  Truck,
  MapPin,
  DollarSign,
  User,
  Phone,
  Mail,
  Calendar,
  Clock,
  CheckCircle,
  XCircle,
  Edit,
  Trash2,
  Copy,
  Share2,
  Box,
  Weight,
  Hash
} from "lucide-react";
import { getOrderById, updatePackageOrder, deletePackageOrder } from "@/lib/api";

export default function OrderDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [order, setOrder] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [updating, setUpdating] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetchOrder = async () => {
      try {
        setIsLoading(true);
        const data = await getOrderById(params.id as string);
        setOrder(data);
      } catch (error) {
        console.error("Failed to fetch order:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchOrder();
  }, [params.id]);

  const handleStatusUpdate = async (status: string) => {
    setUpdating(true);
    try {
      await updatePackageOrder(params.id as string, { orderStatus: status });
      await fetchOrder();
    } catch (error) {
      console.error("Failed to update order:", error);
    } finally {
      setUpdating(false);
    }
  };

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this order?")) return;
    setDeleting(true);
    try {
      await deletePackageOrder(params.id as string);
      router.push("/brand-owner/orders");
    } catch (error) {
      console.error("Failed to delete order:", error);
    } finally {
      setDeleting(false);
    }
  };

  const fetchOrder = async () => {
    try {
      const data = await getOrderById(params.id as string);
      setOrder(data);
    } catch (error) {
      console.error("Failed to fetch order:", error);
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

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!order) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Order not found</p>
        <Link href="/brand-owner/orders">
          <Button className="mt-4">Back to Orders</Button>
        </Link>
      </div>
    );
  }

  const status = getStatusBadge(order.orderStatus);
  const statusOptions = ["Open", "Accepted", "Picked", "Ongoing", "Received", "Closed", "Cancelled"];

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/orders">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Order Details</h1>
          <p className="text-sm text-gray-500">Tracking: {order.trackingNum || "N/A"}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Item Info */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-primary" />
              Package Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Item Name</p>
                <p className="font-semibold text-gray-900">{order.itemName || "N/A"}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Package Type</p>
                <p className="font-semibold text-gray-900">{order.packageType || "N/A"}</p>
              </div>
            </div>

            <div className="grid grid-cols-3 gap-4">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Quantity</p>
                <p className="font-semibold">{order.itemQuantity || 1}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Weight</p>
                <p className="font-semibold">{order.itemWeight || "N/A"} kg</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Cost</p>
                <p className="font-semibold text-primary">₦{order.itemCost?.toLocaleString() || 0}</p>
              </div>
            </div>

            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Description</p>
              <p className="text-gray-700">{order.itemDescription || "No description"}</p>
            </div>
          </CardContent>
        </Card>

        {/* Status & Actions */}
        <Card className="lg:col-span-1">
          <CardHeader>
            <CardTitle>Status & Actions</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="p-4 bg-gray-50 rounded-xl text-center">
              <Badge className={status.className}>
                {status.label}
              </Badge>
            </div>

            <div className="space-y-2">
              <select
                className="w-full px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={order.orderStatus || "Open"}
                onChange={(e) => handleStatusUpdate(e.target.value)}
                disabled={updating}
              >
                {statusOptions.map((s) => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
              {updating && (
                <div className="flex items-center gap-2 text-sm text-gray-500">
                  <Loader2 className="h-3 w-3 animate-spin" />
                  Updating...
                </div>
              )}
            </div>

            <Button
              variant="outline"
              className="w-full gap-2 text-red-500 border-red-200 hover:bg-red-50"
              onClick={handleDelete}
              disabled={deleting}
            >
              {deleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
              Delete Order
            </Button>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Sender Info */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5 text-primary" />
              Sender Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Name</p>
              <p className="font-medium">{order.senderName || "N/A"}</p>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Phone</p>
                <p className="font-medium">{order.senderPhoneNo || "N/A"}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Email</p>
                <p className="font-medium">{order.senderEmail || "N/A"}</p>
              </div>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-600">
              <MapPin className="h-4 w-4 text-gray-400" />
              {order.senderAddress || "N/A"}
            </div>
          </CardContent>
        </Card>

        {/* Receiver Info */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5 text-primary" />
              Receiver Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Name</p>
              <p className="font-medium">{order.recieverName || "N/A"}</p>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Phone</p>
                <p className="font-medium">{order.recieverPhoneNo || "N/A"}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Email</p>
                <p className="font-medium">{order.recieverEmail || "N/A"}</p>
              </div>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-600">
              <MapPin className="h-4 w-4 text-gray-400" />
              {order.recieverAddress || "N/A"}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Delivery Info */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Truck className="h-5 w-5 text-primary" />
            Delivery Information
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Rider</p>
              <p className="font-medium">{order.riderName || "Unassigned"}</p>
            </div>
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Shipping Cost</p>
              <p className="font-medium">₦{order.shippingCost?.toLocaleString() || 0}</p>
            </div>
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Payment</p>
              <Badge className={order.paymentStatus ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}>
                {order.paymentStatus ? "Paid" : "Unpaid"}
              </Badge>
            </div>
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Expected Delivery</p>
              <p className="font-medium">{order.expectedDeliveryTime ? new Date(order.expectedDeliveryTime).toLocaleDateString() : "N/A"}</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}