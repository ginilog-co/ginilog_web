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
  AddCustomerBookedReservation,
  logout,
  clearAuthData
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
  User,
  Menu,
  X,
  LayoutDashboard,
  ShoppingBag,
  UserCircle,
  ChevronRight,
  Sparkles,
  LogOut as LogOutIcon,
  Star
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
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

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

  const handleLogout = () => {
    logout();
    clearAuthData();
    router.push("/customer-portal/login");
  };

  const getInitials = () => {
    if (!user) return "U";
    return `${user.firstName?.[0] || ""}${user.lastName?.[0] || ""}`.toUpperCase();
  };

  // Mobile menu items
  const menuItems = [
    { icon: LayoutDashboard, label: "Dashboard", href: "/customer-portal/dashboard" },
    { icon: ShoppingBag, label: "My Orders & Bookings", href: "/customer-portal/orders" },
    { icon: UserCircle, label: "Profile", href: "/customer-portal/profile" },
    { icon: LogOutIcon, label: "Logout", href: "#", isLogout: true },
  ];

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-50 shadow-sm">
        <div className="container mx-auto px-4 py-3">
          <div className="flex items-center justify-between">
            <Link href="/" className="text-2xl font-bold text-primary flex items-center gap-2">
             
              GINILOG
            </Link>

            {/* Desktop Navigation */}
            <nav className="hidden md:flex items-center space-x-8">
              <Link href="/customer-portal/dashboard" className="text-gray-600 hover:text-gray-900 transition-colors">
                Dashboard
              </Link>
              <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900 transition-colors">
                My Orders & Bookings
              </Link>
              <Link href="/customer-portal/profile" className="text-gray-600 hover:text-gray-900 transition-colors">
                Profile
              </Link>
            </nav>

            {/* Desktop Right Side */}
            <div className="hidden md:flex items-center gap-4">
              <Link href="/customer-portal/accommodations">
                <Button variant="ghost" size="sm" className="text-gray-600">
                  <ArrowLeft className="h-4 w-4 mr-2" /> Back
                </Button>
              </Link>
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full border-2 border-primary/20 overflow-hidden bg-primary/10 flex items-center justify-center flex-shrink-0">
                  {user?.profilePicture ? (
                    <img 
                      src={user.profilePicture} 
                      alt={user.firstName} 
                      className="h-full w-full object-cover" 
                    />
                  ) : (
                    <span className="text-primary font-semibold text-sm">
                      {getInitials()}
                    </span>
                  )}
                </div>
                <div className="hidden lg:block text-left">
                  <p className="text-sm font-medium text-gray-900">
                    {user?.firstName} {user?.lastName}
                  </p>
                  <p className="text-xs text-gray-500 capitalize">Customer</p>
                </div>
              </div>

              <Button
                variant="ghost"
                size="sm"
                className="text-red-600 hover:text-red-700 hover:bg-red-50"
                onClick={handleLogout}
              >
                <LogOut className="h-4 w-4 mr-2" />
                Logout
              </Button>
            </div>

            {/* Mobile Menu Button */}
            <button
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              className="md:hidden p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors focus:outline-none"
              aria-label="Toggle menu"
            >
              {isMobileMenuOpen ? (
                <X className="h-6 w-6" />
              ) : (
                <Menu className="h-6 w-6" />
              )}
            </button>
          </div>
        </div>

        {/* Mobile Menu Overlay */}
        {isMobileMenuOpen && (
          <div className="md:hidden fixed inset-0 z-50 bg-black/50" onClick={() => setIsMobileMenuOpen(false)}>
            <div className="absolute right-0 top-0 h-full w-80 max-w-[85vw] bg-white shadow-2xl" onClick={(e) => e.stopPropagation()}>
              <div className="p-4 border-b bg-gradient-to-r from-primary/5 to-blue-50">
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-full border-2 border-primary/20 overflow-hidden bg-primary/10 flex items-center justify-center flex-shrink-0">
                      {user?.profilePicture ? (
                        <img 
                          src={user.profilePicture} 
                          alt={user.firstName} 
                          className="h-full w-full object-cover" 
                        />
                      ) : (
                        <span className="text-primary font-semibold text-lg">
                          {getInitials()}
                        </span>
                      )}
                    </div>
                    <div>
                      <p className="font-semibold text-gray-900 text-base">
                        {user?.firstName} {user?.lastName}
                      </p>
                      <p className="text-xs text-gray-500">{user?.email}</p>
                    </div>
                  </div>
                  <button onClick={() => setIsMobileMenuOpen(false)} className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg">
                    <X className="h-5 w-5" />
                  </button>
                </div>
                <div className="flex items-center gap-2">
                  <Badge variant="secondary" className="text-xs">Active</Badge>
                  <span className="text-xs text-gray-500">•</span>
                  <span className="text-xs text-gray-500 capitalize">Customer</span>
                </div>
              </div>

              <nav className="p-4 space-y-1 overflow-y-auto" style={{ maxHeight: 'calc(100vh - 200px)' }}>
                <Link
                  href="/customer-portal/accommodations"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors group"
                >
                  <div className="flex items-center gap-3">
                    <div className="h-8 w-8 rounded-lg bg-gray-100 group-hover:bg-primary/10 flex items-center justify-center transition-colors">
                      <ArrowLeft className="h-4 w-4 text-gray-500 group-hover:text-primary transition-colors" />
                    </div>
                    <span className="text-gray-700 group-hover:text-primary font-medium transition-colors">
                      Back to Accommodations
                    </span>
                  </div>
                  <ChevronRight className="h-4 w-4 text-gray-400 group-hover:text-primary transition-colors" />
                </Link>

                {menuItems.map((item) => (
                  item.isLogout ? (
                    <button
                      key={item.label}
                      onClick={() => {
                        setIsMobileMenuOpen(false);
                        handleLogout();
                      }}
                      className="w-full flex items-center justify-between p-3 rounded-lg hover:bg-red-50 transition-colors group"
                    >
                      <div className="flex items-center gap-3">
                        <div className="h-8 w-8 rounded-lg bg-red-50 group-hover:bg-red-100 flex items-center justify-center transition-colors">
                          <item.icon className="h-4 w-4 text-red-600 group-hover:text-red-700 transition-colors" />
                        </div>
                        <span className="text-red-600 group-hover:text-red-700 font-medium transition-colors">
                          {item.label}
                        </span>
                      </div>
                      <ChevronRight className="h-4 w-4 text-red-400 group-hover:text-red-500 transition-colors" />
                    </button>
                  ) : (
                    <Link
                      key={item.href}
                      href={item.href}
                      onClick={() => setIsMobileMenuOpen(false)}
                      className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors group"
                    >
                      <div className="flex items-center gap-3">
                        <div className="h-8 w-8 rounded-lg bg-gray-100 group-hover:bg-primary/10 flex items-center justify-center transition-colors">
                          <item.icon className="h-4 w-4 text-gray-500 group-hover:text-primary transition-colors" />
                        </div>
                        <span className="text-gray-700 group-hover:text-primary font-medium transition-colors">
                          {item.label}
                        </span>
                      </div>
                      <ChevronRight className="h-4 w-4 text-gray-400 group-hover:text-primary transition-colors" />
                    </Link>
                  )
                ))}
              </nav>
            </div>
          </div>
        )}
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
                <div className="flex items-center justify-between">
                  <h1 className="text-3xl font-bold text-gray-900">Select a Room</h1>
                  <Link href="/customer-portal/accommodations" className="md:hidden">
                    <Button variant="outline" size="sm">
                      <ArrowLeft className="h-4 w-4 mr-2" /> Back
                    </Button>
                  </Link>
                </div>
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