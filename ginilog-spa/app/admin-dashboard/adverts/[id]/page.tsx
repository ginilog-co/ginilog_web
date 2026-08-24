// app/admin-dashboard/adverts/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Loader2, ArrowLeft, Calendar, DollarSign, 
  Edit, Trash2, Megaphone, Clock, Eye,
  Share2, Download, Copy, CheckCircle
} from "lucide-react";
import { getAdvertById, deleteAdvert } from "@/lib/api";

export default function AdvertDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [advert, setAdvert] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    const fetchAdvert = async () => {
      try {
        setIsLoading(true);
        const data = await getAdvertById(params.id as string);
        setAdvert(data);
      } catch (error) {
        console.error("Failed to fetch advert:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchAdvert();
  }, [params.id]);

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this advert?")) return;
    setDeleting(true);
    try {
      await deleteAdvert(params.id as string);
      router.push("/admin-dashboard/adverts");
    } catch (error) {
      console.error("Failed to delete advert:", error);
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

  if (!advert) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Advert not found</p>
        <Link href="/admin-dashboard/adverts">
          <Button className="mt-4">Back to Adverts</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/adverts">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Advert Details</h1>
          <p className="text-sm text-gray-500">View advert information</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Image Card */}
        <Card className="lg:col-span-1 overflow-hidden">
          <div className="h-64 bg-gray-200">
            {advert.advertImage ? (
              <img
                src={advert.advertImage}
                alt={advert.advertName}
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="flex items-center justify-center h-full bg-gradient-to-br from-purple-100 to-blue-100">
                <Megaphone className="h-16 w-16 text-gray-400" />
              </div>
            )}
          </div>
          <CardContent className="p-6">
            <Badge className="mb-2 bg-blue-100 text-blue-800">{advert.advertType}</Badge>
            <h3 className="text-xl font-bold text-gray-900">{advert.advertName}</h3>
            <p className="text-gray-600 mt-2 line-clamp-3">{advert.advertItemDescription}</p>
            
            <div className="mt-4 flex items-center justify-between">
              <div className="flex items-center gap-1 text-lg font-bold text-primary">
                <DollarSign className="h-5 w-5" />
                ₦{advert.advertItemCost?.toLocaleString() || 0}
              </div>
              <div className="flex items-center gap-1 text-sm text-gray-500">
                <Clock className="h-4 w-4" />
                {advert.advertDays4} days
              </div>
            </div>

            <div className="mt-4 flex gap-2">
              <Button variant="outline" className="flex-1 gap-2" onClick={handleCopy}>
                {copied ? <CheckCircle className="h-4 w-4 text-green-500" /> : <Copy className="h-4 w-4" />}
                {copied ? "Copied!" : "Copy Link"}
              </Button>
              <Button variant="outline" className="flex-1 gap-2">
                <Share2 className="h-4 w-4" />
                Share
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Details Card */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Eye className="h-5 w-5 text-primary" />
              Advert Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Advert Name</p>
                <p className="font-semibold text-gray-900">{advert.advertName}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Type</p>
                <p className="font-semibold text-gray-900">{advert.advertType}</p>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Cost</p>
                <p className="font-semibold text-primary">
                  ₦{advert.advertItemCost?.toLocaleString() || 0}
                </p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Duration</p>
                <p className="font-semibold text-gray-900">{advert.advertDays4} days</p>
              </div>
            </div>

            <div className="p-4 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Description</p>
              <p className="text-gray-700 mt-1">{advert.advertItemDescription}</p>
            </div>

            <div className="flex items-center gap-4 text-sm text-gray-500">
              <div className="flex items-center gap-1">
                <Calendar className="h-4 w-4" />
                Created: {new Date(advert.createdAt).toLocaleDateString()}
              </div>
              <Badge variant="secondary">Active</Badge>
            </div>

            <div className="flex gap-2 pt-4 border-t">
              <Link href={`/admin-dashboard/adverts/${advert.id}/edit`} className="flex-1">
                <Button variant="outline" className="w-full">
                  <Edit className="h-4 w-4 mr-2" /> Edit Advert
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
      </div>
    </div>
  );
}