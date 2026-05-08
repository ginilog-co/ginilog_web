"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";
import { 
  createOrder, 
  getStoredUser, 
  getProfile,
  UserProfile,
  AddOrder
} from "@/lib/api";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { 
  Loader2, 
  Package, 
  Truck, 
  MapPin, 
  User, 
  ArrowLeft,
  CheckCircle2,
  AlertCircle
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
            <Link href="/customer-portal/logistics">
              <Button variant="ghost" size="sm">
                <ArrowLeft className="h-4 w-4 mr-2" /> Back
              </Button>
            </Link>
          </div>
        </div>
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
