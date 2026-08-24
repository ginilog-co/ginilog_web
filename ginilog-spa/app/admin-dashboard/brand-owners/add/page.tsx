"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Eye, EyeOff, Building2, Check } from "lucide-react";
import { registerBrandOwner } from "@/lib/api";

export default function RegisterBrandOwnerPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [selectedTypes, setSelectedTypes] = useState<string[]>([]);
  const [formData, setFormData] = useState({
    staffType: "BrandOwner",
    firstName: "",
    surName: "",
    email: "",
    password: "",
    sex: "Male",
    staffCode: "",
    phoneNo: "",
    address: "",
    state: "Lagos",
    branch: "Main",
    locality: "Ikeja",
    companyName: "",
    companyUserName: "",
    companyType: [] as string[],
    roles: ["BrandOwner"],
    permissions: [
      // Staff Permissions
      "CanCreateStaff",
      "CanManageStaff",
      "CanViewStaff",
      "CanDeleteStaff",
      // Brand Permissions
      "CanViewBrands",
      "CanManageBrands",
      "CanDeleteBrands",
      // Product Permissions
      "CanCreateProduct",
      "CanViewProduct",
      "CanManageProduct",
      "CanDeleteProduct",
      // Wallet Permissions
      "CanViewWallet",
      "CanManageWallet",
      // Order Management Permissions
      "CanViewOrders",
      "CanManageOrders",
      "CanDeleteOrders",
      // Bookings Permissions
      "CanViewBookings",
      "CanManageBookings",
      "CanDeleteBookings",
    ],
  });

  const companyTypes = ["Accommodations", "Logistics", "Airlines", "Restaurants", "Retail"];
  const sexOptions = ["Male", "Female", "Other"];

  const toggleCompanyType = (type: string) => {
    const updated = selectedTypes.includes(type)
      ? selectedTypes.filter(t => t !== type)
      : [...selectedTypes, type];
    setSelectedTypes(updated);
    setFormData({ ...formData, companyType: updated });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setIsLoading(true);

    try {
      // Validate required fields
      if (!formData.firstName || !formData.surName || !formData.email || 
          !formData.password || !formData.phoneNo || !formData.companyName) {
        setError("Please fill in all required fields");
        setIsLoading(false);
        return;
      }

      if (selectedTypes.length === 0) {
        setError("Please select at least one company type");
        setIsLoading(false);
        return;
      }

      // Generate staff code if not provided
      const staffCode = formData.staffCode || `${formData.firstName.substring(0, 3)}${Math.random().toString(36).substring(2, 6).toUpperCase()}`;
      
      const payload = {
        ...formData,
        state: formData.state || "Lagos",
        branch: formData.branch || "Main",
        locality: formData.locality || "Ikeja",
        staffCode: staffCode,
        companyType: selectedTypes,
      };

      await registerBrandOwner({
        ...payload,
        staffType: "BrandOwner" as const,
        state: payload.state || "Lagos",
        branch: payload.branch || "Main",
        locality: payload.locality || "Ikeja",
      } as any);
      setSuccess("Brand owner registered successfully!");
      setTimeout(() => router.push("/admin-dashboard/brand-owners"), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/brand-owners">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Register Brand Owner</h1>
          <p className="text-sm text-gray-500">Create a new brand owner account with full permissions</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Brand Owner Details</CardTitle>
        </CardHeader>
        <CardContent>
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
              {error}
            </div>
          )}
          {success && (
            <div className="mb-4 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl text-sm flex items-center gap-2">
              <Check className="h-4 w-4" />
              {success}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="firstName">First Name *</Label>
                <Input
                  id="firstName"
                  placeholder="Emeka"
                  value={formData.firstName}
                  onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                  required
                />
              </div>
              <div>
                <Label htmlFor="surName">Last Name *</Label>
                <Input
                  id="surName"
                  placeholder="Ugwu"
                  value={formData.surName}
                  onChange={(e) => setFormData({ ...formData, surName: e.target.value })}
                  required
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="email">Email *</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="johnpaulchigozie80@gmail.com"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  required
                />
              </div>
              <div>
                <Label htmlFor="phoneNo">Phone Number *</Label>
                <Input
                  id="phoneNo"
                  type="tel"
                  placeholder="+2349085678234"
                  value={formData.phoneNo}
                  onChange={(e) => setFormData({ ...formData, phoneNo: e.target.value })}
                  required
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="password">Password *</Label>
                <div className="relative">
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    placeholder="Password1234$"
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    required
                    minLength={8}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  >
                    {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                  </button>
                </div>
              </div>
              <div>
                <Label htmlFor="sex">Gender *</Label>
                <select
                  id="sex"
                  className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                  value={formData.sex}
                  onChange={(e) => setFormData({ ...formData, sex: e.target.value })}
                  required
                >
                  {sexOptions.map((option) => (
                    <option key={option} value={option}>{option}</option>
                  ))}
                </select>
              </div>
            </div>

            <div>
              <Label htmlFor="staffCode">Staff Code</Label>
              <Input
                id="staffCode"
                placeholder="Jopachi001 (Auto-generated if left empty)"
                value={formData.staffCode}
                onChange={(e) => setFormData({ ...formData, staffCode: e.target.value })}
              />
              <p className="text-xs text-gray-500 mt-1">Leave empty to auto-generate</p>
            </div>

            <div>
              <Label htmlFor="address">Business Address *</Label>
              <Input
                id="address"
                placeholder="New Haven Market Road"
                value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="companyName">Company Name *</Label>
                <Input
                  id="companyName"
                  placeholder="Jopachi Group"
                  value={formData.companyName}
                  onChange={(e) => setFormData({ ...formData, companyName: e.target.value })}
                  required
                />
              </div>
              <div>
                <Label htmlFor="companyUserName">Company Username *</Label>
                <Input
                  id="companyUserName"
                  placeholder="Jopachi"
                  value={formData.companyUserName}
                  onChange={(e) => setFormData({ ...formData, companyUserName: e.target.value })}
                  required
                />
              </div>
            </div>

            <div>
              <Label>Company Type *</Label>
              <div className="flex flex-wrap gap-2 mt-2">
                {companyTypes.map((type) => (
                  <button
                    key={type}
                    type="button"
                    className={`px-4 py-2 rounded-xl text-sm font-medium border transition-all ${
                      selectedTypes.includes(type)
                        ? "bg-primary text-white border-primary shadow-sm"
                        : "bg-gray-100 text-gray-700 border-gray-200 hover:bg-gray-200"
                    }`}
                    onClick={() => toggleCompanyType(type)}
                  >
                    {type}
                  </button>
                ))}
              </div>
              {selectedTypes.length === 0 && (
                <p className="text-sm text-red-500 mt-2">Select at least one company type</p>
              )}
            </div>

            {/* Permissions Summary */}
            <div className="p-4 bg-gray-50 rounded-xl">
              <p className="text-sm font-medium text-gray-700 mb-2">Permissions Granted</p>
              <div className="flex flex-wrap gap-1">
                {formData.permissions.map((perm) => (
                  <span key={perm} className="px-2 py-0.5 bg-blue-100 text-blue-700 rounded-full text-xs">
                    {perm}
                  </span>
                ))}
              </div>
              <p className="text-xs text-gray-500 mt-2">Full brand owner permissions included</p>
            </div>

            <div className="flex gap-4 pt-4">
              <Button 
                type="submit" 
                disabled={isLoading || selectedTypes.length === 0} 
                className="flex-1"
              >
                {isLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                {isLoading ? "Registering..." : "Register Brand Owner"}
              </Button>
              <Link href="/admin-dashboard/brand-owners" className="flex-1">
                <Button type="button" variant="outline" className="w-full">Cancel</Button>
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}