"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  ArrowLeft,
  Loader2,
  AlertCircle,
  CheckCircle,
  Plus,
  User,
  Mail,
  Phone,
  MapPin,
  Building2,
  Key,
  Users,
} from "lucide-react";
import { 
  getStoredUser, 
  addStaff, 
  isAuthenticated, 
  validateSession 
} from "@/lib/api";

// Staff types
const STAFF_TYPES = [
  { value: "Manager", label: "Manager" },
  { value: "Staff", label: "Staff" },
  { value: "Admin", label: "Admin" },
  { value: "Rider", label: "Rider" },
  { value: "Driver", label: "Driver" },
];

// Company types
const COMPANY_TYPES = [
  "Logistics",
  "Accommodation",
  "Retail",
  "Manufacturing",
  "Technology",
  "Healthcare",
  "Education",
  "Finance",
  "Real Estate",
  "Transportation",
];

export default function AddStaffPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  
  const [formData, setFormData] = useState({
    staffType: "",
    firstName: "",
    surName: "",
    email: "",
    password: "",
    sex: "Male",
    phoneNo: "",
    address: "",
    companyName: "",
    companyUserName: "",
    companyType: [] as string[],
    roles: ["Staff"] as string[],
    permissions: [] as string[],
  });

  const [currentUser, setCurrentUser] = useState<any>(null);

  useEffect(() => {
    if (!isAuthenticated() || !validateSession()) {
      router.push("/brand-owner/login");
      return;
    }

    const user = getStoredUser();
    if (!user) {
      router.push("/brand-owner/login");
      return;
    }

    setCurrentUser(user);

    // Pre-fill company name if user has one
    if (user.companyName) {
      setFormData(prev => ({ 
        ...prev, 
        companyName: user.companyName || "",
        companyUserName: (user.companyName || "").toLowerCase().replace(/\s+/g, "_")
      }));
    }
  }, [router]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (validationErrors[name]) {
      setValidationErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const toggleCompanyType = (type: string) => {
    setFormData(prev => ({
      ...prev,
      companyType: prev.companyType.includes(type)
        ? prev.companyType.filter(t => t !== type)
        : [...prev.companyType, type]
    }));
  };

  const validateForm = () => {
    const errors: Record<string, string> = {};
    
    if (!formData.staffType) {
      errors.staffType = "Staff Type is required";
    }
    if (!formData.firstName.trim()) {
      errors.firstName = "First Name is required";
    }
    if (!formData.surName.trim()) {
      errors.surName = "Surname is required";
    }
    if (!formData.email.trim()) {
      errors.email = "Email is required";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      errors.email = "Please enter a valid email address";
    }
    if (!formData.password) {
      errors.password = "Password is required";
    } else if (formData.password.length < 8) {
      errors.password = "Password must be at least 8 characters";
    }
    if (!formData.phoneNo.trim()) {
      errors.phoneNo = "Phone Number is required";
    } else if (!/^\+?[0-9]{10,15}$/.test(formData.phoneNo.replace(/\s/g, ''))) {
      errors.phoneNo = "Please enter a valid phone number";
    }
    if (!formData.companyName.trim()) {
      errors.companyName = "Company Name is required";
    }
    
    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    setIsLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const payload = {
        staffType: formData.staffType,
        firstName: formData.firstName.trim(),
        surName: formData.surName.trim(),
        email: formData.email.trim().toLowerCase(),
        password: formData.password,
        sex: formData.sex,
        staffCode: `${formData.firstName.slice(0, 3).toUpperCase()}${Math.floor(1000 + Math.random() * 9000)}`,
        phoneNo: formData.phoneNo.trim(),
        address: formData.address.trim() || "N/A",
        companyName: formData.companyName.trim(),
        companyUserName: formData.companyUserName.trim() || formData.companyName.toLowerCase().replace(/\s+/g, "_"),
        companyType: formData.companyType,
        roles: ["Staff"],
        permissions: [], // Empty permissions array
      };

      console.log("📤 Adding staff with payload:", payload);
      
      const result = await addStaff(payload);
      console.log("✅ Staff added successfully:", result);
      
      setSuccess(true);
      setTimeout(() => {
        router.push("/brand-owner/staff");
      }, 2000);

    } catch (err) {
      console.error("❌ Failed to add staff:", err);
      
      const errorMessage = err instanceof Error ? err.message : "Failed to add staff. Please try again.";
      
      if (errorMessage.toLowerCase().includes("email")) {
        setError("This email is already registered. Please use a different email.");
      } else if (errorMessage.toLowerCase().includes("phone")) {
        setError("This phone number is already registered. Please use a different number.");
      } else {
        setError(errorMessage);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const getFieldError = (fieldName: string) => {
    return validationErrors[fieldName] ? (
      <p className="text-xs text-red-500 mt-1">{validationErrors[fieldName]}</p>
    ) : null;
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Link 
            href="/brand-owner/staff" 
            className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
          >
            <ArrowLeft className="h-5 w-5 text-gray-500" />
          </Link>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Add Staff Member</h1>
            <p className="text-sm text-gray-500">Add a new staff member to your team</p>
          </div>
        </div>
        <Badge variant="outline">Step 1 of 1</Badge>
      </div>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 flex items-start gap-3">
          <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-medium">Error</p>
            <p className="text-sm">{error}</p>
          </div>
        </div>
      )}

      {success && (
        <div className="p-4 bg-green-50 border border-green-200 rounded-xl text-green-700 flex items-start gap-3">
          <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-medium">Success!</p>
            <p className="text-sm">Staff member added successfully. Redirecting...</p>
          </div>
        </div>
      )}

      <Card>
        <CardContent className="p-6">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Staff Type */}
            <div>
              <Label htmlFor="staffType" className="text-sm font-medium text-gray-700">
                Staff Type *
              </Label>
              <select
                id="staffType"
                name="staffType"
                value={formData.staffType}
                onChange={handleInputChange}
                className={`mt-1.5 w-full rounded-lg border px-3 py-2 focus:border-primary focus:ring-1 focus:ring-primary ${
                  validationErrors.staffType ? 'border-red-500' : 'border-gray-300'
                }`}
                required
                disabled={isLoading || success}
              >
                <option value="">Select staff type</option>
                {STAFF_TYPES.map((type) => (
                  <option key={type.value} value={type.value}>
                    {type.label}
                  </option>
                ))}
              </select>
              {getFieldError('staffType')}
            </div>

            {/* Name */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <Label htmlFor="firstName" className="text-sm font-medium text-gray-700">
                  First Name *
                </Label>
                <div className="relative mt-1.5">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input
                    id="firstName"
                    name="firstName"
                    value={formData.firstName}
                    onChange={handleInputChange}
                    placeholder="John"
                    className={`pl-10 ${validationErrors.firstName ? 'border-red-500' : ''}`}
                    required
                    disabled={isLoading || success}
                  />
                </div>
                {getFieldError('firstName')}
              </div>
              <div>
                <Label htmlFor="surName" className="text-sm font-medium text-gray-700">
                  Surname *
                </Label>
                <div className="relative mt-1.5">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input
                    id="surName"
                    name="surName"
                    value={formData.surName}
                    onChange={handleInputChange}
                    placeholder="Doe"
                    className={`pl-10 ${validationErrors.surName ? 'border-red-500' : ''}`}
                    required
                    disabled={isLoading || success}
                  />
                </div>
                {getFieldError('surName')}
              </div>
            </div>

            {/* Email & Phone */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <Label htmlFor="email" className="text-sm font-medium text-gray-700">
                  Email *
                </Label>
                <div className="relative mt-1.5">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    placeholder="staff@company.com"
                    className={`pl-10 ${validationErrors.email ? 'border-red-500' : ''}`}
                    required
                    disabled={isLoading || success}
                  />
                </div>
                {getFieldError('email')}
              </div>
              <div>
                <Label htmlFor="phoneNo" className="text-sm font-medium text-gray-700">
                  Phone Number *
                </Label>
                <div className="relative mt-1.5">
                  <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input
                    id="phoneNo"
                    name="phoneNo"
                    value={formData.phoneNo}
                    onChange={handleInputChange}
                    placeholder="+2348000000000"
                    className={`pl-10 ${validationErrors.phoneNo ? 'border-red-500' : ''}`}
                    required
                    disabled={isLoading || success}
                  />
                </div>
                {getFieldError('phoneNo')}
              </div>
            </div>

            {/* Password */}
            <div>
              <Label htmlFor="password" className="text-sm font-medium text-gray-700">
                Password *
              </Label>
              <div className="relative mt-1.5">
                <Key className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="password"
                  name="password"
                  type="password"
                  value={formData.password}
                  onChange={handleInputChange}
                  placeholder="Min 8 characters"
                  className={`pl-10 ${validationErrors.password ? 'border-red-500' : ''}`}
                  required
                  minLength={8}
                  disabled={isLoading || success}
                />
              </div>
              {getFieldError('password')}
              <p className="text-xs text-gray-400 mt-1">Password must be at least 8 characters long</p>
            </div>

            {/* Sex/Gender */}
            <div>
              <Label htmlFor="sex" className="text-sm font-medium text-gray-700">
                Gender
              </Label>
              <select
                id="sex"
                name="sex"
                value={formData.sex}
                onChange={handleInputChange}
                className="mt-1.5 w-full rounded-lg border border-gray-300 px-3 py-2 focus:border-primary focus:ring-1 focus:ring-primary"
                disabled={isLoading || success}
              >
                <option value="Male">Male</option>
                <option value="Female">Female</option>
                <option value="Other">Other</option>
              </select>
            </div>

            {/* Address */}
            <div>
              <Label htmlFor="address" className="text-sm font-medium text-gray-700">
                Address
              </Label>
              <div className="relative mt-1.5">
                <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="address"
                  name="address"
                  value={formData.address}
                  onChange={handleInputChange}
                  placeholder="123 Main Street"
                  className="pl-10"
                  disabled={isLoading || success}
                />
              </div>
            </div>

            {/* Company Name */}
            <div>
              <Label htmlFor="companyName" className="text-sm font-medium text-gray-700">
                Company Name *
              </Label>
              <div className="relative mt-1.5">
                <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="companyName"
                  name="companyName"
                  value={formData.companyName}
                  onChange={handleInputChange}
                  placeholder="Acme Logistics Ltd"
                  className={`pl-10 ${validationErrors.companyName ? 'border-red-500' : ''}`}
                  required
                  disabled={isLoading || success}
                />
              </div>
              {getFieldError('companyName')}
            </div>

            {/* Company User Name */}
            <div>
              <Label htmlFor="companyUserName" className="text-sm font-medium text-gray-700">
                Company Username
              </Label>
              <Input
                id="companyUserName"
                name="companyUserName"
                value={formData.companyUserName}
                onChange={handleInputChange}
                placeholder="acme_logistics"
                className="mt-1.5"
                disabled={isLoading || success}
              />
              <p className="text-xs text-gray-400 mt-1">Auto-generated if left empty</p>
            </div>

            {/* Company Types */}
            <div>
              <Label className="text-sm font-medium text-gray-700">
                Company Types (Optional)
              </Label>
              <div className="mt-2 grid grid-cols-2 md:grid-cols-3 gap-2">
                {COMPANY_TYPES.map((type) => {
                  const isSelected = formData.companyType.includes(type);
                  return (
                    <button
                      key={type}
                      type="button"
                      onClick={() => toggleCompanyType(type)}
                      className={`flex items-center gap-2 p-2 rounded-lg border text-sm transition-all ${
                        isSelected
                          ? 'border-primary bg-primary/5 text-primary'
                          : 'border-gray-200 hover:border-gray-300 text-gray-600'
                      }`}
                      disabled={isLoading || success}
                    >
                      <CheckCircle className={`h-4 w-4 ${isSelected ? 'text-primary' : 'text-gray-300'}`} />
                      <span>{type}</span>
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Submit */}
            <div className="flex gap-3 pt-4 border-t border-gray-200">
              <Button
                type="button"
                variant="outline"
                onClick={() => router.push("/brand-owner/staff")}
                className="flex-1"
                disabled={isLoading || success}
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isLoading || success}
                className="flex-1 bg-primary hover:bg-primary/90 text-white"
              >
                {isLoading ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                    Adding Staff...
                  </>
                ) : (
                  <>
                    <Plus className="h-4 w-4 mr-2" />
                    Add Staff
                  </>
                )}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}