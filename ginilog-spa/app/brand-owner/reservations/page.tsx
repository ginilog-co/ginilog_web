// app/brand-owner/reservations/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
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
  Mail,
  Hotel,
  Plus,
  Shield
} from "lucide-react";
import { 
  getAccommodationReservations, 
  updateAccommodationReservation, 
  getBrandOwnerAccommodations,
  getStoredUser,
  getToken
} from "@/lib/api";

export default function ReservationsPage() {
  const router = useRouter();
  const [reservations, setReservations] = useState<any[]>([]);
  const [filteredReservations, setFilteredReservations] = useState<any[]>([]);
  const [accommodations, setAccommodations] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [selectedAccommodation, setSelectedAccommodation] = useState<string>("all");
  const [currentUser, setCurrentUser] = useState<any>(null);

  useEffect(() => {
    const user = getStoredUser();
    setCurrentUser(user);
    fetchData();
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

    if (selectedAccommodation !== "all") {
      filtered = filtered.filter((r) => 
        r.accomodationId === selectedAccommodation || 
        r.accomodationName?.toLowerCase() === selectedAccommodation.toLowerCase()
      );
    }
    
    setFilteredReservations(filtered);
  }, [search, filter, reservations, selectedAccommodation]);

  const fetchData = async () => {
    try {
      setIsLoading(true);
      
      // Get current user
      const user = getStoredUser();
      const userId = user?.userId || user?.id || user?.Id;
      const companyId = user?.companyId || user?.CompanyId;
      const userEmail = user?.email;
      
      console.log("Current user:", { userId, companyId, userEmail });
      
      // Fetch only accommodations owned by the current brand owner
      const userAccommodations = await getBrandOwnerAccommodations(userId || "").catch(() => []);
      console.log("Brand owner accommodations:", userAccommodations.length);
      
      console.log(`✅ Found ${userAccommodations.length} accommodations for this brand owner`);
      setAccommodations(userAccommodations);
      
      // Get all reservations
      const allReservations = await getAccommodationReservations().catch(() => []);
      console.log("All reservations from API:", allReservations.length);
      
      // 🔐 SECURITY: Filter reservations to only those belonging to user's accommodations
      const userAccommodationIds = userAccommodations.map((acc: any) => acc.id);
      const userReservations = allReservations.filter((r: any) => {
        const belongsToUser = 
          userAccommodationIds.includes(r.accomodationId) ||
          userAccommodations.some((acc: any) => acc.accomodationName === r.accomodationName) ||
          r.managerId === userId ||
          r.userId === userId ||
          r.ownerId === userId;
        
        if (belongsToUser) {
          console.log(`✅ Reservation "${r.bookingRefNo}" belongs to user's property`);
        }
        return belongsToUser;
      });
      
      console.log(`✅ Found ${userReservations.length} reservations for this brand owner`);
      setReservations(userReservations || []);
      setFilteredReservations(userReservations || []);
      
    } catch (error) {
      console.error("Failed to fetch data:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusUpdate = async (id: string, status: string) => {
    setUpdatingId(id);
    try {
      await updateAccommodationReservation(id, { bookingStatus: status });
      await fetchData();
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

  const getUniqueAccommodations = () => {
    const names = reservations
      .map(r => r.accomodationName)
      .filter(Boolean);
    return [...new Set(names)];
  };

  // Handle navigation to add rooms page with accommodation pre-selected
  const handleAddRoom = (accommodationId?: string) => {
    if (accommodationId) {
      router.push(`/brand-owner/reservations/add-rooms?accommodationId=${accommodationId}`);
    } else {
      router.push("/brand-owner/reservations/add-rooms");
    }
  };

  // Handle navigation to manage rooms page with accommodation pre-selected
  const handleManageRooms = (accommodationId?: string) => {
    if (accommodationId) {
      router.push(`/brand-owner/reservations/rooms?accommodationId=${accommodationId}`);
    } else {
      router.push("/brand-owner/reservations/rooms");
    }
  };

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
          <p className="text-sm text-gray-500">Manage all bookings and reservations for your properties</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Link href="/brand-owner/reservations/rooms">
            <Button variant="outline" className="gap-2">
              <Hotel className="h-4 w-4" />
              Manage Rooms
            </Button>
          </Link>
          <Link href="/brand-owner/reservations/add-rooms">
            <Button className="gap-2">
              <Plus className="h-4 w-4" />
              Add Room
            </Button>
          </Link>
        </div>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-4">
        <div className="relative flex-1 min-w-[200px]">
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

        <select
          className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
          value={selectedAccommodation}
          onChange={(e) => setSelectedAccommodation(e.target.value)}
        >
          <option value="all">All Properties</option>
          {accommodations.map((acc) => (
            <option key={acc.id} value={acc.id}>
              {acc.accomodationName}
            </option>
          ))}
          {getUniqueAccommodations()
            .filter(name => !accommodations.some(a => a.accomodationName === name))
            .map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))
          }
        </select>

        <Button variant="outline" onClick={fetchData} className="gap-2">
          <Loader2 className={`h-4 w-4 ${isLoading ? 'animate-spin' : ''}`} />
          Refresh
        </Button>
      </div>

      {/* Security Notice - Only shown if user has no accommodations */}
      {accommodations.length === 0 && (
        <Card className="border-yellow-200 bg-yellow-50">
          <CardContent className="p-4 flex items-center gap-3">
            <Shield className="h-5 w-5 text-yellow-600" />
            <div>
              <p className="text-sm font-medium text-yellow-800">You don't have any properties yet</p>
              <p className="text-xs text-yellow-700">Add your first property to start managing reservations.</p>
            </div>
            <Link href="/brand-owner/properties/add" className="ml-auto">
              <Button size="sm" className="gap-2">
                <Plus className="h-4 w-4" />
                Add Property
              </Button>
            </Link>
          </CardContent>
        </Card>
      )}

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Total Reservations</p>
            <p className="text-2xl font-bold">{reservations.length}</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Pending</p>
            <p className="text-2xl font-bold text-yellow-600">
              {reservations.filter(r => r.bookingStatus?.toLowerCase() === "pending").length}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Confirmed</p>
            <p className="text-2xl font-bold text-green-600">
              {reservations.filter(r => r.bookingStatus?.toLowerCase() === "confirmed").length}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Properties</p>
            <p className="text-2xl font-bold text-blue-600">{accommodations.length}</p>
          </CardContent>
        </Card>
      </div>

      {/* Properties Quick Actions */}
      {accommodations.length > 0 && (
        <div className="flex flex-wrap gap-3">
          {accommodations.slice(0, 5).map((acc) => (
            <Card key={acc.id} className="flex-1 min-w-[180px] hover:shadow-md transition-shadow">
              <CardContent className="p-3">
                <p className="text-sm font-medium truncate">{acc.accomodationName}</p>
                <p className="text-xs text-gray-500 mt-1">ID: {acc.id?.slice(0, 8)}...</p>
                <div className="flex gap-2 mt-2">
                  <Button 
                    size="sm" 
                    variant="outline" 
                    className="text-xs h-7 px-2"
                    onClick={() => handleAddRoom(acc.id)}
                  >
                    <Plus className="h-3 w-3 mr-1" />
                    Add Room
                  </Button>
                  <Button 
                    size="sm" 
                    variant="ghost" 
                    className="text-xs h-7 px-2"
                    onClick={() => handleManageRooms(acc.id)}
                  >
                    <Hotel className="h-3 w-3 mr-1" />
                    Rooms
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
          {accommodations.length > 5 && (
            <Card className="flex items-center justify-center min-w-[100px]">
              <CardContent className="p-3 text-center">
                <p className="text-sm font-medium">+{accommodations.length - 5} more</p>
                <Button 
                  size="sm" 
                  variant="link" 
                  className="text-xs h-7 px-0"
                  onClick={() => handleManageRooms()}
                >
                  View All
                </Button>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      {filteredReservations.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Calendar className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No reservations found for your properties</p>
            {accommodations.length > 0 && reservations.length === 0 && (
              <p className="text-sm text-gray-400 mt-2">You haven't received any bookings yet.</p>
            )}
            {reservations.length > 0 && (
              <Button 
                variant="outline" 
                className="mt-4"
                onClick={() => {
                  setSearch("");
                  setFilter("all");
                  setSelectedAccommodation("all");
                }}
              >
                Clear Filters
              </Button>
            )}
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