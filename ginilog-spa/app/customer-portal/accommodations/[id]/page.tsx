"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";
import { 
  getRooms, 
  bookAccommodation, 
  getStoredUser, 
  getProfile,
  initializePaystackPayment,
  initializeFlutterwavePayment,
  AddCustomerBookedReservation,
  logout,
  clearAuthData,
  UserProfile
} from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { 
  Loader2, 
  CheckCircle2, 
  AlertCircle,
  ArrowLeft,
  Home,
  LogOut,
  Menu,
  X,
  LayoutDashboard,
  ShoppingBag,
  UserCircle,
  ChevronRight,
  LogOut as LogOutIcon,
  Phone,
  ChevronLeft,
  ChevronRight as ChevronRightIcon,
  CreditCard,
  Wallet
} from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";

// Helper function to generate unique reference
const generateReference = () => {
  return `REF-${Date.now()}-${Math.random().toString(36).substr(2, 6)}`;
};

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
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<string>("");
  const [isProcessingPayment, setIsProcessingPayment] = useState(false);

  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);

  const [formData, setFormData] = useState({
    roomId: "",
    startDate: "",
    endDate: "",
    guests: 1,
    customerPhone: "",
    comment: ""
  });

  // Calculate total price
  const calculateTotalPrice = () => {
    if (!formData.startDate || !formData.endDate) return 0;
    const start = new Date(formData.startDate);
    const end = new Date(formData.endDate);
    const nights = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
    const selectedRoom = rooms.find(r => r.id === formData.roomId);
    return nights * (selectedRoom?.roomPrice || 0);
  };

  // Get the selected room
  const getSelectedRoom = () => {
    return rooms.find(r => r.id === formData.roomId);
  };

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

  // Get current rooms for pagination
  const indexOfLastRoom = currentPage * itemsPerPage;
  const indexOfFirstRoom = indexOfLastRoom - itemsPerPage;
  const currentRooms = rooms.slice(indexOfFirstRoom, indexOfLastRoom);
  const totalPages = Math.ceil(rooms.length / itemsPerPage);

  // Change page
  const paginate = (pageNumber: number) => setCurrentPage(pageNumber);

  // Go to next page
  const nextPage = () => {
    if (currentPage < totalPages) {
      setCurrentPage(currentPage + 1);
    }
  };

  // Go to previous page
  const prevPage = () => {
    if (currentPage > 1) {
      setCurrentPage(currentPage - 1);
    }
  };

  // Initialize payment with backend API
  const initializePayment = async () => {
    setIsProcessingPayment(true);
    setError(null);

    try {
      const totalAmount = calculateTotalPrice();
      const reference = generateReference();
      const noOfDays = Math.ceil(
        (new Date(formData.endDate).getTime() - new Date(formData.startDate).getTime()) /
          (1000 * 60 * 60 * 24)
      );

      const paystackPayload = {
        customerName: `${user?.firstName || ''} ${user?.lastName || ''} ${user?.email || ''}`.trim(),
        customerPhoneNumber: formData.customerPhone,
        customerEmail: user?.email || '',
        numberOfGuests: formData.guests,
        trnxReference: reference,
        paymentChannel: 'paystack',
        paymentStatus: true,
        comment: formData.comment,
        ticketClosed: true,
        reservationStartDate: formData.startDate,
        reservationEndDate: formData.endDate,
        noOfDays,
        reservationId: formData.roomId,
       
        staffId: user?.id || '',
        staffName: `${user?.firstName || ''} ${user?.lastName || ''}`.trim(),
        purchaseChannel: 'web',
        userType: user?.userType || 'Registred User',
      };

      const flutterwavePayload = {
       customerName: `${user?.firstName || ''} ${user?.lastName || ''} ${user?.email || ''}`.trim(),
        customerPhoneNumber: formData.customerPhone,
        customerEmail: user?.email || '',
        numberOfGuests: formData.guests,
        trnxReference: reference,
        paymentChannel: 'paystack',
        paymentStatus: true,
        comment: formData.comment,
        ticketClosed: true,
        reservationStartDate: formData.startDate,
        reservationEndDate: formData.endDate,
        noOfDays,
        reservationId: formData.roomId,
       
        staffId: user?.id || '',
        staffName: `${user?.firstName || ''} ${user?.lastName || ''}`.trim(),
        purchaseChannel: 'web',
        userType: user?.userType || 'Registred User',
      };

      let data: any;
      if (selectedPaymentMethod === 'flutterwave') {
        data = await initializeFlutterwavePayment(flutterwavePayload);
      } else {
        data = await initializePaystackPayment(paystackPayload);
      }

      if (!data) {
        throw new Error('Failed to initialize payment');
      }

      const result = data.data ?? data;
      const paymentUrl =
        result?.authorizationUrl ||
        result?.link ||
        result?.url ||
        result?.payment_link;

      if (!paymentUrl) {
        console.error('Unexpected payment init response:', data);
        throw new Error('No payment link received');
      }

      window.location.href = paymentUrl;

    } catch (err) {
      console.error('Payment initialization error:', err);
      setError(err instanceof Error ? err.message : "Failed to initialize payment. Please try again.");
      setIsProcessingPayment(false);
    }
  };

  // Handle payment method selection and proceed
  const handleProceedToPayment = async () => {
    if (!selectedPaymentMethod) {
      setError("Please select a payment method");
      return;
    }

    await initializePayment();
  };

  // Handle confirm booking - show confirmation modal first
  const handleConfirmBooking = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate form
    if (!formData.customerPhone || formData.customerPhone.trim() === "") {
      setError("Phone number is required");
      return;
    }
    if (!formData.startDate) {
      setError("Check-in date is required");
      return;
    }
    if (!formData.endDate) {
      setError("Check-out date is required");
      return;
    }
    if (!formData.roomId) {
      setError("Please select a room");
      return;
    }

    // Check if user has email
    if (!user?.email) {
      setError("Please update your profile with an email address");
      return;
    }

    // Show confirmation modal
    setShowConfirmModal(true);
    setError(null);
  };

  // Handle confirmation - then show payment modal
  const handleConfirmAndProceedToPayment = () => {
    setShowConfirmModal(false);
    setShowPaymentModal(true);
    setError(null);
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
                
                {/* Rooms List */}
                {currentRooms.map((room) => (
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

                {/* Pagination Controls */}
                {rooms.length > itemsPerPage && (
                  <div className="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 rounded-lg shadow-sm">
                    <div className="flex flex-1 justify-between sm:hidden">
                      <Button
                        onClick={prevPage}
                        disabled={currentPage === 1}
                        variant="outline"
                        size="sm"
                      >
                        Previous
                      </Button>
                      <Button
                        onClick={nextPage}
                        disabled={currentPage === totalPages}
                        variant="outline"
                        size="sm"
                      >
                        Next
                      </Button>
                    </div>
                    <div className="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
                      <div>
                        <p className="text-sm text-gray-700">
                          Showing <span className="font-medium">{indexOfFirstRoom + 1}</span> to{" "}
                          <span className="font-medium">
                            {Math.min(indexOfLastRoom, rooms.length)}
                          </span>{" "}
                          of <span className="font-medium">{rooms.length}</span> rooms
                        </p>
                      </div>
                      <div>
                        <nav className="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                          <Button
                            onClick={prevPage}
                            disabled={currentPage === 1}
                            variant="outline"
                            size="sm"
                            className="rounded-l-md"
                          >
                            <span className="sr-only">Previous</span>
                            <ChevronLeft className="h-4 w-4" aria-hidden="true" />
                          </Button>
                          {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                            <Button
                              key={page}
                              onClick={() => paginate(page)}
                              variant={currentPage === page ? "default" : "outline"}
                              size="sm"
                              className={`relative z-10 ${
                                currentPage === page
                                  ? "bg-primary text-white hover:bg-primary/90"
                                  : "text-gray-900 hover:bg-gray-50"
                              }`}
                            >
                              {page}
                            </Button>
                          ))}
                          <Button
                            onClick={nextPage}
                            disabled={currentPage === totalPages}
                            variant="outline"
                            size="sm"
                            className="rounded-r-md"
                          >
                            <span className="sr-only">Next</span>
                            <ChevronRightIcon className="h-4 w-4" aria-hidden="true" />
                          </Button>
                        </nav>
                      </div>
                    </div>
                  </div>
                )}
              </div>

              <div className="lg:col-span-1">
                <Card className="sticky top-24">
                  <CardHeader>
                    <CardTitle>Book Your Stay</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleConfirmBooking} className="space-y-4">
                      {error && (
                        <div className="p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-center gap-2">
                          <AlertCircle className="h-4 w-4" />
                          {error}
                        </div>
                      )}
                      
                      <div className="space-y-2">
                        <Label htmlFor="customerPhone" className="flex items-center gap-2">
                          <Phone className="h-4 w-4 text-gray-500" />
                          Phone Number <span className="text-red-500">*</span>
                        </Label>
                        <Input 
                          id="customerPhone"
                          type="tel" 
                          required 
                          placeholder="Enter your phone number"
                          value={formData.customerPhone}
                          onChange={(e) => setFormData({ ...formData, customerPhone: e.target.value })}
                        />
                      </div>
                      
                      <div className="space-y-2">
                        <Label>Check-in Date <span className="text-red-500">*</span></Label>
                        <Input 
                          type="date" 
                          required 
                          value={formData.startDate}
                          onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                        />
                      </div>
                      
                      <div className="space-y-2">
                        <Label>Check-out Date <span className="text-red-500">*</span></Label>
                        <Input 
                          type="date" 
                          required 
                          value={formData.endDate}
                          onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                        />
                      </div>

                      <div className="space-y-2">
                        <Label>Number of Guests <span className="text-red-500">*</span></Label>
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

                      {/* Show total price if dates are selected */}
                      {formData.startDate && formData.endDate && formData.roomId && (
                        <div className="p-3 bg-primary/5 rounded-md border border-primary/20">
                          <div className="flex justify-between items-center">
                            <span className="text-sm font-medium text-gray-600">Total Amount:</span>
                            <span className="text-xl font-bold text-primary">₦{calculateTotalPrice().toLocaleString()}</span>
                          </div>
                          <p className="text-xs text-gray-500 mt-1">* Payment will be processed after confirming</p>
                        </div>
                      )}

                      <Button 
                        type="submit" 
                        className="w-full h-12" 
                        disabled={!formData.roomId || isBooking}
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

      {/* Confirmation Modal */}
      <Dialog open={showConfirmModal} onOpenChange={setShowConfirmModal}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-center">Confirm Booking</DialogTitle>
            <DialogDescription className="text-center">
              Please review your booking details before proceeding to payment
            </DialogDescription>
          </DialogHeader>
          
          <div className="py-6">
            {/* Order Summary */}
            <div className="bg-gray-50 rounded-lg p-4">
              <h4 className="font-semibold text-gray-700 mb-2">Booking Summary</h4>
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Room:</span>
                  <span className="font-medium">{rooms.find(r => r.id === formData.roomId)?.roomType || "Selected Room"}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Check-in:</span>
                  <span className="font-medium">{formData.startDate}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Check-out:</span>
                  <span className="font-medium">{formData.endDate}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Guests:</span>
                  <span className="font-medium">{formData.guests}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Phone:</span>
                  <span className="font-medium">{formData.customerPhone}</span>
                </div>
                {formData.comment && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Special Requests:</span>
                    <span className="font-medium text-right max-w-[60%]">{formData.comment}</span>
                  </div>
                )}
                <div className="flex justify-between pt-2 border-t border-gray-200">
                  <span className="font-semibold text-gray-700">Total:</span>
                  <span className="font-bold text-primary">₦{calculateTotalPrice().toLocaleString()}</span>
                </div>
              </div>
            </div>

            {error && (
              <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-center gap-2">
                <AlertCircle className="h-4 w-4" />
                {error}
              </div>
            )}
          </div>

          <DialogFooter className="flex flex-col sm:flex-row gap-3">
            <Button
              variant="outline"
              onClick={() => {
                setShowConfirmModal(false);
                setError(null);
              }}
              className="w-full sm:w-auto"
            >
              Cancel
            </Button>
            <Button
              onClick={handleConfirmAndProceedToPayment}
              className="w-full sm:w-auto"
            >
              Proceed to Payment
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Payment Method Modal */}
      <Dialog open={showPaymentModal} onOpenChange={setShowPaymentModal}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-center">Select Payment Method</DialogTitle>
            <DialogDescription className="text-center">
              Choose your preferred payment method to complete the booking
            </DialogDescription>
          </DialogHeader>
          
          <div className="py-6">
            {/* Order Summary */}
            <div className="bg-gray-50 rounded-lg p-4 mb-6">
              <h4 className="font-semibold text-gray-700 mb-2">Booking Summary</h4>
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Room:</span>
                  <span className="font-medium">{rooms.find(r => r.id === formData.roomId)?.roomType || "Selected Room"}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Check-in:</span>
                  <span className="font-medium">{formData.startDate}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Check-out:</span>
                  <span className="font-medium">{formData.endDate}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Guests:</span>
                  <span className="font-medium">{formData.guests}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Phone:</span>
                  <span className="font-medium">{formData.customerPhone}</span>
                </div>
                <div className="flex justify-between pt-2 border-t border-gray-200">
                  <span className="font-semibold text-gray-700">Total:</span>
                  <span className="font-bold text-primary">₦{calculateTotalPrice().toLocaleString()}</span>
                </div>
              </div>
            </div>

            <RadioGroup 
              value={selectedPaymentMethod} 
              onValueChange={setSelectedPaymentMethod}
              className="space-y-3"
            >
              <div 
                className={`flex items-center space-x-3 border-2 rounded-lg p-4 cursor-pointer transition-all ${
                  selectedPaymentMethod === "flutterwave" 
                    ? "border-primary bg-primary/5" 
                    : "border-gray-200 hover:border-gray-300"
                }`}
                onClick={() => setSelectedPaymentMethod("flutterwave")}
              >
                <RadioGroupItem value="flutterwave" id="flutterwave" />
                <div className="flex items-center gap-3 flex-1">
                  <div className="h-10 w-10 rounded-lg bg-blue-50 flex items-center justify-center">
                    <CreditCard className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <Label htmlFor="flutterwave" className="font-semibold cursor-pointer">
                      Flutterwave
                    </Label>
                    <p className="text-xs text-gray-500">Pay with card, bank transfer, or USSD</p>
                  </div>
                </div>
              </div>

              <div 
                className={`flex items-center space-x-3 border-2 rounded-lg p-4 cursor-pointer transition-all ${
                  selectedPaymentMethod === "paystack" 
                    ? "border-primary bg-primary/5" 
                    : "border-gray-200 hover:border-gray-300"
                }`}
                onClick={() => setSelectedPaymentMethod("paystack")}
              >
                <RadioGroupItem value="paystack" id="paystack" />
                <div className="flex items-center gap-3 flex-1">
                  <div className="h-10 w-10 rounded-lg bg-green-50 flex items-center justify-center">
                    <Wallet className="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <Label htmlFor="paystack" className="font-semibold cursor-pointer">
                      Paystack
                    </Label>
                    <p className="text-xs text-gray-500">Secure payment with cards, bank accounts, or QR</p>
                  </div>
                </div>
              </div>
            </RadioGroup>

            {error && (
              <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-center gap-2">
                <AlertCircle className="h-4 w-4" />
                {error}
              </div>
            )}
          </div>

          <DialogFooter className="flex flex-col sm:flex-row gap-3">
            <Button
              variant="outline"
              onClick={() => {
                setShowPaymentModal(false);
                setSelectedPaymentMethod("");
                setError(null);
              }}
              className="w-full sm:w-auto"
            >
              Cancel
            </Button>
            <Button
              onClick={handleProceedToPayment}
              disabled={!selectedPaymentMethod || isProcessingPayment}
              className="w-full sm:w-auto"
            >
              {isProcessingPayment ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing...
                </>
              ) : (
                "Pay Now"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}