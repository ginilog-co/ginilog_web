"use client";

import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Search,
  Loader2,
  Package,
  Truck,
  MapPin,
  Phone,
  Building2,
  CreditCard,
  AlertTriangle,
  ArrowLeft,
  Clock,
  User,
  Home,
  Calendar,
} from "lucide-react";
import { Badge } from "@/components/ui/badge";

interface TrackingModalProps {
  isOpen: boolean;
  onClose: () => void;
  orders: any[];
  bookings: any[];
}

export function TrackingModal({
  isOpen,
  onClose,
  orders = [],
  bookings = [],
}: TrackingModalProps) {
  const [trackingNumber, setTrackingNumber] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [result, setResult] = useState<any>(null);
  const [error, setError] = useState<string | null>(null);

  // Combine orders and bookings for searching
  const allItems = [
    ...orders.map((o) => ({ ...o, type: "order" })),
    ...bookings.map((b) => ({ ...b, type: "booking" })),
  ];

  const handleTrack = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!trackingNumber.trim()) return;

    setIsLoading(true);
    setError(null);
    setResult(null);

    // Simulate API delay for better UX
    await new Promise((resolve) => setTimeout(resolve, 800));

    // Search in orders and bookings
    const found = allItems.find(
      (item) =>
        item.trackingNum?.toLowerCase() === trackingNumber.trim().toLowerCase() ||
        item.bookingRefNo?.toLowerCase() === trackingNumber.trim().toLowerCase() ||
        item.id?.toLowerCase() === trackingNumber.trim().toLowerCase()
    );

    if (found) {
      setResult(found);
    } else {
      setError(
        "No tracking information found for this ID. Please verify and try again."
      );
    }

    setIsLoading(false);
  };

  const getStatusColor = (status: string) => {
    const statusMap: Record<string, string> = {
      delivered: "bg-green-100 text-green-800 border-green-200",
      "in-transit": "bg-blue-100 text-blue-800 border-blue-200",
      pending: "bg-yellow-100 text-yellow-800 border-yellow-200",
      cancelled: "bg-red-100 text-red-800 border-red-200",
      processing: "bg-purple-100 text-purple-800 border-purple-200",
      confirmed: "bg-green-100 text-green-800 border-green-200",
      completed: "bg-green-100 text-green-800 border-green-200",
      "waiting for pickup": "bg-yellow-100 text-yellow-800 border-yellow-200",
    };
    return (
      statusMap[status?.toLowerCase()] || "bg-gray-100 text-gray-800 border-gray-200"
    );
  };

  const getStatusIcon = (status: string) => {
    const statusMap: Record<string, any> = {
      delivered: Package,
      "in-transit": Truck,
      pending: Clock,
      cancelled: AlertTriangle,
      processing: Loader2,
      confirmed: Package,
      completed: Package,
      "waiting for pickup": Clock,
    };
    const Icon = statusMap[status?.toLowerCase()];
    return Icon ? <Icon className="h-4 w-4" /> : <Package className="h-4 w-4" />;
  };

  const formatCurrency = (amount: number) => {
    if (!amount) return "N0.00";
    return `N${amount.toLocaleString()}`;
  };

  // Get display name for the item
  const getItemName = (item: any) => {
    if (item.type === "booking") {
      return item.accomodationName || item.hotelName || "Accommodation";
    }
    return item.itemName || item.productName || "Package";
  };

  // Get tracking reference
  const getTrackingRef = (item: any) => {
    return item.trackingNum || item.bookingRefNo || item.id || "N/A";
  };

  // Get status
  const getStatus = (item: any) => {
    return item.orderStatus || item.bookingStatus || item.status || "Pending";
  };

  // Get origin
  const getOrigin = (item: any) => {
    return item.pickupLocation || item.origin || item.from || "N/A";
  };

  // Get destination
  const getDestination = (item: any) => {
    return item.deliveryLocation || item.destination || item.to || "N/A";
  };

  // Get contact
  const getContact = (item: any) => {
    return item.recipientPhone || item.contact || item.phone || "N/A";
  };

  // Get company name
  const getCompanyName = (item: any) => {
    return item.companyName || item.logisticsCompany || "N/A";
  };

  // Get company address
  const getCompanyAddress = (item: any) => {
    return item.companyAddress || item.address || "N/A";
  };

  // Get company contact
  const getCompanyContact = (item: any) => {
    return item.companyContact || item.contactNumber || "N/A";
  };

  // Get delivery charge
  const getDeliveryCharge = (item: any) => {
    return item.deliveryCharge || item.shippingCost || item.amount || 0;
  };

  // Get VAT
  const getVat = (item: any) => {
    return item.vat || item.tax || item.serviceCharge || 0;
  };

  // Get total
  const getTotal = (item: any) => {
    return getDeliveryCharge(item) + getVat(item);
  };

  // Get quantity
  const getQuantity = (item: any) => {
    return item.quantity || item.itemQuantity || 1;
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-lg max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-xl">
            <Search className="h-5 w-5 text-primary" />
            Track Package
          </DialogTitle>
          <DialogDescription>
            Enter your tracking number or booking reference
          </DialogDescription>
        </DialogHeader>

        {/* Search Form */}
        <form onSubmit={handleTrack} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="tracking-input">Tracking / Booking ID</Label>
            <div className="flex gap-2">
              <Input
                id="tracking-input"
                placeholder="e.g., C59IXG-KDUN-EPU"
                value={trackingNumber}
                onChange={(e) => setTrackingNumber(e.target.value)}
                className="flex-1"
                disabled={isLoading}
                autoFocus
              />
              <Button type="submit" disabled={isLoading || !trackingNumber.trim()}>
                {isLoading ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <Search className="h-4 w-4" />
                )}
                <span className="ml-2">{isLoading ? "Searching..." : "Track"}</span>
              </Button>
            </div>
            <p className="text-xs text-gray-500">
              Enter tracking number or booking reference
            </p>
          </div>
        </form>

        {/* Loading */}
        {isLoading && (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="ml-2 text-gray-600">Searching for your package...</span>
          </div>
        )}

        {/* Error */}
        {error && !isLoading && (
          <div className="p-4 border rounded-lg bg-red-50 border-red-200">
            <div className="flex items-start gap-3">
              <AlertTriangle className="h-5 w-5 text-red-600 mt-0.5" />
              <div>
                <p className="font-medium text-red-800">Not Found</p>
                <p className="text-sm text-red-600">{error}</p>
                <p className="text-xs text-red-500 mt-1">
                  Please check the ID and try again
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Result - Order Details like mobile app */}
        {result && !isLoading && (
          <div className="space-y-4">
            {/* Header */}
            <div className="flex items-center justify-between p-4 bg-primary/5 rounded-lg border border-primary/10">
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                  {result.type === "booking" ? (
                    <Building2 className="h-5 w-5 text-blue-600" />
                  ) : (
                    <Package className="h-5 w-5 text-primary" />
                  )}
                </div>
                <div>
                  <p className="font-semibold text-gray-900">{getItemName(result)}</p>
                  <p className="text-xs text-gray-500">
                    {result.type === "booking" ? "Booking" : "Order"} · {getTrackingRef(result)}
                  </p>
                </div>
              </div>
              <Badge className={getStatusColor(getStatus(result))}>
                <span className="flex items-center gap-1 capitalize">
                  {getStatusIcon(getStatus(result))}
                  {getStatus(result)}
                </span>
              </Badge>
            </div>

            {/* Package Information */}
            <div className="border rounded-lg overflow-hidden">
              <div className="bg-gray-50 px-4 py-2 border-b">
                <h4 className="font-semibold text-sm text-gray-700">
                  Package Information
                </h4>
              </div>
              <div className="p-4 space-y-3">
                {/* Origin & Destination */}
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div>
                    <p className="text-gray-500 text-xs">Origin Details</p>
                    <p className="font-medium text-gray-900 flex items-center gap-1">
                      <MapPin className="h-3 w-3 text-gray-400" />
                      {getOrigin(result)}
                    </p>
                  </div>
                  <div>
                    <p className="text-gray-500 text-xs">Destination Details</p>
                    <p className="font-medium text-gray-900 flex items-center gap-1">
                      <MapPin className="h-3 w-3 text-gray-400" />
                      {getDestination(result)}
                    </p>
                  </div>
                </div>

                {/* Contact */}
                {getContact(result) !== "N/A" && (
                  <div>
                    <p className="text-gray-500 text-xs">Contact</p>
                    <p className="font-medium text-gray-900 flex items-center gap-1">
                      <Phone className="h-3 w-3 text-gray-400" />
                      {getContact(result)}
                    </p>
                  </div>
                )}

                {/* Order Details */}
                <div className="border-t pt-3">
                  <p className="text-gray-500 text-xs mb-2">Order Details</p>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <p className="text-gray-500 text-xs">Item Name</p>
                      <p className="font-medium text-gray-900">{getItemName(result)}</p>
                    </div>
                    <div>
                      <p className="text-gray-500 text-xs">Tracking Number</p>
                      <p className="font-medium text-gray-900 text-xs break-all">
                        {getTrackingRef(result)}
                      </p>
                    </div>
                    <div>
                      <p className="text-gray-500 text-xs">Item Quantity</p>
                      <p className="font-medium text-gray-900">{getQuantity(result)}</p>
                    </div>
                    {result.type === "booking" && result.checkInDate && (
                      <div>
                        <p className="text-gray-500 text-xs">Check-in Date</p>
                        <p className="font-medium text-gray-900">
                          {new Date(result.checkInDate).toLocaleDateString()}
                        </p>
                      </div>
                    )}
                  </div>
                </div>

                {/* Company Details - Only for orders */}
                {result.type === "order" && getCompanyName(result) !== "N/A" && (
                  <div className="border-t pt-3">
                    <p className="text-gray-500 text-xs mb-2">Company Details</p>
                    <div className="space-y-1">
                      <div>
                        <p className="text-gray-500 text-xs">Company Name</p>
                        <p className="font-medium text-gray-900">
                          {getCompanyName(result)}
                        </p>
                      </div>
                      {getCompanyAddress(result) !== "N/A" && (
                        <div>
                          <p className="text-gray-500 text-xs">Company Address</p>
                          <p className="font-medium text-gray-900">
                            {getCompanyAddress(result)}
                          </p>
                        </div>
                      )}
                      {getCompanyContact(result) !== "N/A" && (
                        <div>
                          <p className="text-gray-500 text-xs">Company Contact</p>
                          <p className="font-medium text-gray-900">
                            {getCompanyContact(result)}
                          </p>
                        </div>
                      )}
                    </div>
                  </div>
                )}

                {/* Charges */}
                <div className="border-t pt-3">
                  <p className="text-gray-500 text-xs mb-2">Charges</p>
                  <div className="space-y-1">
                    <div className="flex justify-between">
                      <span className="text-gray-600 text-sm">Delivery Charges</span>
                      <span className="font-medium text-gray-900">
                        {formatCurrency(getDeliveryCharge(result))}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600 text-sm">Vat/Tax Services</span>
                      <span className="font-medium text-gray-900">
                        {formatCurrency(getVat(result))}
                      </span>
                    </div>
                    <div className="flex justify-between border-t pt-2 font-bold">
                      <span className="text-gray-900">Total Cost</span>
                      <span className="text-primary">
                        {formatCurrency(getTotal(result))}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Status Message for Pending */}
                {(getStatus(result).toLowerCase() === "pending" || 
                  getStatus(result).toLowerCase() === "waiting for pickup") && (
                  <div className="p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
                    <p className="text-sm text-yellow-800 flex items-center gap-2">
                      <Clock className="h-4 w-4 flex-shrink-0" />
                      Waiting for the company to update the shipping cost once they pick up the package
                    </p>
                  </div>
                )}

                {/* Action Buttons */}
                <div className="flex gap-2 pt-2">
                  <Button
                    variant="outline"
                    className="flex-1"
                    onClick={() => {
                      setResult(null);
                      setTrackingNumber("");
                      setError(null);
                    }}
                  >
                    <ArrowLeft className="h-4 w-4 mr-2" />
                    Back Home
                  </Button>
                  <Button
                    variant="default"
                    className="flex-1"
                    onClick={onClose}
                  >
                    Close
                  </Button>
                </div>

                {/* Status message for pending - bottom */}
                {(getStatus(result).toLowerCase() === "pending" || 
                  getStatus(result).toLowerCase() === "waiting for pickup") && (
                  <p className="text-center text-xs text-yellow-600 mt-2">
                    Waiting for the company to update the shipping cost
                  </p>
                )}
              </div>
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}