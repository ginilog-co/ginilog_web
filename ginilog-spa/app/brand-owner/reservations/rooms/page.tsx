// app/brand-owner/reservations/rooms/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Loader2, Plus, Eye, Edit, Trash2, Hotel, Users, DollarSign, Bed } from "lucide-react";
import { getReservationRooms, getAccommodationById, deleteReservationRoom } from "@/lib/api";

export default function ReservationRoomsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const accommodationId = searchParams.get("accommodationId");

  const [rooms, setRooms] = useState<any[]>([]);
  const [accommodation, setAccommodation] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    if (accommodationId) {
      fetchData();
    }
  }, [accommodationId]);

  const fetchData = async () => {
    setIsLoading(true);
    try {
      const [roomsData, accommodationData] = await Promise.all([
        getReservationRooms(accommodationId!),
        getAccommodationById(accommodationId!),
      ]);
      setRooms(roomsData || []);
      setAccommodation(accommodationData);
    } catch (error) {
      console.error("Failed to fetch rooms:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this room?")) return;
    
    setDeletingId(id);
    try {
      await deleteReservationRoom(id);
      await fetchData();
    } catch (error) {
      console.error("Failed to delete room:", error);
    } finally {
      setDeletingId(null);
    }
  };

  if (!accommodationId) {
    return (
      <div className="max-w-2xl mx-auto py-20 px-4 text-center">
        <Hotel className="h-12 w-12 text-gray-300 mx-auto mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">No Accommodation Selected</h2>
        <p className="text-gray-500 mb-6">Please select an accommodation from your reservations.</p>
        <Button onClick={() => router.push("/brand-owner/reservations")}>
          Go to Reservations
        </Button>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {accommodation?.accomodationName || "Accommodation"} Rooms
          </h1>
          <p className="text-sm text-gray-500">
            {rooms.length} room{rooms.length !== 1 ? "s" : ""} available
          </p>
        </div>
        <Link href={`/brand-owner/reservations/add-rooms?accommodationId=${accommodationId}`}>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Add Room
          </Button>
        </Link>
      </div>

      {rooms.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Bed className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No rooms added yet</p>
            <Link href={`/brand-owner/reservations/add-rooms?accommodationId=${accommodationId}`}>
              <Button className="mt-4 gap-2">
                <Plus className="h-4 w-4" />
                Add Your First Room
              </Button>
            </Link>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {rooms.map((room) => (
            <Card key={room.id} className="hover:shadow-md transition-shadow">
              <CardHeader>
                <CardTitle className="text-base flex items-center justify-between">
                  <span className="flex items-center gap-2">
                    <Hotel className="h-4 w-4 text-primary" />
                    Room {room.roomNumber}
                  </span>
                  <Badge className={room.isBooked ? "bg-red-100 text-red-800" : "bg-green-100 text-green-800"}>
                    {room.isBooked ? "Booked" : "Available"}
                  </Badge>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <Users className="h-4 w-4 text-gray-400" />
                  {room.maximumNoOfGuest} guests
                </div>
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <DollarSign className="h-4 w-4 text-gray-400" />
                  ₦{room.roomPrice?.toLocaleString() || 0}/night
                </div>
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <Bed className="h-4 w-4 text-gray-400" />
                  {room.roomType || "Standard"}
                </div>
                {room.roomFeatures && room.roomFeatures.length > 0 && (
                  <div className="flex flex-wrap gap-1">
                    {room.roomFeatures.slice(0, 3).map((feature: string) => (
                      <Badge key={feature} variant="outline" className="text-xs">
                        {feature}
                      </Badge>
                    ))}
                    {room.roomFeatures.length > 3 && (
                      <Badge variant="outline" className="text-xs">
                        +{room.roomFeatures.length - 3} more
                      </Badge>
                    )}
                  </div>
                )}
                <div className="flex items-center gap-2 pt-2 border-t">
                  <Link href={`/brand-owner/reservations/${room.id}`}>
                    <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                      <Eye className="h-4 w-4" />
                    </Button>
                  </Link>
                  <Link href={`/brand-owner/reservations/${room.id}/edit`}>
                    <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                      <Edit className="h-4 w-4" />
                    </Button>
                  </Link>
                  <Button
                    size="sm"
                    variant="ghost"
                    className="h-8 w-8 p-0 text-red-500 hover:text-red-700"
                    onClick={() => handleDelete(room.id)}
                    disabled={deletingId === room.id}
                  >
                    {deletingId === room.id ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <Trash2 className="h-4 w-4" />
                    )}
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}