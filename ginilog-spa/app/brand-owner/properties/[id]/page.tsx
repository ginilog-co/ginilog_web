// app/brand-owner/properties/[id]/page.tsx
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
  MapPin,
  Star,
  DollarSign,
  Calendar,
  Users,
  Edit,
  Trash2,
  Hotel,
  Wifi,
  CarFront,
  Coffee,
  Waves,
  Dumbbell,
  CheckCircle,
  XCircle,
  Copy,
  Share2
} from "lucide-react";
import { getAccommodationById, deleteAccommodation } from "@/lib/api";

const facilityIcons: Record<string, any> = {
  "Wi-Fi": Wifi,
  "Parking": CarFront,
  "Coffee": Coffee,
  "Pool": Waves,
  "Gym": Dumbbell,
  "Swimming": Waves,
  "Swimming Pool": Waves,
};

export default function PropertyDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [property, setProperty] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    const fetchProperty = async () => {
      try {
        setIsLoading(true);
        const data = await getAccommodationById(params.id as string);
        setProperty(data);
      } catch (error) {
        console.error("Failed to fetch property:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchProperty();
  }, [params.id]);

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this property?")) return;
    setDeleting(true);
    try {
      await deleteAccommodation(params.id as string);
      router.push("/brand-owner/properties");
    } catch (error) {
      console.error("Failed to delete property:", error);
    } finally {
      setDeleting(false);
    }
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(window.location.href);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!property) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Property not found</p>
        <Link href="/brand-owner/properties">
          <Button className="mt-4">Back to Properties</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/properties">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{property.accomodationName}</h1>
          <p className="text-sm text-gray-500">{property.accomodationType}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-2">
          <div className="relative h-64 bg-gray-200 rounded-t-xl overflow-hidden">
            {property.accomodationImages?.[0] ? (
              <img
                src={property.accomodationImages[0]}
                alt={property.accomodationName}
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="flex items-center justify-center h-full bg-gray-100">
                <Hotel className="h-16 w-16 text-gray-400" />
              </div>
            )}
            <Badge
              className={`absolute top-4 right-4 ${property.available ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}
            >
              {property.available ? 'Available' : 'Booked'}
            </Badge>
          </div>
          <CardContent className="p-6">
            <div className="flex items-start justify-between">
              <div>
                <h2 className="text-xl font-bold text-gray-900">{property.accomodationName}</h2>
                <div className="flex items-center gap-2 text-sm text-gray-500 mt-1">
                  <MapPin className="h-4 w-4" />
                  {property.location}
                </div>
              </div>
              <div className="flex items-center gap-1">
                <Star className="h-5 w-5 text-yellow-500 fill-yellow-500" />
                <span className="text-lg font-medium">{property.rating || '4.5'}</span>
              </div>
            </div>

            <div className="flex items-center gap-6 mt-4 text-sm">
              <div className="flex items-center gap-1">
                <DollarSign className="h-4 w-4 text-gray-400" />
                <span className="font-medium">₦{property.bookingAmount?.toLocaleString() || 0}</span>
                <span className="text-gray-500">/night</span>
              </div>
              <div className="flex items-center gap-1">
                <Users className="h-4 w-4 text-gray-400" />
                <span>{property.noOfRooms || 0} rooms</span>
              </div>
            </div>

            <div className="mt-4">
              <h3 className="font-medium text-gray-900">Description</h3>
              <p className="text-sm text-gray-600 mt-1">{property.accomodationDescription || "No description provided."}</p>
            </div>

            {property.accomodationFacilities?.length > 0 && (
              <div className="mt-4">
                <h3 className="font-medium text-gray-900">Facilities</h3>
                <div className="flex flex-wrap gap-2 mt-2">
                  {property.accomodationFacilities.map((facility: string) => {
                    const Icon = facilityIcons[facility] || CheckCircle;
                    return (
                      <Badge key={facility} className="bg-gray-100 text-gray-700 flex items-center gap-1">
                        <Icon className="h-3 w-3" />
                        {facility}
                      </Badge>
                    );
                  })}
                </div>
              </div>
            )}

            {property.accomodationImages?.length > 1 && (
              <div className="mt-4">
                <h3 className="font-medium text-gray-900">More Images</h3>
                <div className="flex gap-2 mt-2 overflow-x-auto pb-2">
                  {property.accomodationImages.slice(1, 4).map((img: string, idx: number) => (
                    <div key={idx} className="h-20 w-20 rounded-lg overflow-hidden flex-shrink-0">
                      <img src={img} alt={`${property.accomodationName} ${idx + 2}`} className="h-full w-full object-cover" />
                    </div>
                  ))}
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        <Card className="lg:col-span-1">
          <CardHeader>
            <CardTitle>Actions</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <Link href={`/brand-owner/properties/${property.id}/edit`} className="block">
              <Button variant="outline" className="w-full gap-2">
                <Edit className="h-4 w-4" />
                Edit Property
              </Button>
            </Link>
            <Button variant="outline" className="w-full gap-2" onClick={handleCopy}>
              {copied ? <CheckCircle className="h-4 w-4 text-green-500" /> : <Copy className="h-4 w-4" />}
              {copied ? "Copied!" : "Copy Link"}
            </Button>
            <Button variant="outline" className="w-full gap-2">
              <Share2 className="h-4 w-4" />
              Share
            </Button>
            <Button
              variant="outline"
              className="w-full gap-2 text-red-500 border-red-200 hover:bg-red-50"
              onClick={handleDelete}
              disabled={deleting}
            >
              {deleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
              Delete Property
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}