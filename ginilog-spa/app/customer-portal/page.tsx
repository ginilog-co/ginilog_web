import Link from "next/link";
import Image from "next/image";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Search, Package, Home, Truck } from "lucide-react";

// Mock data based on traditional web app models
const accommodations = [
  {
    id: "1",
    accomodationName: "Grand Hotel",
    accomodationType: "Hotel",
    location: "Lagos, Nigeria",
    bookingAmount: 45000,
    accomodationImages: ["/images/hotel1.jpg"],
    accomodationDescription: "Luxury hotel with modern amenities",
    rating: 4.5,
  },
  {
    id: "2",
    accomodationName: "Sunset Apartments",
    accomodationType: "Apartment",
    location: "Abuja, Nigeria",
    bookingAmount: 35000,
    accomodationImages: ["/images/hotel2.jpg"],
    accomodationDescription: "Spacious apartments with city views",
    rating: 4.2,
  },
  {
    id: "3",
    accomodationName: "Beach Resort",
    accomodationType: "Resort",
    location: "Calabar, Nigeria",
    bookingAmount: 55000,
    accomodationImages: ["/images/hotel3.jpg"],
    accomodationDescription: "Beachfront resort with pool access",
    rating: 4.8,
  },
];

const roomBookings = [
  {
    id: "1",
    accomodationName: "Grand Hotel",
    roomType: "Deluxe Suite",
    roomNumber: 101,
    roomPrice: 25000,
    accomodationImage: "/images/room1.jpg",
    accomodationLocality: "Victoria Island",
    accomodationState: "Lagos",
  },
  {
    id: "2",
    accomodationName: "Sunset Apartments",
    roomType: "Standard Room",
    roomNumber: 205,
    roomPrice: 18000,
    accomodationImage: "/images/room2.jpg",
    accomodationLocality: "Maitama",
    accomodationState: "Abuja",
  },
  {
    id: "3",
    accomodationName: "Beach Resort",
    roomType: "Ocean View",
    roomNumber: 301,
    roomPrice: 35000,
    accomodationImage: "/images/room3.jpg",
    accomodationLocality: "Marina",
    accomodationState: "Calabar",
  },
  {
    id: "4",
    accomodationName: "City Lodge",
    roomType: "Executive Room",
    roomNumber: 405,
    roomPrice: 22000,
    accomodationImage: "/images/room4.jpg",
    accomodationLocality: "Ikeja",
    accomodationState: "Lagos",
  },
];

const logisticsCompanies = [
  {
    id: "1",
    companyName: "Swift Logistics",
    companyLogo: "/images/logistics1.jpg",
    companyInfo: "Fast and reliable delivery services",
    valueCharge: 1500,
  },
  {
    id: "2",
    companyName: "Express Delivery",
    companyLogo: "/images/logistics2.jpg",
    companyInfo: "Nationwide coverage with tracking",
    valueCharge: 2000,
  },
  {
    id: "3",
    companyName: "Prime Couriers",
    companyLogo: "/images/logistics3.jpg",
    companyInfo: "Same-day delivery specialists",
    valueCharge: 3000,
  },
  {
    id: "4",
    companyName: "Trusty Transport",
    companyLogo: "/images/logistics4.jpg",
    companyInfo: "Secure handling for fragile items",
    valueCharge: 1800,
  },
];

export default function CustomerPortalHome() {
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
            <Link href="/customer-portal/app/accommodations" className="text-gray-600 hover:text-gray-900">
              Accommodations
            </Link>
            <Link href="/customer-portal/app/logistics" className="text-gray-600 hover:text-gray-900">
              Send A Parcel
            </Link>
          </nav>
          <div className="flex items-center gap-4">
            <Link href="/customer-portal/app/login">
              <Button variant="outline">Sign In</Button>
            </Link>
            <Link href="/customer-portal/app/register">
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
          <div className="max-w-2xl mx-auto bg-white rounded-lg shadow-lg p-2">
            <form className="flex items-center gap-2" action="/customer-portal/app/logistics" method="get">
              <div className="flex-1 relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                <Input
                  type="text"
                  name="id"
                  placeholder="Enter Tracking Number"
                  className="pl-10 h-14 text-lg border-0 focus-visible:ring-0"
                  required
                />
              </div>
              <Button type="submit" size="lg" className="h-14 px-8">
                Track Package
              </Button>
            </form>
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
                <div className="relative h-48 bg-gray-200">
                  <div className="absolute inset-0 flex items-center justify-center text-gray-400">
                    <Home className="h-12 w-12" />
                  </div>
                  {/* Placeholder for actual images */}
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
                <div className="relative h-48 bg-gray-200">
                  <div className="absolute inset-0 flex items-center justify-center text-gray-400">
                    <Home className="h-10 w-10" />
                  </div>
                  <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                    <Link href={`/customer-portal/app/accommodations`}>
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
                <div className="relative h-40 bg-gray-100">
                  <div className="absolute inset-0 flex items-center justify-center text-gray-400">
                    <Truck className="h-12 w-12" />
                  </div>
                  <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                    <Link href={`/customer-portal/app/logistics`}>
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
                <li><Link href="/customer-portal/app/accommodations" className="text-gray-400 hover:text-white">Accommodations</Link></li>
                <li><Link href="/customer-portal/app/logistics" className="text-gray-400 hover:text-white">Logistics</Link></li>
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
