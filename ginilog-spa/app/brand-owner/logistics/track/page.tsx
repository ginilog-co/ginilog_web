// app/brand-owner/logistics/track/page.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, Search, MapPin, Truck, Package, User, Phone, Mail, Calendar, Clock, CheckCircle, XCircle, AlertCircle } from "lucide-react";
import { trackPackageOrder, getRiderById, OrderTrackingResult } from "@/lib/api";

export default function TrackOrderPage() {
  const router = useRouter();
  const [trackingNumber, setTrackingNumber] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [orderData, setOrderData] = useState<OrderTrackingResult | null>(null);
  const [riderData, setRiderData] = useState<any>(null);
  const [loadingRider, setLoadingRider] = useState(false);

  const handleTrack = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!trackingNumber.trim()) {
      setError("Please enter a tracking number");
      return;
    }

    setLoading(true);
    setError("");
    setOrderData(null);
    setRiderData(null);

    try {
      const data = await trackPackageOrder(trackingNumber.trim());
      setOrderData(data);
      
      // Fetch rider details if assigned
      const riderId = (data as any).riderId || data.riderId;
      if (riderId) {
        try {
          setLoadingRider(true);
          const rider = await getRiderById(riderId);
          setRiderData(rider);
        } catch (riderError) {
          console.error("Failed to fetch rider details:", riderError);
        } finally {
          setLoadingRider(false);
        }
      }
    } catch (err: any) {
      console.error("Failed to track order:", err);
      setError(err.message || "Failed to track order. Please check the tracking number.");
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800", icon: Clock },
      confirmed: { label: "Confirmed", className: "bg-blue-100 text-blue-800", icon: CheckCircle },
      in_transit: { label: "In Transit", className: "bg-blue-100 text-blue-800", icon: Truck },
      delivered: { label: "Delivered", className: "bg-green-100 text-green-800", icon: CheckCircle },
      cancelled: { label: "Cancelled", className: "bg-red-100 text-red-800", icon: XCircle },
      failed: { label: "Failed", className: "bg-red-100 text-red-800", icon: XCircle },
    };
    const s = (status || "").toLowerCase();
    return statusMap[s] || { label: status || "Unknown", className: "bg-gray-100 text-gray-800", icon: AlertCircle };
  };

  const getStatusIcon = (status: string) => {
    const statusMap: Record<string, any> = {
      pending: Clock,
      confirmed: CheckCircle,
      in_transit: Truck,
      delivered: CheckCircle,
      cancelled: XCircle,
      failed: XCircle,
    };
    return statusMap[status?.toLowerCase()] || AlertCircle;
  };

  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-6">Track Package Order</h1>
      
      {/* Search Form */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="text-lg">Enter Tracking Number</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleTrack} className="flex gap-4">
            <div className="flex-1">
              <Label htmlFor="trackingNumber" className="sr-only">Tracking Number</Label>
              <Input
                id="trackingNumber"
                value={trackingNumber}
                onChange={(e) => setTrackingNumber(e.target.value)}
                placeholder="Enter tracking number (e.g., TRK-001)"
                className="w-full"
              />
            </div>
            <Button type="submit" disabled={loading}>
              {loading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <Search className="h-4 w-4 mr-2" />
              )}
              Track
            </Button>
          </form>
          {error && (
            <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 flex items-start gap-2">
              <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
              <p className="text-sm">{error}</p>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Order Details */}
      {orderData && (
        <div className="space-y-6">
          {/* Status Card */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                <span>Order Status</span>
                <Badge className={getStatusBadge(orderData.orderStatus || "").className}>
                  {getStatusBadge(orderData.orderStatus || "").label}
                </Badge>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div>
                  <p className="text-sm text-gray-500">Tracking Number</p>
                  <p className="font-semibold">{orderData.trackingNum || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Order Status</p>
                  <p className="font-semibold">{orderData.orderStatus || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Expected Delivery</p>
                  <p className="font-semibold">
                    {orderData.expectedDeliveryTime 
                      ? new Date(orderData.expectedDeliveryTime).toLocaleDateString() 
                      : "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Payment Status</p>
                  <p className="font-semibold">
                    {orderData.paymentStatus ? "Paid" : "Unpaid"}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Package Details */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Package className="h-5 w-5" />
                Package Details
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <div>
                  <p className="text-sm text-gray-500">Item Name</p>
                  <p className="font-semibold">{orderData.itemName || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Item Description</p>
                  <p className="font-semibold">{orderData.itemDescription || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Package Type</p>
                  <p className="font-semibold">{orderData.packageType || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Weight</p>
                  <p className="font-semibold">{orderData.itemWeight || 0} kg</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Quantity</p>
                  <p className="font-semibold">{orderData.itemQuantity || 0}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Cost</p>
                  <p className="font-semibold">₦{orderData.itemCost || 0}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Sender & Receiver */}
          <div className="grid md:grid-cols-2 gap-6">
            {/* Sender */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <User className="h-5 w-5" />
                  Sender
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <div>
                  <p className="text-sm text-gray-500">Name</p>
                  <p className="font-medium">{orderData.senderName || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Phone</p>
                  <p className="font-medium flex items-center gap-1">
                    <Phone className="h-3 w-3 text-gray-400" />
                    {orderData.senderPhoneNo || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Email</p>
                  <p className="font-medium flex items-center gap-1">
                    <Mail className="h-3 w-3 text-gray-400" />
                    {orderData.senderEmail || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Address</p>
                  <p className="font-medium flex items-center gap-1">
                    <MapPin className="h-3 w-3 text-gray-400" />
                    {orderData.senderAddress || "N/A"}, {orderData.senderLocality || ""}, {orderData.senderState || ""}
                  </p>
                </div>
              </CardContent>
            </Card>

            {/* Receiver */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <User className="h-5 w-5" />
                  Receiver
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <div>
                  <p className="text-sm text-gray-500">Name</p>
                  <p className="font-medium">{orderData.recieverName || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Phone</p>
                  <p className="font-medium flex items-center gap-1">
                    <Phone className="h-3 w-3 text-gray-400" />
                    {orderData.recieverPhoneNo || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Email</p>
                  <p className="font-medium flex items-center gap-1">
                    <Mail className="h-3 w-3 text-gray-400" />
                    {orderData.recieverEmail || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Address</p>
                  <p className="font-medium flex items-center gap-1">
                    <MapPin className="h-3 w-3 text-gray-400" />
                    {orderData.recieverAddress || "N/A"}, {orderData.recieverLocality || ""}, {orderData.recieverState || ""}
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Rider Details */}
          {(riderData || (orderData as any).riderName) && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Truck className="h-5 w-5" />
                  Rider Details
                </CardTitle>
              </CardHeader>
              <CardContent>
                {loadingRider ? (
                  <div className="flex items-center justify-center py-4">
                    <Loader2 className="h-6 w-6 animate-spin" />
                  </div>
                ) : riderData ? (
                  <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                    <div>
                      <p className="text-sm text-gray-500">Rider Name</p>
                      <p className="font-semibold">{riderData.firstName} {riderData.lastName}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Phone</p>
                      <p className="font-semibold">{riderData.phoneNumber || "N/A"}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Vehicle Type</p>
                      <p className="font-semibold">{riderData.vehicleType || "N/A"}</p>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Status</p>
                      <Badge className={riderData.status === "Available" ? "bg-green-100 text-green-800" : "bg-yellow-100 text-yellow-800"}>
                        {riderData.status || "N/A"}
                      </Badge>
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Rating</p>
                      <p className="font-semibold">{riderData.rating || 0} ⭐</p>
                    </div>
                  </div>
                ) : (
                  <div>
                    <p className="font-semibold">{orderData.riderName || "Rider not assigned"}</p>
                  </div>
                )}
              </CardContent>
            </Card>
          )}

          {/* Delivery Flow Timeline */}
          {orderData.orderDeliveryFlows && orderData.orderDeliveryFlows.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Clock className="h-5 w-5" />
                  Delivery Timeline
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {orderData.orderDeliveryFlows.map((flow, index) => {
                    const StatusIcon = getStatusIcon(flow.orderStatus);
                    const status = getStatusBadge(flow.orderStatus);
                    return (
                      <div key={flow.id || index} className="flex items-start gap-4">
                        <div className={`p-2 rounded-full ${status.className}`}>
                          <StatusIcon className="h-4 w-4" />
                        </div>
                        <div>
                          <p className="font-medium">{status.label}</p>
                          {flow.currentLocation && (
                            <p className="text-sm text-gray-500">{flow.currentLocation}</p>
                          )}
                          {flow.updatedAt && (
                            <p className="text-xs text-gray-400">
                              {new Date(flow.updatedAt).toLocaleString()}
                            </p>
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Actions */}
          <div className="flex gap-4">
            <Button onClick={() => window.print()} variant="outline">
              Print Details
            </Button>
            <Button onClick={() => setOrderData(null)} variant="secondary">
              Track Another
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}