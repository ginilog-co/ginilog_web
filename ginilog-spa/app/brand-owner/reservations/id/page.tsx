// app/brand-owner/reservations/[id]/page.tsx
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
  Calendar,
  Users,
  DollarSign,
  Mail,
  Phone,
  MapPin,
  Building2,
  CheckCircle,
  Clock,
  XCircle,
  Edit,
  Trash2,
  User,
  Copy,
  Share2
} from "lucide-react";
import { getReservationById, updateReservation, deleteReservation } from "@/lib/api";

export default function ReservationDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [reservation, setReservation] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [updating, setUpdating] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetchReservation = async () => {
      try {
        setIsLoading(true);
        const data = await getReservationById(params.id as string);
        setReservation(data);
      } catch (error) {
        console.error("Failed to fetch reservation:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchReservation();
  }, [params.id]);

  const handleStatusUpdate = async (status: string) => {
    setUpdating(true);
    try {
      await updateReservation(params.id as string, { bookingStatus: status });
      await fetchReservation();
    } catch (error) {
      console.error("Failed to update reservation:", error);
    } finally {
      setUpdating(false);
    }
  };

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this reservation?")) return;
    setDeleting(true);
    try {
      await deleteReservation(params.id as string);
      router.push("/brand-owner/reservations");
    } catch (error) {
      console.error("Failed to delete reservation:", error);
    } finally {
      setDeleting(false);
    }
  };

  const fetchReservation = async () => {
    try {
      const data = await getReservationById(params.id as string);
      setReservation(data);
    } catch (error) {
      console.error("Failed to fetch reservation:", error);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      confirmed: { label: "Confirmed", className: "bg-green-100 text-green-800", icon: CheckCircle },
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800", icon: Clock },
      completed: { label: "Completed", className: "bg-blue-100 text-blue-800", icon: CheckCircle },
      cancelled: { label: "Cancelled", className: "bg-red-100 text-red-800", icon: XCircle },
    };
    return statusMap[status?.toLowerCase()] || statusMap.pending;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!reservation) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Reservation not found</p>
        <Link href="/brand-owner/reservations">
          <Button className="mt-4">Back to Reservations</Button>
        </Link>
      </div>
    );
  }

  const status = getStatusBadge(reservation.bookingStatus);
  const StatusIcon = status.icon;

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/reservations">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Reservation Details</h1>
          <p className="text-sm text-gray-500">Booking reference: {reservation.bookingRefNo || "N/A"}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Info */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5 text-primary" />
              Guest Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Guest Name</p>
                <p className="font-semibold text-gray-900">{reservation.guestName || reservation.customerName || "N/A"}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Number of Guests</p>
                <p className="font-semibold text-gray-900">{reservation.numberOfGuests || 1}</p>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Email</p>
                <p className="font-semibold text-gray-900">{reservation.guestEmail || reservation.customerEmail || "N/A"}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Phone</p>
                <p className="font-semibold text-gray-900">{reservation.guestPhoneNo || reservation.customerPhoneNumber || "N/A"}</p>
              </div>
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
              <Badge className={`${status.className} flex items-center gap-1 justify-center text-sm py-1.5`}>
                <StatusIcon className="h-4 w-4" />
                {status.label}
              </Badge>
            </div>

            <div className="space-y-2">
              <select
                className="w-full px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={reservation.bookingStatus || "Pending"}
                onChange={(e) => handleStatusUpdate(e.target.value)}
                disabled={updating}
              >
                <option value="Pending">Pending</option>
                <option value="Confirmed">Confirmed</option>
                <option value="Completed">Completed</option>
                <option value="Cancelled">Cancelled</option>
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
              Delete Reservation
            </Button>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Property Info */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5 text-primary" />
              Property Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Property</p>
              <p className="font-medium">{reservation.accomodationName || "N/A"}</p>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Room Type</p>
                <p className="font-medium">{reservation.roomType || "N/A"}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Room Number</p>
                <p className="font-medium">{reservation.roomNumber || "N/A"}</p>
              </div>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-600">
              <MapPin className="h-4 w-4 text-gray-400" />
              {reservation.accomodationAddress || reservation.location || "N/A"}
            </div>
          </CardContent>
        </Card>

        {/* Booking Details */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Calendar className="h-5 w-5 text-primary" />
              Booking Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Check In</p>
                <p className="font-medium">{reservation.checkInDate ? new Date(reservation.checkInDate).toLocaleDateString() : "N/A"}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Check Out</p>
                <p className="font-medium">{reservation.checkOutDate ? new Date(reservation.checkOutDate).toLocaleDateString() : "N/A"}</p>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Nights</p>
                <p className="font-medium">{reservation.numberOfNights || "N/A"}</p>
              </div>
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Total Amount</p>
                <p className="font-medium text-primary">₦{reservation.totalAmount?.toLocaleString() || 0}</p>
              </div>
            </div>
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Payment Status</p>
              <Badge className={reservation.paymentStatus ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}>
                {reservation.paymentStatus ? "Paid" : "Unpaid"}
              </Badge>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}