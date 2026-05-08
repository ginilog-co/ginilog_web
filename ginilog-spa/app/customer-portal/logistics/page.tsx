"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { 
  getCompanies, 
  Company, 
  getStoredUser, 
  getProfile,
  UserProfile 
} from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { 
  Truck, 
  MapPin, 
  Loader2, 
  ChevronRight,
  Package,
  LogOut,
  Bell,
  User,
  Info
} from "lucide-react";

export default function LogisticsPage() {
  const router = useRouter();
  const [companies, setCompanies] = useState<Company[]>([]);
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
        const [profile, comps] = await Promise.all([
          getProfile(),
          getCompanies()
        ]);
        setUser(profile);
        setCompanies(comps || []);
      } catch (error) {
        console.error("Failed to fetch logistics data:", error);
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
        <div className="mb-8 text-center md:text-left">
          <h1 className="text-3xl font-bold text-gray-900">Logistics Partners</h1>
          <p className="text-gray-600 mt-1">Send your packages nationwide with our trusted logistics providers.</p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {companies.map((comp) => (
            <Card key={comp.id} className="overflow-hidden group hover:shadow-lg transition-all border-none shadow-sm">
              <div className="relative h-40 bg-gray-100 flex items-center justify-center overflow-hidden">
                {comp.companyLogo ? (
                  <img 
                    src={comp.companyLogo} 
                    alt={comp.companyName}
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                  />
                ) : (
                  <Truck className="h-12 w-12 text-gray-300" />
                )}
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-0 group-hover:opacity-100 transition-opacity flex items-end p-4">
                  <p className="text-white text-xs font-medium">Starting from ₦{comp.valueCharge?.toLocaleString()}</p>
                </div>
              </div>
              <CardContent className="p-5 text-center">
                <h3 className="text-lg font-bold text-gray-900 mb-2 group-hover:text-primary transition-colors">
                  {comp.companyName}
                </h3>
                <div className="flex items-center justify-center gap-2 text-gray-500 text-xs mb-4">
                  <Info className="h-3 w-3" />
                  <span className="line-clamp-1">{comp.companyInfo || "Professional delivery services"}</span>
                </div>
                <Link href={`/customer-portal/logistics/${comp.id}`}>
                  <Button className="w-full bg-primary hover:bg-primary/90">
                    Send Package
                    <ChevronRight className="h-4 w-4 ml-2" />
                  </Button>
                </Link>
              </CardContent>
            </Card>
          ))}

          {companies.length === 0 && (
            <div className="col-span-full text-center py-20 bg-white rounded-xl border-2 border-dashed">
              <Package className="h-12 w-12 text-gray-300 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-gray-900">No logistics partners found</h3>
              <p className="text-gray-500">We are expanding our network. Check back soon!</p>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
