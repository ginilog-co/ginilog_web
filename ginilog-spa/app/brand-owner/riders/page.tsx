"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ArrowLeft, Bike, Car, CheckCircle, Clock, Loader2, Plus, UserCheck } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { deleteRider, getAllRiders } from "@/lib/api";

export default function BrandOwnerRidersPage() {
  const router = useRouter();
  const [riders, setRiders] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const data = await getAllRiders();
        setRiders(data || []);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  const handleDelete = async (id: string) => {
    if (!confirm("Remove this rider?")) return;
    try {
      await deleteRider(id);
      setRiders((current) => current.filter((item) => item.id !== id));
    } catch (error) {
      console.error("Failed to delete rider:", error);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <Link href="/brand-owner">
            <Button variant="ghost" size="sm" className="gap-2">
              <ArrowLeft className="h-4 w-4" />
              Back
            </Button>
          </Link>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Riders</h1>
            <p className="text-sm text-gray-500">All active logistics riders</p>
          </div>
        </div>

        <Button onClick={() => router.push("/brand-owner/riders/add")} className="gap-2">
          <Plus className="h-4 w-4" />
          Add Rider
        </Button>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Total Riders</p>
            <p className="mt-2 text-2xl font-bold">{riders.length}</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">Available</p>
            <p className="mt-2 text-2xl font-bold text-green-600">{riders.filter((r) => (r.status || "").toLowerCase() === "available").length}</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <p className="text-sm text-gray-500">On Delivery</p>
            <p className="mt-2 text-2xl font-bold text-blue-600">{riders.filter((r) => (r.status || "").toLowerCase() === "on delivery").length}</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Rider management</CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {riders.length === 0 ? (
            <div className="p-8 text-center text-sm text-gray-500">No riders found yet.</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-left">Name</th>
                    <th className="px-4 py-3 text-left">Vehicle</th>
                    <th className="px-4 py-3 text-left">Status</th>
                    <th className="px-4 py-3 text-left">Phone</th>
                    <th className="px-4 py-3 text-right">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {riders.map((rider) => (
                    <tr key={rider.id} className="border-t">
                      <td className="px-4 py-3">
                        <div className="font-medium">{rider.firstName || "N/A"} {rider.lastName || ""}</div>
                        <div className="text-xs text-gray-500">{rider.email || "No email"}</div>
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-2">
                          {rider.vehicleType?.toLowerCase().includes("bike") ? <Bike className="h-4 w-4" /> : <Car className="h-4 w-4" />}
                          {rider.vehicleType || "N/A"}
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <Badge className={((rider.status || "").toLowerCase() === "available" ? "bg-green-100 text-green-700" : "bg-blue-100 text-blue-700") + " flex w-fit items-center gap-1"}>
                          {((rider.status || "").toLowerCase() === "available" ? <CheckCircle className="h-3 w-3" /> : <Clock className="h-3 w-3" />)}
                          {rider.status || "Available"}
                        </Badge>
                      </td>
                      <td className="px-4 py-3">{rider.phoneNumber || rider.phoneNo || "N/A"}</td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="destructive" size="sm" onClick={() => handleDelete(rider.id)}>
                          Remove
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
