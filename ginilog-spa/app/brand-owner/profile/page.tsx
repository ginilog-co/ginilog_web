// app/brand-owner/profile/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  User,
  Mail,
  Phone,
  Building2,
  MapPin,
  Calendar,
  Edit,
  Loader2,
  Shield,
  CheckCircle,
  Clock,
  Star,
  Users,
  Home,
  Briefcase
} from "lucide-react";
import { getStoredUser, getBrandOwnerById } from "@/lib/api";

export default function BrandOwnerProfilePage() {
  const router = useRouter();
  const [profile, setProfile] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const user = getStoredUser();
        if (!user) {
          router.push("/brand-owner/login");
          return;
        }
        
        const data = await getBrandOwnerById(user.userId || user.companyId || "");
        setProfile(data);
      } catch (error) {
        console.error("Failed to fetch profile:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfile();
  }, [router]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Profile not found</p>
        <Button className="mt-4" onClick={() => router.push("/brand-owner")}>
          Back to Dashboard
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Profile</h1>
          <p className="text-sm text-gray-500">View and manage your profile information</p>
        </div>
        <Link href="/brand-owner/profile/edit">
          <Button className="gap-2">
            <Edit className="h-4 w-4" />
            Edit Profile
          </Button>
        </Link>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-1">
          <CardContent className="p-6 text-center">
            <div className="h-24 w-24 rounded-full bg-primary/10 flex items-center justify-center mx-auto">
              {profile.profilePicture ? (
                <img
                  src={profile.profilePicture}
                  alt={profile.firstName}
                  className="h-full w-full rounded-full object-cover"
                />
              ) : (
                <span className="text-3xl font-bold text-primary">
                  {profile.firstName?.[0]}{profile.surName?.[0]}
                </span>
              )}
            </div>
            <h2 className="text-xl font-bold text-gray-900 mt-4">
              {profile.firstName} {profile.surName}
            </h2>
            <p className="text-sm text-gray-500">{profile.staffType || "Brand Owner"}</p>
            <Badge className="mt-2 bg-green-100 text-green-800">Active</Badge>

            <div className="mt-6 space-y-3 text-left">
              <div className="flex items-center gap-3 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{profile.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Phone className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{profile.phoneNo || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">
                  Joined {new Date(profile.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-500">Staff Code</span>
                <span className="font-mono text-xs">{profile.staffCode || "N/A"}</span>
              </div>
              <div className="flex items-center justify-between text-sm mt-2">
                <span className="text-gray-500">Account Type</span>
                <Badge variant="secondary">{profile.staffType || "Brand Owner"}</Badge>
              </div>
            </div>
          </CardContent>
        </Card>

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
                <p className="font-semibold text-gray-900">{profile.companyName}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Username</p>
                <p className="font-semibold text-gray-900">{profile.companyUserName}</p>
              </div>
            </div>

            <div>
              <p className="text-sm text-gray-500 mb-2">Company Types</p>
              <div className="flex flex-wrap gap-2">
                {profile.companyType?.map((type: string) => (
                  <Badge key={type} className="bg-blue-100 text-blue-800">
                    {type}
                  </Badge>
                ))}
              </div>
            </div>

            <div className="p-4 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Business Address</p>
              <p className="text-gray-700">{profile.address || "N/A"}</p>
            </div>

            <div>
              <p className="text-sm text-gray-500 mb-2">Permissions</p>
              <div className="flex flex-wrap gap-1">
                {profile.permissions?.slice(0, 8).map((perm: string) => (
                  <Badge key={perm} variant="secondary" className="text-xs">
                    {perm}
                  </Badge>
                ))}
                {(profile.permissions?.length || 0) > 8 && (
                  <Badge variant="secondary" className="text-xs">
                    +{profile.permissions.length - 8} more
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