// app/brand-owner/adverts/[id]/page.tsx
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
  Calendar,
  DollarSign,
  Edit,
  Trash2,
  Megaphone,
  Clock,
  Eye,
  Share2,
  Copy,
  CheckCircle
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
      router.push("/brand-owner/adverts");
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
        <Link href="/brand-owner/adverts">
          <Button className="mt-4">Back to Adverts</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/adverts">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{advert.advertName}</h1>
          <p className="text-sm text-gray-500">{advert.advertType} Advert</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="lg:col-span-2">
          <div className="relative h-64 bg-gray-200 rounded-t-xl overflow-hidden">
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
            <Badge className="absolute top-4 right-4 bg-white/90 text-gray-700">
              {advert.advertType}
            </Badge>
          </div>
          <CardContent className="p-6">
            <h2 className="text-xl font-bold text-gray-900">{advert.advertName}</h2>
            <p className="text-gray-600 mt-2">{advert.advertItemDescription}</p>

            <div className="flex items-center gap-6 mt-4 text-sm">
              <div className="flex items-center gap-1">
                <DollarSign className="h-4 w-4 text-gray-400" />
                <span className="font-medium">₦{advert.advertItemCost?.toLocaleString() || 0}</span>
              </div>
              <div className="flex items-center gap-1">
                <Clock className="h-4 w-4 text-gray-400" />
                <span>{advert.advertDays4} days</span>
              </div>
              <div className="flex items-center gap-1">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span>Created {new Date(advert.createdAt).toLocaleDateString()}</span>
              </div>
            </div>

            <div className="mt-4 p-4 bg-gray-50 rounded-xl">
              <p className="text-sm font-medium text-gray-700">Advert Status</p>
              <Badge className="mt-1 bg-green-100 text-green-800">Active</Badge>
            </div>
          </CardContent>
        </Card>

        <Card className="lg:col-span-1">
          <CardHeader>
            <CardTitle>Actions</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <Link href={`/brand-owner/adverts/${advert.id}/edit`} className="block">
              <Button variant="outline" className="w-full gap-2">
                <Edit className="h-4 w-4" />
                Edit Advert
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
              Delete Advert
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}