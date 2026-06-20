"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { 
  getAccommodations, 
  Accommodation, 
  getStoredUser, 
  getProfile,
  UserProfile,
  logout,
  clearAuthData
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
  User,
  Menu,
  X,
  LayoutDashboard,
  ShoppingBag,
  UserCircle,
  ChevronRight as ChevronRightIcon,
  Sparkles,
  LogOut as LogOutIcon
} from "lucide-react";

export default function AccommodationsPage() {
  const router = useRouter();
  const [accommodations, setAccommodations] = useState<Accommodation[]>([]);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

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
              <Sparkles className="h-6 w-6" />
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
                      <ChevronRightIcon className="h-4 w-4 text-red-400 group-hover:text-red-500 transition-colors" />
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
                      <ChevronRightIcon className="h-4 w-4 text-gray-400 group-hover:text-primary transition-colors" />
                    </Link>
                  )
                ))}
              </nav>
            </div>
          </div>
        )}
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