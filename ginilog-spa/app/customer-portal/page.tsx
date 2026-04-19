"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Search, Package, Home, Truck, Loader2, MapPin, Calendar, User, Phone, Mail, CheckCircle, Clock, AlertCircle } from "lucide-react";
import { trackParcelOrBooking, OrderTrackingResult, BookingTrackingResult } from "@/lib/api";

// Mock data based on traditional web app models
const accommodations = [
  {
    id: "1",
    accomodationName: "Grand Hotel",
    accomodationType: "Hotel",
    location: "Lagos, Nigeria",
    bookingAmount: 45000,
    accomodationImages: ["/service-1.jpg"],
    accomodationDescription: "Luxury hotel with modern amenities",
    rating: 4.5,
    image: "/service-1.jpg",
  },
  {
    id: "2",
    accomodationName: "Sunset Apartments",
    accomodationType: "Apartment",
    location: "Abuja, Nigeria",
    bookingAmount: 35000,
    accomodationImages: ["/service-2.jpg"],
    accomodationDescription: "Spacious apartments with city views",
    rating: 4.2,
    image: "/service-2.jpg",
  },
  {
    id: "3",
    accomodationName: "Beach Resort",
    accomodationType: "Resort",
    location: "Calabar, Nigeria",
    bookingAmount: 55000,
    accomodationImages: ["/service-3.jpg"],
    accomodationDescription: "Beachfront resort with pool access",
    rating: 4.8,
    image: "/service-3.jpg",
  },
];

const roomBookings = [
  {
    id: "1",
    accomodationName: "Grand Hotel",
    roomType: "Deluxe Suite",
    roomNumber: 101,
    roomPrice: 25000,
    accomodationImage: "/service-4.jpg",
    accomodationLocality: "Victoria Island",
    accomodationState: "Lagos",
  },
  {
    id: "2",
    accomodationName: "Sunset Apartments",
    roomType: "Standard Room",
    roomNumber: 205,
    roomPrice: 18000,
    accomodationImage: "/service-5.jpg",
    accomodationLocality: "Maitama",
    accomodationState: "Abuja",
  },
  {
    id: "3",
    accomodationName: "Beach Resort",
    roomType: "Ocean View",
    roomNumber: 301,
    roomPrice: 35000,
    accomodationImage: "/service-6.jpg",
    accomodationLocality: "Marina",
    accomodationState: "Calabar",
  },
  {
    id: "4",
    accomodationName: "City Lodge",
    roomType: "Executive Room",
    roomNumber: 405,
    roomPrice: 22000,
    accomodationImage: "/service-1.jpg",
    accomodationLocality: "Ikeja",
    accomodationState: "Lagos",
  },
];

const logisticsCompanies = [
  {
    id: "1",
    companyName: "Swift Logistics",
    companyLogo: "/service-1.jpg",
    companyInfo: "Fast and reliable delivery services",
    valueCharge: 1500,
  },
  {
    id: "2",
    companyName: "Express Delivery",
    companyLogo: "/service-2.jpg",
    companyInfo: "Nationwide coverage with tracking",
    valueCharge: 2000,
  },
  {
    id: "3",
    companyName: "Prime Couriers",
    companyLogo: "/service-3.jpg",
    companyInfo: "Same-day delivery specialists",
    valueCharge: 3000,
  },
  {
    id: "4",
    companyName: "Trusty Transport",
    companyLogo: "/service-4.jpg",
    companyInfo: "Secure handling for fragile items",
    valueCharge: 1800,
  },
];

export default function CustomerPortalHome() {
  const [searchId, setSearchId] = useState("");
  const [isSearching, setIsSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);
  const [trackingResult, setTrackingResult] = useState<{
    type: 'order' | 'booking';
    data: OrderTrackingResult | BookingTrackingResult;
  } | null>(null);

  const handleTracking = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchId.trim()) return;

    setIsSearching(true);
    setSearchError(null);
    setTrackingResult(null);

    try {
      const result = await trackParcelOrBooking(searchId.trim());
      setTrackingResult(result);
    } catch (err) {
      setSearchError(err instanceof Error ? err.message : "Tracking failed. Please try again.");
    } finally {
      setIsSearching(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      'Open': 'bg-blue-100 text-blue-800',
      'Accepted': 'bg-yellow-100 text-yellow-800',
      'Picked': 'bg-purple-100 text-purple-800',
      'Ongoing': 'bg-orange-100 text-orange-800',
      'Delivered': 'bg-green-100 text-green-800',
      'Completed': 'bg-green-100 text-green-800',
      'Closed': 'bg-gray-100 text-gray-800',
      'Rejected': 'bg-red-100 text-red-800',
      'pending': 'bg-yellow-100 text-yellow-800',
      'confirmed': 'bg-green-100 text-green-800',
      'checked_in': 'bg-blue-100 text-blue-800',
      'checked_out': 'bg-gray-100 text-gray-800',
      'cancelled': 'bg-red-100 text-red-800',
    };
    return statusColors[status] || 'bg-gray-100 text-gray-800';
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b sticky top-0 z-50">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/customer-portal" className="text-2xl font-bold text-primary">
            GINILOG
          </Link>
          <nav className="hidden md:flex items-center space-x-8">
            <Link href="/customer-portal" className="text-gray-900 font-medium">
              Home
            </Link>
            <Link href="/customer-portal/dashboard" className="text-gray-600 hover:text-gray-900">
              Dashboard
            </Link>
          </nav>
          <div className="flex items-center gap-4">
            <Link href="/customer-portal/login">
              <Button variant="outline">Sign In</Button>
            </Link>
            <Link href="/customer-portal/register">
              <Button>Get Started</Button>
            </Link>
          </div>
        </div>
      </header>

      {/* Hero Banner with Tracking Search */}
      <section className="relative bg-gradient-to-r from-primary/90 to-primary py-20">
        <div className="absolute inset-0 bg-black/20" />
        <div className="container mx-auto px-4 relative z-10">
          <div className="text-center mb-12">
            <h6 className="text-white/80 text-lg font-medium mb-2">Ginilog</h6>
            <h1 className="text-4xl md:text-5xl font-bold text-white mb-4">
              Relax Your Mind
            </h1>
            <p className="text-xl text-white/90 max-w-2xl mx-auto">
              We help customers find and reserve comfortable stays while also offering efficient 
              package delivery and tracking services — all in one convenient place.
            </p>
          </div>

          {/* Tracking Search Form */}
          <div className="max-w-2xl mx-auto">
            <div className="bg-white rounded-lg shadow-lg p-2">
              <form onSubmit={handleTracking} className="flex items-center gap-2">
                <div className="flex-1 relative">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    type="text"
                    value={searchId}
                    onChange={(e) => setSearchId(e.target.value)}
                    placeholder="Enter Tracking Number or Booking Reference"
                    className="pl-10 h-14 text-lg border-0 focus-visible:ring-0"
                    required
                  />
                </div>
                <Button type="submit" size="lg" className="h-14 px-8" disabled={isSearching}>
                  {isSearching ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Tracking...
                    </>
                  ) : (
                    "Track"
                  )}
                </Button>
              </form>
            </div>

            {/* Error Message */}
            {searchError && (
              <div className="mt-4 p-4 bg-red-50 border border-red-200 rounded-lg text-red-600">
                <div className="flex items-center gap-2">
                  <AlertCircle className="h-5 w-5" />
                  <span>{searchError}</span>
                </div>
              </div>
            )}

            {/* Tracking Results */}
            {trackingResult && trackingResult.type === 'order' && (
              <div className="mt-6 bg-white rounded-lg shadow-lg overflow-hidden">
                <div className="bg-gradient-to-r from-primary to-red-800 p-4 text-white">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">Parcel Tracking</h3>
                      <p className="text-white/80 text-sm">Tracking #: {(trackingResult.data as OrderTrackingResult).trackingNum}</p>
                    </div>
                    <Badge className={`${getStatusBadge((trackingResult.data as OrderTrackingResult).orderStatus)} text-sm px-3 py-1`}>
                      {(trackingResult.data as OrderTrackingResult).orderStatus}
                    </Badge>
                  </div>
                </div>

                <div className="p-6 space-y-6">
                  {/* Item Details */}
                  <div className="grid md:grid-cols-2 gap-4">
                    <div className="space-y-3">
                      <h4 className="font-semibold text-gray-900 flex items-center gap-2">
                        <Package className="h-4 w-4 text-primary" />
                        Item Details
                      </h4>
                      <div className="text-sm text-gray-600 space-y-1">
                        <p><span className="font-medium">Name:</span> {(trackingResult.data as OrderTrackingResult).itemName}</p>
                        <p><span className="font-medium">Description:</span> {(trackingResult.data as OrderTrackingResult).itemDescription}</p>
                        <p><span className="font-medium">Weight:</span> {(trackingResult.data as OrderTrackingResult).itemWeight} kg</p>
                        <p><span className="font-medium">Quantity:</span> {(trackingResult.data as OrderTrackingResult).itemQuantity}</p>
                      </div>
                    </div>

                    <div className="space-y-3">
                      <h4 className="font-semibold text-gray-900 flex items-center gap-2">
                        <Truck className="h-4 w-4 text-primary" />
                        Shipping Details
                      </h4>
                      <div className="text-sm text-gray-600 space-y-1">
                        <p><span className="font-medium">Company:</span> {(trackingResult.data as OrderTrackingResult).companyName}</p>
                        <p><span className="font-medium">Rider:</span> {(trackingResult.data as OrderTrackingResult).riderName || 'Not assigned'}</p>
                        <p><span className="font-medium">Expected Delivery:</span> {(trackingResult.data as OrderTrackingResult).expectedDeliveryTime || 'Pending'}</p>
                        <p><span className="font-medium">Current Location:</span> {(trackingResult.data as OrderTrackingResult).currentLocation || 'Not available'}</p>
                      </div>
                    </div>
                  </div>

                  {/* Sender & Receiver */}
                  <div className="grid md:grid-cols-2 gap-4 pt-4 border-t">
                    <div className="space-y-2">
                      <h4 className="font-semibold text-gray-900">Sender Information</h4>
                      <div className="text-sm text-gray-600">
                        <p className="font-medium">{(trackingResult.data as OrderTrackingResult).senderName}</p>
                        <p>{(trackingResult.data as OrderTrackingResult).senderAddress}</p>
                        <p>{(trackingResult.data as OrderTrackingResult).senderState}, {(trackingResult.data as OrderTrackingResult).senderLocality}</p>
                      </div>
                    </div>

                    <div className="space-y-2">
                      <h4 className="font-semibold text-gray-900">Receiver Information</h4>
                      <div className="text-sm text-gray-600">
                        <p className="font-medium">{(trackingResult.data as OrderTrackingResult).recieverName}</p>
                        <p>{(trackingResult.data as OrderTrackingResult).recieverAddress}</p>
                        <p>{(trackingResult.data as OrderTrackingResult).recieverState}, {(trackingResult.data as OrderTrackingResult).recieverLocality}</p>
                      </div>
                    </div>
                  </div>

                  {/* Delivery Timeline */}
                  {(trackingResult.data as OrderTrackingResult).orderDeliveryFlows && (trackingResult.data as OrderTrackingResult).orderDeliveryFlows.length > 0 && (
                    <div className="pt-4 border-t">
                      <h4 className="font-semibold text-gray-900 mb-4">Delivery Timeline</h4>
                      <div className="space-y-3">
                        {(trackingResult.data as OrderTrackingResult).orderDeliveryFlows.map((flow, index) => (
                          <div key={flow.id} className="flex items-start gap-3">
                            <div className="flex flex-col items-center">
                              <div className={`w-3 h-3 rounded-full ${index === 0 ? 'bg-primary' : 'bg-gray-300'}`} />
                              {index < (trackingResult.data as OrderTrackingResult).orderDeliveryFlows.length - 1 && (
                                <div className="w-0.5 h-8 bg-gray-200 mt-1" />
                              )}
                            </div>
                            <div className="flex-1 pb-4">
                              <p className="font-medium text-sm">{flow.orderStatus}</p>
                              <p className="text-xs text-gray-500">{flow.currentLocation}</p>
                              <p className="text-xs text-gray-400">{new Date(flow.updatedAt).toLocaleString()}</p>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Booking Result */}
            {trackingResult && trackingResult.type === 'booking' && (
              <div className="mt-6 bg-white rounded-lg shadow-lg overflow-hidden">
                <div className="bg-gradient-to-r from-blue-600 to-blue-800 p-4 text-white">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">Accommodation Booking</h3>
                      <p className="text-white/80 text-sm">Ref #: {(trackingResult.data as BookingTrackingResult).bookingRefNo}</p>
                    </div>
                    <Badge className={`${getStatusBadge((trackingResult.data as BookingTrackingResult).bookingStatus)} text-sm px-3 py-1`}>
                      {(trackingResult.data as BookingTrackingResult).bookingStatus}
                    </Badge>
                  </div>
                </div>

                <div className="p-6 space-y-6">
                  <div className="grid md:grid-cols-2 gap-6">
                    <div className="space-y-3">
                      <h4 className="font-semibold text-gray-900 flex items-center gap-2">
                        <Home className="h-4 w-4 text-primary" />
                        Accommodation Details
                      </h4>
                      <div className="text-sm text-gray-600 space-y-1">
                        <p className="font-medium text-base">{(trackingResult.data as BookingTrackingResult).accomodationName}</p>
                        <p>{(trackingResult.data as BookingTrackingResult).accomodationType}</p>
                        <p>Room: {(trackingResult.data as BookingTrackingResult).roomType} (#{(trackingResult.data as BookingTrackingResult).roomNumber})</p>
                        <p>{(trackingResult.data as BookingTrackingResult).accomodationAddress}</p>
                        <p>{(trackingResult.data as BookingTrackingResult).accomodationLocality}, {(trackingResult.data as BookingTrackingResult).accomodationState}</p>
                      </div>
                    </div>

                    <div className="space-y-3">
                      <h4 className="font-semibold text-gray-900 flex items-center gap-2">
                        <Calendar className="h-4 w-4 text-primary" />
                        Stay Details
                      </h4>
                      <div className="text-sm text-gray-600 space-y-1">
                        <p><span className="font-medium">Check-in:</span> {new Date((trackingResult.data as BookingTrackingResult).checkInDate).toLocaleDateString()}</p>
                        <p><span className="font-medium">Check-out:</span> {new Date((trackingResult.data as BookingTrackingResult).checkOutDate).toLocaleDateString()}</p>
                        <p><span className="font-medium">Nights:</span> {(trackingResult.data as BookingTrackingResult).numberOfNights}</p>
                        <p><span className="font-medium">Guests:</span> {(trackingResult.data as BookingTrackingResult).numberOfGuests}</p>
                      </div>
                    </div>
                  </div>

                  {/* Guest Info */}
                  <div className="pt-4 border-t">
                    <h4 className="font-semibold text-gray-900 mb-3">Guest Information</h4>
                    <div className="text-sm text-gray-600 space-y-1">
                      <p className="font-medium">{(trackingResult.data as BookingTrackingResult).guestName}</p>
                      <p>{(trackingResult.data as BookingTrackingResult).guestEmail}</p>
                      <p>{(trackingResult.data as BookingTrackingResult).guestPhoneNo}</p>
                    </div>
                  </div>

                  {/* Payment Info */}
                  <div className="pt-4 border-t">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-600">Total Amount</p>
                        <p className="text-2xl font-bold text-primary">₦{(trackingResult.data as BookingTrackingResult).totalAmount?.toLocaleString()}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-sm text-gray-600">Payment Status</p>
                        <Badge className={(trackingResult.data as BookingTrackingResult).paymentStatus ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'}>
                          {(trackingResult.data as BookingTrackingResult).paymentStatus ? 'Paid' : 'Pending'}
                        </Badge>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </section>

      {/* Accommodations Section */}
      <section className="py-16 bg-white">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-2">Accommodations</h2>
            <p className="text-gray-600">Some of the accommodation bookings close to you</p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {accommodations.map((accommodation) => (
              <Card key={accommodation.id} className="overflow-hidden group cursor-pointer hover:shadow-lg transition-shadow">
                <div className="relative h-48 overflow-hidden">
                  <img 
                    src={accommodation.image || "/service-1.jpg"} 
                    alt={accommodation.accomodationName} 
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                  />
                  <div className="absolute top-4 right-4 bg-white/90 px-3 py-1 rounded-full text-sm font-medium">
                    {accommodation.accomodationType}
                  </div>
                </div>
                <CardContent className="p-6">
                  <div className="flex items-center gap-2 mb-2">
                    <span className="text-primary font-semibold">₦{accommodation.bookingAmount.toLocaleString()}</span>
                    <span className="text-gray-400">|</span>
                    <span className="text-yellow-500">★ {accommodation.rating}</span>
                  </div>
                  <h3 className="text-xl font-semibold mb-2 group-hover:text-primary transition-colors">
                    {accommodation.accomodationName}
                  </h3>
                  <p className="text-gray-600 text-sm mb-2">{accommodation.accomodationDescription}</p>
                  <p className="text-gray-500 text-sm">{accommodation.location}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Available Room Bookings Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-2">Available Room Bookings</h2>
            <p className="text-gray-600">We all live in an age that belongs to the young at heart. Life that is becoming extremely fast.</p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {roomBookings.map((room) => (
              <Card key={room.id} className="overflow-hidden group">
                <div className="relative h-48 overflow-hidden">
                  <img 
                    src={room.accomodationImage} 
                    alt={room.accomodationName} 
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                  />
                  <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                    <Link href={`/customer-portal/register`}>
                      <Button className="opacity-0 group-hover:opacity-100 transition-opacity">
                        Book Now
                      </Button>
                    </Link>
                  </div>
                </div>
                <CardContent className="p-4 text-center">
                  <h4 className="font-semibold mb-1">{room.accomodationName}</h4>
                  <p className="text-sm text-gray-500 mb-2">(Type: {room.roomType})</p>
                  <h5 className="text-primary font-bold">
                    ₦{room.roomPrice.toLocaleString()}<small className="text-gray-500 font-normal">/night</small>
                  </h5>
                  <p className="text-xs text-gray-500 mt-1">Room: {room.roomNumber}</p>
                  <p className="text-xs text-gray-400">{room.accomodationLocality}, {room.accomodationState}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Logistics Listings Section */}
      <section className="py-16 bg-white">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold text-gray-900 mb-2">Logistics Listings</h2>
            <p className="text-gray-600">Send your packages with trusted logistics partners</p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {logisticsCompanies.map((company) => (
              <Card key={company.id} className="overflow-hidden group">
                <div className="relative h-40 overflow-hidden">
                  <img 
                    src={company.companyLogo} 
                    alt={company.companyName} 
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                  />
                  <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                    <Link href={`/customer-portal/register`}>
                      <Button className="opacity-0 group-hover:opacity-100 transition-opacity">
                        Send
                      </Button>
                    </Link>
                  </div>
                </div>
                <CardContent className="p-4 text-center">
                  <h4 className="font-semibold">{company.companyName}</h4>
                  <p className="text-xs text-gray-500 mt-1">{company.companyInfo}</p>
                  <p className="text-sm text-primary font-medium mt-2">From ₦{company.valueCharge.toLocaleString()}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-3 gap-8">
            <div className="text-center p-6">
              <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                <Package className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Track Orders</h3>
              <p className="text-gray-600">Monitor your logistics shipments in real-time</p>
            </div>
            <div className="text-center p-6">
              <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                <Home className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Book Accommodations</h3>
              <p className="text-gray-600">Find and reserve hotels and apartments</p>
            </div>
            <div className="text-center p-6">
              <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                <Search className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-xl font-semibold mb-2">24/7 Support</h3>
              <p className="text-gray-600">Get help whenever you need it</p>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-gray-900 text-white py-12">
        <div className="container mx-auto px-4">
          <div className="grid md:grid-cols-3 gap-8 mb-8">
            <div>
              <h3 className="text-lg font-semibold mb-4">About Ginilog</h3>
              <p className="text-gray-400">
                Your one-stop platform for accommodation bookings and reliable logistics solutions.
              </p>
            </div>
            <div>
              <h3 className="text-lg font-semibold mb-4">Quick Links</h3>
              <ul className="space-y-2">
                <li><Link href="/customer-portal" className="text-gray-400 hover:text-white">Home</Link></li>
                <li><Link href="/customer-portal/login" className="text-gray-400 hover:text-white">Sign In</Link></li>
                <li><Link href="/customer-portal/register" className="text-gray-400 hover:text-white">Get Started</Link></li>
              </ul>
            </div>
            <div>
              <h3 className="text-lg font-semibold mb-4">Contact</h3>
              <p className="text-gray-400">
                Get in touch with us for support and inquiries.
              </p>
            </div>
          </div>
          <div className="border-t border-gray-800 pt-8 text-center text-gray-400">
            <p>&copy; {new Date().getFullYear()} GINILOG. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
