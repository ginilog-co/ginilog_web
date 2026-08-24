// app/brand-owner/register/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, Mail, Lock, Eye, EyeOff, Building2, Phone, User, AlertCircle, CheckCircle } from "lucide-react";
import { registerBrandOwner } from "@/lib/api";

export default function BrandOwnerRegisterPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [selectedTypes, setSelectedTypes] = useState<string[]>([]);
  const [formData, setFormData] = useState({
    firstName: "",
    surName: "",
    email: "",
    password: "",
    phoneNo: "",
    address: "",
    companyName: "",
    companyUserName: "",
    companyType: [] as string[],
    staffType: "BrandOwner",
    roles: ["BrandOwner"],
    state: "",
    branch: "",
    locality: "",
    sex: "Male",
    staffCode: "",
    permissions: [
      "CanCreateStaff", "CanManageStaff", "CanViewStaff", "CanDeleteStaff",
      "CanViewBrands", "CanManageBrands", "CanDeleteBrands",
      "CanCreateProduct", "CanViewProduct", "CanManageProduct", "CanDeleteProduct",
      "CanViewOrders", "CanManageOrders", "CanDeleteOrders",
      "CanViewBookings", "CanManageBookings", "CanDeleteBookings",
      "CanViewWallet", "CanManageWallet",
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
      const requiredFields = ['firstName', 'surName', 'email', 'password', 'phoneNo', 'companyName'];
      const missing = requiredFields.filter(field => !formData[field as keyof typeof formData]);
      if (missing.length > 0) {
        setError(`Missing: ${missing.join(', ')}`);
        setIsLoading(false);
        return;
      }

      if (selectedTypes.length === 0) {
        setError("Please select at least one company type");
        setIsLoading(false);
        return;
      }

      const staffCode = formData.staffCode || `${formData.firstName.substring(0, 3)}${Math.random().toString(36).substring(2, 6).toUpperCase()}`;

      await registerBrandOwner({
        ...formData,
        staffType: "BrandOwner",
        roles: ["BrandOwner"],
        permissions: formData.permissions,
        staffCode,
        companyType: selectedTypes,
      } as any);

      setSuccess("Registration successful! Redirecting to login...");
      setTimeout(() => router.push("/brand-owner/login"), 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 px-4 py-8">
      <div className="w-full max-w-2xl">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 mb-4">
            <Building2 className="h-8 w-8 text-primary" />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">GINILOG</h1>
          <p className="text-gray-500 text-sm mt-1">Register as Brand Owner</p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl border border-gray-100 p-8">
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm flex items-start gap-2">
              <AlertCircle className="h-4 w-4 flex-shrink-0 mt-0.5" />
              <span>{error}</span>
            </div>
          )}
          {success && (
            <div className="mb-4 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl text-sm flex items-start gap-2">
              <CheckCircle className="h-4 w-4 flex-shrink-0 mt-0.5" />
              <span>{success}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="firstName">First Name *</Label>
                <Input
                  id="firstName"
                  value={formData.firstName}
                  onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                  required
                  placeholder="Emeka"
                />
              </div>
              <div>
                <Label htmlFor="surName">Last Name *</Label>
                <Input
                  id="surName"
                  value={formData.surName}
                  onChange={(e) => setFormData({ ...formData, surName: e.target.value })}
                  required
                  placeholder="Ugwu"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="email">Email *</Label>
                <Input
                  id="email"
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  required
                  placeholder="brand@ginilog.com"
                />
              </div>
              <div>
                <Label htmlFor="phoneNo">Phone Number *</Label>
                <Input
                  id="phoneNo"
                  type="tel"
                  value={formData.phoneNo}
                  onChange={(e) => setFormData({ ...formData, phoneNo: e.target.value })}
                  required
                  placeholder="+2348012345678"
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
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    required
                    minLength={8}
                    placeholder="••••••••"
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
                <Label htmlFor="sex">Gender</Label>
                <select
                  id="sex"
                  className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                  value={formData.sex}
                  onChange={(e) => setFormData({ ...formData, sex: e.target.value })}
                >
                  {sexOptions.map((option) => (
                    <option key={option} value={option}>{option}</option>
                  ))}
                </select>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="companyName">Company Name *</Label>
                <Input
                  id="companyName"
                  value={formData.companyName}
                  onChange={(e) => setFormData({ ...formData, companyName: e.target.value })}
                  required
                  placeholder="Jopachi Group"
                />
              </div>
              <div>
                <Label htmlFor="companyUserName">Company Username *</Label>
                <Input
                  id="companyUserName"
                  value={formData.companyUserName}
                  onChange={(e) => setFormData({ ...formData, companyUserName: e.target.value })}
                  required
                  placeholder="Jopachi"
                />
              </div>
            </div>

            <div>
              <Label htmlFor="address">Business Address</Label>
              <Input
                id="address"
                value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                placeholder="New Haven Market Road"
              />
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

            <div>
              <Label htmlFor="staffCode">Staff Code (Optional)</Label>
              <Input
                id="staffCode"
                value={formData.staffCode}
                onChange={(e) => setFormData({ ...formData, staffCode: e.target.value })}
                placeholder="Leave empty to auto-generate"
              />
            </div>

            <Button type="submit" disabled={isLoading || selectedTypes.length === 0} className="w-full h-11 rounded-xl font-semibold">
              {isLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              {isLoading ? "Registering..." : "Register"}
            </Button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-sm text-gray-500">
              Already have an account?{" "}
              <Link href="/brand-owner/login" className="text-primary hover:underline font-medium">
                Sign in
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}