"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";
import { 
  getRooms, 
  bookAccommodation, 
  getStoredUser, 
  getProfile,
  UserProfile,
  AddCustomerBookedReservation
} from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { 
  Loader2, 
  MapPin, 
  Calendar, 
  Users, 
  CheckCircle2, 
  AlertCircle,
  ArrowLeft,
  Home,
  LogOut,
  Bell,
  User
} from "lucide-react";

export default function AccommodationDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const accommodationId = params.id as string;

  const [rooms, setRooms] = useState<any[]>([]);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isBooking, setIsBooking] = useState(false);
  const [bookingSuccess, setBookingSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    roomId: "",
    startDate: "",
    endDate: "",
    guests: 1,
    comment: ""
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const storedUser = getStoredUser();
        if (!storedUser) {
          router.push("/customer-portal/login");
          return;
        }
        const [profile, roomList] = await Promise.all([
          getProfile(),
          getRooms(accommodationId)
        ]);
        setUser(profile);
        setRooms(roomList || []);
      } catch (err) {
        console.error("Failed to fetch details:", err);
        setError("Failed to load accommodation details.");
      } finally {
        setIsLoading(false);
      }
    };

    if (accommodationId) fetchData();
  }, [accommodationId, router]);

  const handleBook = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;
    
    setIsBooking(true);
    setError(null);

    try {
      const bookingData: AddCustomerBookedReservation = {
        userId: user.id,
        customerName: `${user.firstName} ${user.lastName}`,
        customerEmail: user.email,
        customerPhoneNumber: user.phoneNo,
        numberOfGuests: formData.guests,
        reservationStartDate: formData.startDate,
        reservationEndDate: formData.endDate,
        comment: formData.comment,
        userType: "Registered"
      };

      await bookAccommodation(formData.roomId, bookingData);
      setBookingSuccess(true);
      setTimeout(() => router.push("/customer-portal/orders"), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Booking failed. Please try again.");
    } finally {
      setIsBooking(false);
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white border-b sticky top-0 z-50">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <Link href="/" className="text-2xl font-bold text-primary">
              GINILOG
            </Link>
            <div className="flex items-center gap-4">
               <Link href="/customer-portal/accommodations">
                 <Button variant="ghost" size="sm">
                   <ArrowLeft className="h-4 w-4 mr-2" /> Back
                 </Button>
               </Link>
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        <div className="max-w-5xl mx-auto">
          {bookingSuccess ? (
            <Card className="text-center p-12">
              <div className="h-20 w-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
                <CheckCircle2 className="h-10 w-10 text-green-600" />
              </div>
              <h2 className="text-3xl font-bold text-gray-900 mb-2">Booking Successful!</h2>
              <p className="text-gray-600">Your reservation has been received. Redirecting to your orders...</p>
            </Card>
          ) : (
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
              <div className="lg:col-span-2 space-y-6">
                <h1 className="text-3xl font-bold text-gray-900">Select a Room</h1>
                {rooms.map((room) => (
                  <Card 
                    key={room.id} 
                    className={`cursor-pointer transition-all border-2 ${formData.roomId === room.id ? 'border-primary bg-primary/5' : 'border-transparent hover:border-gray-200'}`}
                    onClick={() => setFormData({ ...formData, roomId: room.id })}
                  >
                    <CardContent className="p-6">
                      <div className="flex flex-col md:flex-row gap-6">
                        <div className="w-full md:w-48 h-32 rounded-lg overflow-hidden flex-shrink-0 bg-gray-100">
                          {room.roomImages?.[0] ? (
                            <img src={room.roomImages[0]} alt={room.roomType} className="w-full h-full object-cover" />
                          ) : (
                            <div className="w-full h-full flex items-center justify-center text-gray-400">
                              <Home className="h-8 w-8" />
                            </div>
                          )}
                        </div>
                        <div className="flex-1">
                          <div className="flex items-center justify-between mb-2">
                            <h3 className="text-xl font-bold text-gray-900">{room.roomType}</h3>
                            <Badge variant={room.isBooked ? "destructive" : "default"}>
                              {room.isBooked ? "Unavailable" : "Available"}
                            </Badge>
                          </div>
                          <p className="text-sm text-gray-500 mb-4">Room #{room.roomNumber} • Max Guests: {room.maximumNoOfGuest}</p>
                          <div className="flex items-center justify-between mt-auto">
                            <span className="text-2xl font-bold text-primary">₦{room.roomPrice?.toLocaleString()}<small className="text-sm font-normal text-gray-500">/night</small></span>
                            {!room.isBooked && (
                              <Button variant={formData.roomId === room.id ? "default" : "outline"} size="sm">
                                {formData.roomId === room.id ? "Selected" : "Select Room"}
                              </Button>
                            )}
                          </div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>

              <div className="lg:col-span-1">
                <Card className="sticky top-24">
                  <CardHeader>
                    <CardTitle>Book Your Stay</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleBook} className="space-y-4">
                      {error && (
                        <div className="p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-center gap-2">
                          <AlertCircle className="h-4 w-4" />
                          {error}
                        </div>
                      )}
                      
                      <div className="space-y-2">
                        <Label>Check-in Date</Label>
                        <Input 
                          type="date" 
                          required 
                          value={formData.startDate}
                          onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                        />
                      </div>
                      
                      <div className="space-y-2">
                        <Label>Check-out Date</Label>
                        <Input 
                          type="date" 
                          required 
                          value={formData.endDate}
                          onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Number of Guests</Label>
                        <Input 
                          type="number" 
                          min="1" 
                          required 
                          value={formData.guests}
                          onChange={(e) => setFormData({ ...formData, guests: parseInt(e.target.value) })}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Special Requests (Optional)</Label>
                        <textarea 
                          className="w-full p-2 border rounded-md text-sm min-h-[80px]"
                          placeholder="Any special requests?"
                          value={formData.comment}
                          onChange={(e) => setFormData({ ...formData, comment: e.target.value })}
                        />
                      </div>

                      <Button 
                        type="submit" 
                        className="w-full h-12" 
                        disabled={isBooking || !formData.roomId}
                      >
                        {isBooking ? (
                          <>
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Processing...
                          </>
                        ) : (
                          "Confirm Booking"
                        )}
                      </Button>
                      
                      {!formData.roomId && (
                        <p className="text-xs text-center text-gray-500 italic mt-2">Please select a room to continue</p>
                      )}
                    </form>
                  </CardContent>
                </Card>
              </div>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
