"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { createAdmin } from "@/lib/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Loader2, ArrowLeft, Eye, EyeOff, UserCog, Shield } from "lucide-react";

export default function CreateAdminPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  
  const [formData, setFormData] = useState({
    adminType: "Super_Admin",
    firstName: "",
    surName: "",          // Use surName instead of lastName
    email: "",
    password: "",
    sex: "Male",
    staffCode: `ADMIN${Math.floor(1000 + Math.random() * 9000)}`,
    phoneNo: "",
    state: "",
    locality: "",
    address: "",
    branch: "",
    roles: ["Super_Admin"],
    permissions: [
      "CanAccessAllData",
      "CanDeleteAllData",
      "CanAssignRoles",
      "CanViewAdmin",
      "CanDeleteAdmin",
      "CanUpdateAdmin",
      "CanCreateAdmin",
      "CanManageUsers",
      "CanDeleteUsers",
      "CanCreateStaff",
      "CanManageStaff",
      "CanViewStaff",
      "CanDeleteStaff",
      "CanViewBrands",
      "CanManageBrands",
      "CanDeleteBrands",
      "CanCreateProduct",
      "CanViewProduct",
      "CanManageProduct",
      "CanDeleteProduct",
      "CanViewWallet",
      "CanManageWallet",
      "CanViewOrders",
      "CanManageOrders",
      "CanDeleteOrders",
      "CanViewBookings",
      "CanManageBookings",
      "CanDeleteBookings"
    ],
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      console.log("Submitting form data:", formData);
      await createAdmin(formData);
      router.push("/admin-dashboard/admins");
      router.refresh();
    } catch (err: any) {
      console.error("Error creating admin:", err);
      setError(err.message || "Failed to create admin. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSelectChange = (name: string, value: string) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  return (
    <div className="container mx-auto py-8 px-4">
      <div className="max-w-3xl mx-auto">
        <div className="flex items-center gap-4 mb-8">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => router.push("/admin-dashboard/admins")}
          >
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold flex items-center gap-2">
              <Shield className="h-8 w-8 text-primary" />
              Create Admin
            </h1>
            <p className="text-muted-foreground">Add a new administrator to the system</p>
          </div>
        </div>

        {error && (
          <div className="bg-destructive/10 text-destructive p-4 rounded-lg mb-6 whitespace-pre-wrap">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="adminType">Admin Type *</Label>
              <Select
                value={formData.adminType}
                onValueChange={(value) => handleSelectChange("adminType", value)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select admin type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Super_Admin">Super Admin</SelectItem>
                  <SelectItem value="Admin">Admin</SelectItem>
                  <SelectItem value="Staff">Staff</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="staffCode">Staff Code *</Label>
              <Input
                id="staffCode"
                name="staffCode"
                value={formData.staffCode}
                onChange={handleChange}
                placeholder="Enter staff code"
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="firstName">First Name *</Label>
              <Input
                id="firstName"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                placeholder="Enter first name"
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="surName">Surname *</Label>
              <Input
                id="surName"
                name="surName"
                value={formData.surName}
                onChange={handleChange}
                placeholder="Enter surname"
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email *</Label>
              <Input
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                placeholder="Enter email address"
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="phoneNo">Phone Number *</Label>
              <Input
                id="phoneNo"
                name="phoneNo"
                value={formData.phoneNo}
                onChange={handleChange}
                placeholder="Enter phone number"
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="sex">Sex *</Label>
              <Select
                value={formData.sex}
                onValueChange={(value) => handleSelectChange("sex", value)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select sex" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Male">Male</SelectItem>
                  <SelectItem value="Female">Female</SelectItem>
                  <SelectItem value="Other">Other</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="state">State *</Label>
              <Input
                id="state"
                name="state"
                value={formData.state}
                onChange={handleChange}
                placeholder="Enter state"
                required
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="locality">Locality *</Label>
              <Input
                id="locality"
                name="locality"
                value={formData.locality}
                onChange={handleChange}
                placeholder="Enter locality"
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="branch">Branch *</Label>
              <Input
                id="branch"
                name="branch"
                value={formData.branch}
                onChange={handleChange}
                placeholder="Enter branch"
                required
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="address">Address *</Label>
            <Input
              id="address"
              name="address"
              value={formData.address}
              onChange={handleChange}
              placeholder="Enter address"
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="password">Password *</Label>
            <div className="relative">
              <Input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                value={formData.password}
                onChange={handleChange}
                placeholder="Enter password (min 8 characters)"
                required
                minLength={8}
              />
              <Button
                type="button"
                variant="ghost"
                size="icon"
                className="absolute right-0 top-0 h-full"
                onClick={() => setShowPassword(!showPassword)}
              >
                {showPassword ? (
                  <EyeOff className="h-4 w-4" />
                ) : (
                  <Eye className="h-4 w-4" />
                )}
              </Button>
            </div>
            <p className="text-sm text-muted-foreground">
              Password must be at least 8 characters long with uppercase, lowercase, number and special character
            </p>
          </div>

          <div className="flex gap-4 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => router.push("/admin-dashboard/admins")}
              className="flex-1"
            >
              Cancel
            </Button>
            <Button type="submit" disabled={loading} className="flex-1">
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Creating...
                </>
              ) : (
                <>
                  <UserCog className="mr-2 h-4 w-4" />
                  Create Admin
                </>
              )}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}