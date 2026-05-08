"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { 
  getAccommodations, 
  Accommodation, 
  getStoredUser, 
  getProfile,
  UserProfile 
} from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Building2, 
  MapPin, 
  Star, 
  Loader2, 
  ChevronRight,
  Home,
  LogOut,
  Bell,
  User
} from "lucide-react";

export default function AccommodationsPage() {
  const router = useRouter();
  const [accommodations, setAccommodations] = useState<Accommodation[]>([]);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const storedUser = getStoredUser();
        if (!storedUser) {
          router.push("/customer-portal/login");
          return;
        }
        const [profile, accoms] = await Promise.all([
          getProfile(),
          getAccommodations()
        ]);
        setUser(profile);
        setAccommodations(accoms || []);
      } catch (error) {
        console.error("Failed to fetch accommodations:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [router]);

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

            <nav className="hidden md:flex items-center space-x-8">
              <Link href="/customer-portal/dashboard" className="text-gray-600 hover:text-gray-900">
                Dashboard
              </Link>
              <Link href="/customer-portal/orders" className="text-gray-600 hover:text-gray-900">
                My Orders & Bookings
              </Link>
              <Link href="/customer-portal/profile" className="text-gray-600 hover:text-gray-900">
                Profile
              </Link>
            </nav>

            <div className="flex items-center gap-4">
              <button className="relative p-2 text-gray-600 hover:text-gray-900">
                <Bell className="h-6 w-6" />
                <span className="absolute top-1 right-1 h-2 w-2 bg-primary rounded-full" />
              </button>
              
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden">
                  {user?.profilePicture ? (
                    <img src={user.profilePicture} alt={user.firstName} className="h-full w-full object-cover" />
                  ) : (
                    <User className="h-5 w-5 text-primary" />
                  )}
                </div>
              </div>

              <Link href="/customer-portal/login">
                <Button variant="ghost" size="icon" className="text-gray-600">
                  <LogOut className="h-5 w-5" />
                </Button>
              </Link>
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Accommodations</h1>
          <p className="text-gray-600 mt-1">Discover and book the best stays nationwide.</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {accommodations.map((acc) => (
            <Card key={acc.id} className="overflow-hidden group hover:shadow-lg transition-shadow">
              <div className="relative h-48">
                <img 
                  src={acc.accomodationImages?.[0] || "/service-1.jpg"} 
                  alt={acc.accomodationName}
                  className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                />
                <div className="absolute top-4 right-4 bg-white/90 px-3 py-1 rounded-full text-sm font-medium">
                  {acc.accomodationType}
                </div>
              </div>
              <CardContent className="p-6">
                <div className="flex items-center justify-between mb-2">
                  <div className="flex items-center gap-1 text-yellow-500">
                    <Star className="h-4 w-4 fill-current" />
                    <span className="text-sm font-medium">{acc.rating || "4.5"}</span>
                  </div>
                  <span className="text-primary font-bold">₦{acc.bookingAmount?.toLocaleString()}</span>
                </div>
                <h3 className="text-xl font-bold text-gray-900 mb-2">{acc.accomodationName}</h3>
                <div className="flex items-center gap-2 text-gray-500 text-sm mb-4">
                  <MapPin className="h-4 w-4" />
                  {acc.location}
                </div>
                <p className="text-gray-600 text-sm mb-6 line-clamp-2">
                  {acc.accomodationDescription || "Experience comfort and luxury in our well-appointed rooms."}
                </p>
                <Link href={`/customer-portal/accommodations/${acc.id}`}>
                  <Button className="w-full group">
                    Book Now
                    <ChevronRight className="h-4 w-4 ml-2 group-hover:translate-x-1 transition-transform" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
          ))}

          {accommodations.length === 0 && (
            <div className="col-span-full text-center py-20 bg-white rounded-xl border-2 border-dashed">
              <Building2 className="h-12 w-12 text-gray-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-gray-900">No accommodations found</h3>
              <p className="text-gray-500">Check back later for new listings.</p>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
