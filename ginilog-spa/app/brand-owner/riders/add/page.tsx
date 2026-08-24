"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ArrowLeft, Plus, UserCheck } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { addRider } from "@/lib/api";

export default function BrandOwnerAddRiderPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: "",
    vehicleType: "Bike",
    status: "Available",
  });

  const handleSubmit = async () => {
    setLoading(true);
    try {
      await addRider({
        ...form,
        password: "Temp@1234",
        profilePictureUrl: "",
        rating: 0,
        available: true,
        status: (form.status as "Available" | "On Delivery" | "Off Duty") || "Available",
      });
      router.push("/brand-owner/riders");
    } catch (error) {
      console.error("Failed to add rider:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/riders">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Add Rider</h1>
          <p className="text-sm text-gray-500">Register a new rider for logistics tasks</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserCheck className="h-5 w-5 text-primary" />
            Rider details
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="firstName">First name</Label>
              <Input id="firstName" value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="lastName">Last name</Label>
              <Input id="lastName" value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="phoneNumber">Phone</Label>
              <Input id="phoneNumber" value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="vehicleType">Vehicle type</Label>
              <Input id="vehicleType" value={form.vehicleType} onChange={(e) => setForm({ ...form, vehicleType: e.target.value })} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <Input id="status" value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })} />
            </div>
          </div>

          <Button onClick={handleSubmit} disabled={loading} className="gap-2">
            <Plus className="h-4 w-4" />
            {loading ? "Saving..." : "Save rider"}
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
