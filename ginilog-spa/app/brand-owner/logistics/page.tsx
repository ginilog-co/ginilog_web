// app/brand-owner/logistics/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Truck,
  Users,
  Package,
  MapPin,
  Plus,
  Loader2,
  Eye,
  Edit,
  Trash2,
  ArrowRight,
  Bike,
  Car,
  Navigation,
  Clock,
  CheckCircle,
  XCircle
} from "lucide-react";
import { getAllRiders, deleteRider } from "@/lib/api";

export default function LogisticsPage() {
  const router = useRouter();
  const [riders, setRiders] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchRiders();
  }, []);

  const fetchRiders = async () => {
    try {
      setIsLoading(true);
      const data = await getAllRiders();
      setRiders(data || []);
    } catch (error) {
      console.error("Failed to fetch riders:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this rider?")) return;
    setDeletingId(id);
    try {
      await deleteRider(id);
      await fetchRiders();
    } catch (error) {
      console.error("Failed to delete rider:", error);
    } finally {
      setDeletingId(null);
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

  const stats = {
    totalRiders: riders.length,
    available: riders.filter(r => r.status?.toLowerCase() === "available").length,
    onDelivery: riders.filter(r => r.status?.toLowerCase() === "on delivery").length,
    offDuty: riders.filter(r => r.status?.toLowerCase() === "off duty").length,
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Logistics</h1>
          <p className="text-sm text-gray-500">Manage your riders and deliveries</p>
        </div>
        <div className="flex gap-2">
          <Button onClick={() => router.push("/brand-owner/logistics/riders/add")} className="gap-2">
            <Plus className="h-4 w-4" />
            Add Rider
          </Button>
          <Button variant="outline" onClick={() => router.push("/brand-owner/logistics/assign")} className="gap-2">
            <Package className="h-4 w-4" />
            Assign Orders
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Riders</p>
                <p className="text-2xl font-bold">{stats.totalRiders}</p>
              </div>
              <Users className="h-8 w-8 text-gray-400" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Available</p>
                <p className="text-2xl font-bold text-green-600">{stats.available}</p>
              </div>
              <CheckCircle className="h-8 w-8 text-green-400" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">On Delivery</p>
                <p className="text-2xl font-bold text-blue-600">{stats.onDelivery}</p>
              </div>
              <Navigation className="h-8 w-8 text-blue-400" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Off Duty</p>
                <p className="text-2xl font-bold text-gray-600">{stats.offDuty}</p>
              </div>
              <Clock className="h-8 w-8 text-gray-400" />
            </div>
          </CardContent>
        </Card>
      </div>

      {riders.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Truck className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No riders found</p>
            <Button className="mt-4" onClick={() => router.push("/brand-owner/logistics/riders/add")}>
              <Plus className="h-4 w-4 mr-2" />
              Add Your First Rider
            </Button>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0 overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-gray-50">
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Rider</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Vehicle</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Rating</th>
                  <th className="text-left py-3 px-4 font-medium text-gray-600">Deliveries</th>
                  <th className="text-right py-3 px-4 font-medium text-gray-600">Actions</th>
                </tr>
              </thead>
              <tbody>
                {riders.map((rider) => {
                  const status = getStatusBadge(rider.status);
                  const StatusIcon = status.icon;
                  return (
                    <tr key={rider.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-3">
                          <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                            <span className="text-xs font-medium text-blue-600">
                              {rider.firstName?.[0]}{rider.lastName?.[0]}
                            </span>
                          </div>
                          <div>
                            <p className="font-medium">{rider.firstName} {rider.lastName}</p>
                            <p className="text-xs text-gray-500">{rider.email}</p>
                          </div>
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-1">
                          {rider.vehicleType?.toLowerCase().includes("bike") ? (
                            <Bike className="h-4 w-4 text-gray-500" />
                          ) : (
                            <Car className="h-4 w-4 text-gray-500" />
                          )}
                          {rider.vehicleType || "N/A"}
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <Badge className={`${status.className} flex items-center gap-1 w-fit`}>
                          <StatusIcon className="h-3 w-3" />
                          {status.label}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-1">
                          <span className="text-yellow-500">★</span>
                          {rider.rating || 0}
                        </div>
                      </td>
                      <td className="py-3 px-4">{rider.deliveries || 0}</td>
                      <td className="py-3 px-4 text-right">
                        <div className="flex justify-end gap-2">
                          <Link href={`/brand-owner/logistics/riders/${rider.id}`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Link href={`/brand-owner/logistics/riders/${rider.id}/edit`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Button
                            size="sm"
                            variant="ghost"
                            className="h-8 w-8 p-0 text-red-500 hover:text-red-700"
                            onClick={() => handleDelete(rider.id)}
                            disabled={deletingId === rider.id}
                          >
                            {deletingId === rider.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <Trash2 className="h-4 w-4" />
                            )}
                          </Button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}
    </div>
  );
}