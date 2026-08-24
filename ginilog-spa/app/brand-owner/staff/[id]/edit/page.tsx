// app/brand-owner/staff/[id]/edit/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Eye, EyeOff } from "lucide-react";
import { getStaffById, updateStaff } from "@/lib/api";

export default function EditStaffPage() {
  const router = useRouter();
  const params = useParams();
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [formData, setFormData] = useState({
    firstName: "",
    surName: "",
    phoneNo: "",
    address: "",
    staffCode: "",
  });

  useEffect(() => {
    const fetchStaff = async () => {
      try {
        const data = await getStaffById(params.id as string);
        setFormData({
          firstName: data.firstName || "",
          surName: data.surName || "",
          phoneNo: data.phoneNo || "",
          address: data.address || "",
          staffCode: data.staffCode || "",
        });
      } catch (error) {
        console.error("Failed to fetch staff:", error);
        setError("Failed to load staff data");
      } finally {
        setIsLoading(false);
      }
    };
    fetchStaff();
  }, [params.id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setIsSubmitting(true);

    try {
      await updateStaff(formData);
      setSuccess("Staff updated successfully!");
      setTimeout(() => router.push(`/brand-owner/staff/${params.id}`), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update staff");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href={`/brand-owner/staff/${params.id}`}>
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Edit Staff</h1>
          <p className="text-sm text-gray-500">Update staff member information</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Staff Details</CardTitle>
        </CardHeader>
        <CardContent>
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
              {error}
            </div>
          )}
          {success && (
            <div className="mb-4 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl text-sm">
              {success}
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
                />
              </div>
              <div>
                <Label htmlFor="surName">Last Name *</Label>
                <Input
                  id="surName"
                  value={formData.surName}
                  onChange={(e) => setFormData({ ...formData, surName: e.target.value })}
                  required
                />
              </div>
            </div>

            <div>
              <Label htmlFor="phoneNo">Phone Number</Label>
              <Input
                id="phoneNo"
                type="tel"
                value={formData.phoneNo}
                onChange={(e) => setFormData({ ...formData, phoneNo: e.target.value })}
              />
            </div>

            <div>
              <Label htmlFor="address">Address</Label>
              <Input
                id="address"
                value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              />
            </div>

            <div>
              <Label htmlFor="staffCode">Staff Code</Label>
              <Input
                id="staffCode"
                value={formData.staffCode}
                onChange={(e) => setFormData({ ...formData, staffCode: e.target.value })}
                disabled
                className="bg-gray-50"
              />
              <p className="text-xs text-gray-500 mt-1">Staff code cannot be changed</p>
            </div>

            <div className="flex gap-4 pt-4">
              <Button type="submit" disabled={isSubmitting} className="flex-1">
                {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                {isSubmitting ? "Saving..." : "Save Changes"}
              </Button>
              <Link href={`/brand-owner/staff/${params.id}`} className="flex-1">
                <Button type="button" variant="outline" className="w-full">Cancel</Button>
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}