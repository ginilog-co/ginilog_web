// app/brand-owner/logistics/track/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import {
  Loader2,
  ArrowLeft,
  MapPin,
  Navigation,
  Truck,
  User,
  Package,
  Clock,
  CheckCircle,
  XCircle,
  Search,
  Map,
  Locate,
  Phone,
  Mail,
  Bike,
  Car
} from "lucide-react";
import { trackPackageOrder, getRiderById } from "@/lib/api";

export default function TrackDeliveriesPage() {
  const router = useRouter();
  const [trackingNumber, setTrackingNumber] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [orderData, setOrderData] = useState<any>(null);
  const [error, setError] = useState("");
  const [riderData, setRiderData] = useState<any>(null);

  const handleTrack = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!trackingNumber.trim()) {
      setError("Please enter a tracking number");
      return;
    }

    setIsLoading(true);
    setError("");
    setOrderData(null);
    setRiderData(null);

    try {
      const data = await trackPackageOrder(trackingNumber.trim());
      setOrderData(data);
      
      // Fetch rider details if assigned
      if (data.riderId) {
        try {
          const rider = await getRiderById(data.riderId);
          setRiderData(rider);
        } catch (riderErr) {
          console.error("Failed to fetch rider details:", riderErr);
        }
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to track order");
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      open: { label: "Open", className: "bg-blue-100 text-blue-800", icon: Package },
      accepted: { label: "Accepted", className: "bg-cyan-100 text-cyan-800", icon: CheckCircle },
      picked: { label: "Picked", className: "bg-purple-100 text-purple-800", icon: Truck },
      ongoing: { label: "Ongoing", className: "bg-yellow-100 text-yellow-800", icon: Navigation },
      received: { label: "Received", className: "bg-green-100 text-green-800", icon: CheckCircle },
      closed: { label: "Closed", className: "bg-gray-100 text-gray-800", icon: CheckCircle },
      cancelled: { label: "Cancelled", className: "bg-red-100 text-red-800", icon: XCircle },
    };
    return statusMap[status?.toLowerCase()] || statusMap.open;
  };

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
          <h1 className="text-2xl font-bold text-gray-900">Track Deliveries</h1>
          <p className="text-sm text-gray-500">Track package deliveries in real-time</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Enter Tracking Number</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleTrack} className="flex gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Enter tracking number..."
                value={trackingNumber}
                onChange={(e) => setTrackingNumber(e.target.value)}
                className="pl-9"
                disabled={isLoading}
              />
            </div>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : <MapPin className="h-4 w-4 mr-2" />}
              {isLoading ? "Tracking..." : "Track"}
            </Button>
          </form>

          {error && (
            <div className="mt-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
              {error}
            </div>
          )}
        </CardContent>
      </Card>

      {orderData && (
        <>
          {/* Order Summary */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                <span>Order Details</span>
                <Badge className={getStatusBadge(orderData.orderStatus).className}>
                  {getStatusBadge(orderData.orderStatus).label}
                </Badge>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-sm text-gray-500">Tracking #</p>
                  <p className="font-mono font-medium">{orderData.trackingNum}</p>
                </div>
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-sm text-gray-500">Item</p>
                  <p className="font-medium">{orderData.itemName}</p>
                </div>
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-sm text-gray-500">Package Type</p>
                  <p className="font-medium">{orderData.packageType || "N/A"}</p>
                </div>
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-sm text-gray-500">Cost</p>
                  <p className="font-medium">₦{orderData.itemCost?.toLocaleString() || 0}</p>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="p-4 bg-blue-50 rounded-xl">
                  <p className="text-sm font-medium text-blue-700 flex items-center gap-2">
                    <MapPin className="h-4 w-4" />
                    Sender
                  </p>
                  <p className="font-medium">{orderData.senderName}</p>
                  <p className="text-sm text-gray-600">{orderData.senderAddress}</p>
                  <p className="text-sm text-gray-500">{orderData.senderPhoneNo}</p>
                </div>
                <div className="p-4 bg-green-50 rounded-xl">
                  <p className="text-sm font-medium text-green-700 flex items-center gap-2">
                    <MapPin className="h-4 w-4" />
                    Receiver
                  </p>
                  <p className="font-medium">{orderData.recieverName}</p>
                  <p className="text-sm text-gray-600">{orderData.recieverAddress}</p>
                  <p className="text-sm text-gray-500">{orderData.recieverPhoneNo}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Rider & Location */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Truck className="h-5 w-5 text-primary" />
                  Rider Information
                </CardTitle>
              </CardHeader>
              <CardContent>
                {riderData ? (
                  <div className="space-y-3">
                    <div className="flex items-center gap-3">
                      <div className="h-12 w-12 rounded-full bg-blue-100 flex items-center justify-center">
                        <span className="text-lg font-semibold text-blue-600">
                          {riderData.firstName?.[0]}{riderData.lastName?.[0]}
                        </span>
                      </div>
                      <div>
                        <p className="font-semibold">{riderData.firstName} {riderData.lastName}</p>
                        <div className="flex items-center gap-2 text-sm text-gray-500">
                          {riderData.vehicleType?.toLowerCase().includes("bike") ? (
                            <Bike className="h-3 w-3" />
                          ) : (
                            <Car className="h-3 w-3" />
                          )}
                          {riderData.vehicleType || "N/A"}
                        </div>
                      </div>
                    </div>
                    <div className="flex items-center gap-2 text-sm">
                      <Phone className="h-4 w-4 text-gray-400" />
                      {riderData.phoneNumber || "N/A"}
                    </div>
                    <Badge className={riderData.status === "Available" ? "bg-green-100 text-green-800" : "bg-blue-100 text-blue-800"}>
                      {riderData.status || "N/A"}
                    </Badge>
                  </div>
                ) : (
                  <div className="text-center py-8 text-gray-500">
                    <User className="h-12 w-12 text-gray-300 mx-auto mb-3" />
                    <p>No rider assigned yet</p>
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Map className="h-5 w-5 text-primary" />
                  Current Location
                </CardTitle>
              </CardHeader>
              <CardContent>
                {orderData.currentLocation ? (
                  <div className="space-y-3">
                    <div className="flex items-start gap-3 p-4 bg-yellow-50 rounded-xl">
                      <Locate className="h-5 w-5 text-yellow-600 flex-shrink-0 mt-0.5" />
                      <div>
                        <p className="font-medium">{orderData.currentLocation}</p>
                        <p className="text-sm text-gray-500">
                          Updated: {new Date(orderData.updatedAt).toLocaleString()}
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-2 text-sm">
                      <MapPin className="h-4 w-4 text-gray-400" />
                      Lat: {orderData.currentLatitude || "N/A"}, Lng: {orderData.currentLongitude || "N/A"}
                    </div>
                  </div>
                ) : (
                  <div className="text-center py-8 text-gray-500">
                    <Map className="h-12 w-12 text-gray-300 mx-auto mb-3" />
                    <p>Location not available</p>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Delivery Timeline */}
          {orderData.orderDeliveryFlows?.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Clock className="h-5 w-5 text-primary" />
                  Delivery Timeline
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {orderData.orderDeliveryFlows.map((flow: any, index: number) => (
                    <div key={flow.id} className="flex items-start gap-4">
                      <div className="flex flex-col items-center">
                        <div className={`h-8 w-8 rounded-full flex items-center justify-center ${
                          index === orderData.orderDeliveryFlows.length - 1 
                            ? 'bg-green-500 text-white' 
                            : 'bg-gray-200 text-gray-500'
                        }`}>
                          {index === orderData.orderDeliveryFlows.length - 1 ? (
                            <CheckCircle className="h-4 w-4" />
                          ) : (
                            <span className="text-xs font-medium">{index + 1}</span>
                          )}
                        </div>
                        {index < orderData.orderDeliveryFlows.length - 1 && (
                          <div className="w-0.5 h-8 bg-gray-200"></div>
                        )}
                      </div>
                      <div className="flex-1 pb-4">
                        <p className="font-medium">{flow.orderStatus}</p>
                        <p className="text-sm text-gray-500">{flow.currentLocation || "N/A"}</p>
                        <p className="text-xs text-gray-400">{new Date(flow.updatedAt).toLocaleString()}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}
        </>
      )}
    </div>
  );
}