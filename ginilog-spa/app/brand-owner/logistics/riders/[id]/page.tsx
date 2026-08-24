// app/brand-owner/logistics/riders/[id]/page.tsx
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
  User,
  Mail,
  Phone,
  Car,
  Bike,
  MapPin,
  Calendar,
  Star,
  Edit,
  Trash2,
  CheckCircle,
  Clock,
  Navigation,
  Truck
} from "lucide-react";
import { getRiderById, deleteRider } from "@/lib/api";

export default function RiderDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [rider, setRider] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetchRider = async () => {
      try {
        setIsLoading(true);
        const data = await getRiderById(params.id as string);
        setRider(data);
      } catch (error) {
        console.error("Failed to fetch rider:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchRider();
  }, [params.id]);

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this rider?")) return;
    setDeleting(true);
    try {
      await deleteRider(params.id as string);
      router.push("/brand-owner/logistics");
    } catch (error) {
      console.error("Failed to delete rider:", error);
    } finally {
      setDeleting(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      available: { label: "Available", className: "bg-green-100 text-green-800", icon: CheckCircle },
      "on delivery": { label: "On Delivery", className: "bg-blue-100 text-blue-800", icon: Navigation },
      "off duty": { label: "Off Duty", className: "bg-gray-100 text-gray-800", icon: Clock },
    };
    return statusMap[status?.toLowerCase()] || statusMap.available;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!rider) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Rider not found</p>
        <Link href="/brand-owner/logistics">
          <Button className="mt-4">Back to Logistics</Button>
        </Link>
      </div>
    );
  }

  const status = getStatusBadge(rider.status);
  const StatusIcon = status.icon;

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/logistics">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Rider Details</h1>
          <p className="text-sm text-gray-500">{rider.firstName} {rider.lastName}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-1">
          <CardContent className="p-6 text-center">
            <div className="h-24 w-24 rounded-full bg-blue-100 flex items-center justify-center mx-auto">
              <span className="text-3xl font-bold text-blue-600">
                {rider.firstName?.[0]}{rider.lastName?.[0]}
              </span>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mt-4">
              {rider.firstName} {rider.lastName}
            </h2>
            <Badge className={`${status.className} mt-2 flex items-center gap-1 justify-center`}>
              <StatusIcon className="h-3 w-3" />
              {status.label}
            </Badge>

            <div className="mt-6 space-y-3 text-left">
              <div className="flex items-center gap-3 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{rider.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Phone className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{rider.phoneNumber || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">
                  Joined {new Date(rider.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t">
              <div className="flex items-center justify-between text-sm">
                <span className="text-gray-500">Rating</span>
                <div className="flex items-center gap-1">
                  <Star className="h-4 w-4 text-yellow-500 fill-yellow-500" />
                  <span>{rider.rating || 0}</span>
                </div>
              </div>
              <div className="flex items-center justify-between text-sm mt-2">
                <span className="text-gray-500">Deliveries</span>
                <span>{rider.deliveries || 0}</span>
              </div>
            </div>

            <div className="mt-6 flex gap-2">
              <Link href={`/brand-owner/logistics/riders/${rider.id}/edit`} className="flex-1">
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

        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Truck className="h-5 w-5 text-primary" />
              Vehicle & Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Vehicle Type</p>
                <div className="flex items-center gap-2 mt-1">
                  {rider.vehicleType?.toLowerCase().includes("bike") ? (
                    <Bike className="h-5 w-5 text-gray-600" />
                  ) : (
                    <Car className="h-5 w-5 text-gray-600" />
                  )}
                  <span className="font-semibold">{rider.vehicleType || "N/A"}</span>
                </div>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">License Number</p>
                <p className="font-semibold">{rider.licenseNumber || "N/A"}</p>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Status</p>
                <Badge className={`${status.className} mt-1`}>
                  {status.label}
                </Badge>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Availability</p>
                <Badge className={rider.available ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}>
                  {rider.available ? "Available" : "Unavailable"}
                </Badge>
              </div>
            </div>

            {rider.address && (
              <div className="flex items-start gap-2 p-3 bg-gray-50 rounded-xl">
                <MapPin className="h-4 w-4 text-gray-400 flex-shrink-0 mt-0.5" />
                <p className="text-sm text-gray-600">{rider.address}</p>
              </div>
            )}

            <div className="p-4 bg-gray-50 rounded-xl">
              <p className="text-sm font-medium text-gray-700 mb-2">Bank Details</p>
              <div className="grid grid-cols-3 gap-3 text-sm">
                <div>
                  <p className="text-gray-500">Bank</p>
                  <p className="font-medium">{rider.bankName || "N/A"}</p>
                </div>
                <div>
                  <p className="text-gray-500">Account Name</p>
                  <p className="font-medium">{rider.accountName || "N/A"}</p>
                </div>
                <div>
                  <p className="text-gray-500">Account Number</p>
                  <p className="font-medium">{rider.accountNumber || "N/A"}</p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}