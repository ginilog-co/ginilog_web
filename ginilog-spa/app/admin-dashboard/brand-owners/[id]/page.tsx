// app/admin-dashboard/brand-owners/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter, useParams } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Loader2, ArrowLeft, Building2, Mail, Phone, MapPin, 
  Users, Star, Calendar, Edit, Trash2, User, Briefcase,
  CheckCircle, XCircle, Clock
} from "lucide-react";
import { getBrandOwnerById } from "@/lib/api";

export default function BrandOwnerDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [owner, setOwner] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchOwner = async () => {
      try {
        setIsLoading(true);
        const data = await getBrandOwnerById(params.id as string);
        setOwner(data);
      } catch (error) {
        console.error("Failed to fetch brand owner:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchOwner();
  }, [params.id]);

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!owner) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Brand owner not found</p>
        <Link href="/admin-dashboard/brand-owners">
          <Button className="mt-4">Back to Brand Owners</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/brand-owners">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Brand Owner Details</h1>
          <p className="text-sm text-gray-500">View and manage brand owner information</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Profile Card */}
        <Card className="lg:col-span-1">
          <CardContent className="p-6 text-center">
            <div className="h-24 w-24 rounded-full bg-purple-100 flex items-center justify-center mx-auto">
              <span className="text-3xl font-bold text-purple-600">
                {owner.firstName?.[0]}{owner.surName?.[0]}
              </span>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mt-4">
              {owner.firstName} {owner.surName}
            </h2>
            <p className="text-sm text-gray-500">{owner.staffType || "Brand Owner"}</p>
            <Badge className="mt-2 bg-green-100 text-green-800">Active</Badge>

            <div className="mt-6 space-y-3 text-left">
              <div className="flex items-center gap-3 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{owner.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Phone className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{owner.phoneNo || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <MapPin className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{owner.address || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">
                  Joined {new Date(owner.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-6 flex gap-2">
              <Link href={`/admin-dashboard/brand-owners/${owner.id}/edit`} className="flex-1">
                <Button variant="outline" className="w-full">
                  <Edit className="h-4 w-4 mr-2" /> Edit
                </Button>
              </Link>
              <Button variant="outline" className="text-red-500 border-red-200 hover:bg-red-50">
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Company Info */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5 text-primary" />
              Company Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Company Name</p>
                <p className="font-semibold text-gray-900">{owner.companyName}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Username</p>
                <p className="font-semibold text-gray-900">{owner.companyUserName}</p>
              </div>
            </div>

            <div>
              <p className="text-sm text-gray-500 mb-2">Company Types</p>
              <div className="flex flex-wrap gap-2">
                {owner.companyType?.map((type: string) => (
                  <Badge key={type} className="bg-blue-100 text-blue-800">
                    {type}
                  </Badge>
                ))}
              </div>
            </div>

            <div>
              <p className="text-sm text-gray-500 mb-2">Permissions</p>
              <div className="flex flex-wrap gap-2">
                {owner.permissions?.slice(0, 6).map((perm: string) => (
                  <Badge key={perm} variant="secondary" className="text-xs">
                    {perm}
                  </Badge>
                ))}
                {(owner.permissions?.length || 0) > 6 && (
                  <Badge variant="secondary" className="text-xs">
                    +{owner.permissions.length - 6} more
                  </Badge>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}