// app/brand-owner/staff/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Loader2,
  ArrowLeft,
  Mail,
  Phone,
  Calendar,
  Edit,
  Trash2,
  User,
  Building2,
  Briefcase,
  Copy,
  MapPin,
  Users,
  Clock,
  CheckCircle,
  XCircle,
  Shield,
  Key
} from "lucide-react";
import { getStaffById, deleteStaff } from "@/lib/api";

export default function StaffDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [staff, setStaff] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetchStaff = async () => {
      try {
        setIsLoading(true);
        const data = await getStaffById(params.id as string);
        setStaff(data);
      } catch (error) {
        console.error("Failed to fetch staff:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchStaff();
  }, [params.id]);

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this staff member?")) return;
    setDeleting(true);
    try {
      await deleteStaff(params.id as string);
      router.push("/brand-owner/staff");
    } catch (error) {
      console.error("Failed to delete staff:", error);
    } finally {
      setDeleting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!staff) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Staff member not found</p>
        <Link href="/brand-owner/staff">
          <Button className="mt-4">Back to Staff</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/staff">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Staff Details</h1>
          <p className="text-sm text-gray-500">View staff member information</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Profile Card */}
        <Card className="lg:col-span-1">
          <CardContent className="p-6 text-center">
            <div className="h-24 w-24 rounded-full bg-blue-100 flex items-center justify-center mx-auto">
              <span className="text-3xl font-bold text-blue-600">
                {staff.firstName?.[0]}{staff.surName?.[0]}
              </span>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mt-4">
              {staff.firstName} {staff.surName}
            </h2>
            <p className="text-sm text-gray-500">{staff.staffType || "Staff Member"}</p>
            <Badge className="mt-2 bg-green-100 text-green-800">Active</Badge>

            <div className="mt-6 space-y-3 text-left">
              <div className="flex items-center gap-3 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{staff.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Phone className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{staff.phoneNo || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">
                  Joined {new Date(staff.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-500">Staff Code</span>
                <span className="font-mono text-xs">{staff.staffCode || "N/A"}</span>
              </div>
            </div>

            <div className="mt-6 flex gap-2">
              <Link href={`/brand-owner/staff/${staff.id}/edit`} className="flex-1">
                <Button variant="outline" className="w-full">
                  <Edit className="h-4 w-4 mr-2" /> Edit
                </Button>
              </Link>
              <Button
                variant="outline"
                className="text-red-500 border-red-200 hover:bg-red-50"
                onClick={handleDelete}
                disabled={deleting}
              >
                {deleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Company & Permissions Info */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Briefcase className="h-5 w-5 text-primary" />
              Company & Permissions
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Company Name</p>
                <p className="font-semibold text-gray-900">{staff.companyName || "N/A"}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Company Type</p>
                <div className="flex flex-wrap gap-1 mt-1">
                  {staff.companyType?.map((type: string) => (
                    <Badge key={type} className="bg-blue-100 text-blue-800 text-xs">
                      {type}
                    </Badge>
                  ))}
                </div>
              </div>
            </div>

            <div>
              <p className="text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                <Shield className="h-4 w-4 text-gray-400" />
                Roles
              </p>
              <div className="flex flex-wrap gap-2">
                {staff.roles?.map((role: string) => (
                  <Badge key={role} className="bg-purple-100 text-purple-800">
                    {role}
                  </Badge>
                ))}
              </div>
            </div>

            <div>
              <p className="text-sm font-medium text-gray-700 mb-2 flex items-center gap-2">
                <Key className="h-4 w-4 text-gray-400" />
                Permissions
              </p>
              <div className="flex flex-wrap gap-1">
                {staff.permissions?.map((perm: string) => (
                  <Badge key={perm} variant="secondary" className="text-xs">
                    {perm}
                  </Badge>
                ))}
              </div>
            </div>

            {staff.address && (
              <div className="flex items-start gap-2 p-3 bg-gray-50 rounded-xl">
                <MapPin className="h-4 w-4 text-gray-400 flex-shrink-0 mt-0.5" />
                <p className="text-sm text-gray-600">{staff.address}</p>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}