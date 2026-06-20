"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";
import { 
  createOrder, 
  getStoredUser, 
  getProfile,
  UserProfile,
  AddOrder,
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
  Package, 
  Truck, 
  MapPin, 
  User, 
  ArrowLeft,
  CheckCircle2,
  AlertCircle,
  Menu,
  X,
  LayoutDashboard,
  ShoppingBag,
  UserCircle,
  ChevronRight,
  Sparkles,
  LogOut as LogOutIcon,
  LogOut
} from "lucide-react";

export default function DeliveryRequestPage() {
  const router = useRouter();
  const params = useParams();
  const companyId = params.id as string;

  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const [formData, setFormData] = useState({
    itemName: "",
    itemDescription: "",
    itemCost: 0,
    itemQuantity: 1,
    itemWeight: 0,
    packageType: "Standard",
    recieverName: "",
    recieverPhoneNo: "",
    recieverEmail: "",
    recieverAddress: "",
    recieverState: "",
    recieverLocality: "",
    riderType: "Bike",
    shippingType: "Express"
  });

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const storedUser = getStoredUser();
        if (!storedUser) {
          router.push("/customer-portal/login");
          return;
        }
        const profile = await getProfile();
        setUser(profile);
      } catch (err) {
        console.error("Failed to fetch user data:", err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchUserData();
  }, [router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;

    setIsSubmitting(true);
    setError(null);

    try {
      const orderData: AddOrder = {
        ...formData,
        senderName: `${user.firstName} ${user.lastName}`,
        senderEmail: user.email,
        senderPhoneNo: user.phoneNo,
        senderAddress: user.address || "N/A",
        senderState: user.state || "N/A",
        senderLocality: user.locality || "N/A",
        userType: "Registered"
      };

      await createOrder(companyId, orderData);
      setOrderSuccess(true);
      setTimeout(() => router.push("/customer-portal/orders"), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create delivery request. Please try again.");
    } finally {
      setIsSubmitting(false);
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
              <Link href="/customer-portal/logistics">
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
                  href="/customer-portal/logistics"
                  onClick={() => setIsMobileMenuOpen(false)}
                  className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition-colors group"
                >
                  <div className="flex items-center gap-3">
                    <div className="h-8 w-8 rounded-lg bg-gray-100 group-hover:bg-primary/10 flex items-center justify-center transition-colors">
                      <ArrowLeft className="h-4 w-4 text-gray-500 group-hover:text-primary transition-colors" />
                    </div>
                    <span className="text-gray-700 group-hover:text-primary font-medium transition-colors">
                      Back to Logistics
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
        <div className="max-w-4xl mx-auto">
          {orderSuccess ? (
            <Card className="text-center p-12">
              <div className="h-20 w-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
                <CheckCircle2 className="h-10 w-10 text-green-600" />
              </div>
              <h2 className="text-3xl font-bold text-gray-900 mb-2">Order Created!</h2>
              <p className="text-gray-600">Your delivery request has been placed successfully. Redirecting to your orders...</p>
            </Card>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-8">
              <div className="flex flex-col gap-2">
                <h1 className="text-3xl font-bold text-gray-900">Delivery Request</h1>
                <p className="text-gray-600">Fill in the details below to send your package.</p>
              </div>

              {error && (
                <div className="p-4 bg-red-50 border border-red-200 rounded-md text-red-600 flex items-center gap-2">
                  <AlertCircle className="h-5 w-5" />
                  {error}
                </div>
              )}

              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                {/* Item Details */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Package className="h-5 w-5 text-primary" />
                      Item Details
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="space-y-2">
                      <Label>Item Name</Label>
                      <Input 
                        required 
                        value={formData.itemName}
                        onChange={(e) => setFormData({ ...formData, itemName: e.target.value })}
                        placeholder="e.g. Laptop, Clothes"
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Description</Label>
                      <textarea 
                        className="w-full p-2 border rounded-md text-sm min-h-[80px]"
                        value={formData.itemDescription}
                        onChange={(e) => setFormData({ ...formData, itemDescription: e.target.value })}
                        placeholder="Provide details about the item..."
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label>Quantity</Label>
                        <Input 
                          type="number" 
                          min="1" 
                          required 
                          value={formData.itemQuantity}
                          onChange={(e) => setFormData({ ...formData, itemQuantity: parseInt(e.target.value) })}
                        />
                      </div>
                      <div className="space-y-2">
                        <Label>Weight (kg)</Label>
                        <Input 
                          type="number" 
                          step="0.1" 
                          required 
                          value={formData.itemWeight}
                          onChange={(e) => setFormData({ ...formData, itemWeight: parseFloat(e.target.value) })}
                        />
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Receiver Details */}
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <User className="h-5 w-5 text-primary" />
                      Receiver Details
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="space-y-2">
                      <Label>Full Name</Label>
                      <Input 
                        required 
                        value={formData.recieverName}
                        onChange={(e) => setFormData({ ...formData, recieverName: e.target.value })}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Phone Number</Label>
                      <Input 
                        required 
                        value={formData.recieverPhoneNo}
                        onChange={(e) => setFormData({ ...formData, recieverPhoneNo: e.target.value })}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Email</Label>
                      <Input 
                        type="email" 
                        required 
                        value={formData.recieverEmail}
                        onChange={(e) => setFormData({ ...formData, recieverEmail: e.target.value })}
                      />
                    </div>
                  </CardContent>
                </Card>

                {/* Delivery Address */}
                <Card className="md:col-span-2">
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <MapPin className="h-5 w-5 text-primary" />
                      Delivery Address
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div className="md:col-span-2 space-y-2">
                      <Label>Street Address</Label>
                      <Input 
                        required 
                        value={formData.recieverAddress}
                        onChange={(e) => setFormData({ ...formData, recieverAddress: e.target.value })}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>State</Label>
                      <Input 
                        required 
                        value={formData.recieverState}
                        onChange={(e) => setFormData({ ...formData, recieverState: e.target.value })}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Locality/City</Label>
                      <Input 
                        required 
                        value={formData.recieverLocality}
                        onChange={(e) => setFormData({ ...formData, recieverLocality: e.target.value })}
                      />
                    </div>
                    <div className="space-y-2">
                      <Label>Vehicle Type</Label>
                      <select 
                        className="w-full h-10 px-3 border rounded-md text-sm"
                        value={formData.riderType}
                        onChange={(e) => setFormData({ ...formData, riderType: e.target.value })}
                      >
                        <option value="Bike">Bike</option>
                        <option value="Car">Car</option>
                        <option value="Van">Van</option>
                        <option value="Truck">Truck</option>
                      </select>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <div className="flex justify-end gap-4">
                <Link href="/customer-portal/logistics">
                  <Button variant="outline" type="button">Cancel</Button>
                </Link>
                <Button type="submit" disabled={isSubmitting} className="px-8">
                  {isSubmitting ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Processing...
                    </>
                  ) : (
                    "Place Delivery Request"
                  )}
                </Button>
              </div>
            </form>
          )}
        </div>
      </main>
    </div>
  );
}