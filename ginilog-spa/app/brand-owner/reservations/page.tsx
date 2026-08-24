// app/brand-owner/reservations/page.tsx
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
  Calendar,
  Users,
  DollarSign,
  CheckCircle,
  Clock,
  XCircle,
  Filter,
  ArrowRight,
  MapPin,
  Phone,
  Mail
} from "lucide-react";
import { getAccommodationReservations, updateAccommodationReservation } from "@/lib/api";

export default function ReservationsPage() {
  const [reservations, setReservations] = useState<any[]>([]);
  const [filteredReservations, setFilteredReservations] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [updatingId, setUpdatingId] = useState<string | null>(null);

  useEffect(() => {
    fetchReservations();
  }, []);

  useEffect(() => {
    let filtered = reservations;
    
    if (search.trim()) {
      filtered = filtered.filter((r) =>
        r.guestName?.toLowerCase().includes(search.toLowerCase()) ||
        r.customerName?.toLowerCase().includes(search.toLowerCase()) ||
        r.accomodationName?.toLowerCase().includes(search.toLowerCase()) ||
        r.bookingRefNo?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter !== "all") {
      filtered = filtered.filter((r) => 
        r.bookingStatus?.toLowerCase() === filter.toLowerCase()
      );
    }
    
    setFilteredReservations(filtered);
  }, [search, filter, reservations]);

  const fetchReservations = async () => {
    try {
      setIsLoading(true);
      const data = await getAccommodationReservations();
      setReservations(data || []);
      setFilteredReservations(data || []);
    } catch (error) {
      console.error("Failed to fetch reservations:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusUpdate = async (id: string, status: string) => {
    setUpdatingId(id);
    try {
      await updateAccommodationReservation(id, { bookingStatus: status });
      await fetchReservations();
    } catch (error) {
      console.error("Failed to update reservation:", error);
    } finally {
      setUpdatingId(null);
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

  const statusOptions = ["Pending", "Confirmed", "Completed", "Cancelled"];

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
          <h1 className="text-2xl font-bold text-gray-900">Reservations</h1>
          <p className="text-sm text-gray-500">Manage all bookings and reservations</p>
        </div>
      </div>

      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            placeholder="Search reservations..."
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
          <option value="pending">Pending</option>
          <option value="confirmed">Confirmed</option>
          <option value="completed">Completed</option>
          <option value="cancelled">Cancelled</option>
        </select>
      </div>

      {filteredReservations.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Calendar className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No reservations found</p>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0 overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-gray-50">
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Guest</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Property</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Dates</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Amount</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredReservations.map((res) => {
                  const status = getStatusBadge(res.bookingStatus);
                  const StatusIcon = status.icon;
                  return (
                    <tr key={res.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div>
                          <p className="font-medium">{res.guestName || res.customerName || "Guest"}</p>
                          <p className="text-xs text-gray-500">{res.guestEmail || res.customerEmail || "N/A"}</p>
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <div>
                          <p className="font-medium">{res.accomodationName || "Property"}</p>
                          <p className="text-xs text-gray-500">Room {res.roomNumber || "N/A"}</p>
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <div className="text-sm">
                          <p>{res.checkInDate ? new Date(res.checkInDate).toLocaleDateString() : "N/A"}</p>
                          <p className="text-gray-500">to</p>
                          <p>{res.checkOutDate ? new Date(res.checkOutDate).toLocaleDateString() : "N/A"}</p>
                        </div>
                      </td>
                      <td className="py-3 px-4 font-medium">
                        ₦{res.totalAmount?.toLocaleString() || 0}
                      </td>
                      <td className="py-3 px-4">
                        <Badge className={`${status.className} flex items-center gap-1 w-fit`}>
                          <StatusIcon className="h-3 w-3" />
                          {status.label}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-2">
                          <select
                            className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
                            value={res.bookingStatus || "Pending"}
                            onChange={(e) => handleStatusUpdate(res.id, e.target.value)}
                            disabled={updatingId === res.id}
                          >
                            {statusOptions.map((s) => (
                              <option key={s} value={s}>{s}</option>
                            ))}
                          </select>
                          <Link href={`/brand-owner/reservations/${res.id}`}>
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