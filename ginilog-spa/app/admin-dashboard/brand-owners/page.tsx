// app/admin-dashboard/brand-owners/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { 
  Plus, Search, Eye, Edit, Trash2, Loader2, Building2, 
  Users, Star, Mail, Phone, MapPin 
} from "lucide-react";
import { getBrandOwners, deleteStaff } from "@/lib/api";

export default function BrandOwnersPage() {
  const [owners, setOwners] = useState<any[]>([]);
  const [filteredOwners, setFilteredOwners] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchOwners();
  }, []);

  useEffect(() => {
    if (search.trim() === "") {
      setFilteredOwners(owners);
    } else {
      const filtered = owners.filter((owner) =>
        owner.firstName?.toLowerCase().includes(search.toLowerCase()) ||
        owner.surName?.toLowerCase().includes(search.toLowerCase()) ||
        owner.companyName?.toLowerCase().includes(search.toLowerCase()) ||
        owner.email?.toLowerCase().includes(search.toLowerCase())
      );
      setFilteredOwners(filtered);
    }
  }, [search, owners]);

  const fetchOwners = async () => {
    try {
      setIsLoading(true);
      const data = await getBrandOwners();
      setOwners(data || []);
      setFilteredOwners(data || []);
    } catch (error) {
      console.error("Failed to fetch brand owners:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this brand owner?")) return;
    setDeletingId(id);
    try {
      await deleteStaff(id);
      await fetchOwners();
    } catch (error) {
      console.error("Failed to delete brand owner:", error);
    } finally {
      setDeletingId(null);
    }
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
          <h1 className="text-2xl font-bold text-gray-900">Brand Owners</h1>
          <p className="text-sm text-gray-500">Manage brand owners and their companies</p>
        </div>
        <Link href="/admin-dashboard/brand-owners/add">
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Register Brand Owner
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Brand Owners ({filteredOwners.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search owners..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredOwners.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              {search ? "No brand owners match your search" : "No brand owners registered"}
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {filteredOwners.map((owner) => (
                <Card key={owner.id} className="hover:shadow-md transition-shadow">
                  <CardContent className="p-4">
                    <div className="flex items-start justify-between">
                      <div className="flex items-center gap-3">
                        <div className="h-12 w-12 rounded-full bg-purple-100 flex items-center justify-center">
                          <span className="text-lg font-semibold text-purple-600">
                            {owner.firstName?.[0]}{owner.surName?.[0]}
                          </span>
                        </div>
                        <div>
                          <h3 className="font-semibold text-gray-900">
                            {owner.firstName} {owner.surName}
                          </h3>
                          <p className="text-xs text-gray-500">{owner.staffType || "Brand Owner"}</p>
                        </div>
                      </div>
                      <Badge className="bg-green-100 text-green-800">Active</Badge>
                    </div>

                    <div className="mt-3 space-y-2">
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Building2 className="h-4 w-4 text-gray-400" />
                        <span>{owner.companyName}</span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Mail className="h-4 w-4 text-gray-400" />
                        <span>{owner.email}</span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Phone className="h-4 w-4 text-gray-400" />
                        <span>{owner.phoneNo || "N/A"}</span>
                      </div>
                    </div>

                    <div className="mt-3 flex flex-wrap gap-1">
                      {owner.companyType?.map((type: string) => (
                        <Badge key={type} variant="secondary" className="text-xs">
                          {type}
                        </Badge>
                      ))}
                    </div>

                    <div className="mt-4 flex gap-2">
                      <Link href={`/admin-dashboard/brand-owners/${owner.id}`} className="flex-1">
                        <Button size="sm" variant="outline" className="w-full">
                          <Eye className="h-3 w-3 mr-1" /> View
                        </Button>
                      </Link>
                      <Link href={`/admin-dashboard/brand-owners/${owner.id}/edit`} className="flex-1">
                        <Button size="sm" variant="outline" className="w-full">
                          <Edit className="h-3 w-3 mr-1" /> Edit
                        </Button>
                      </Link>
                      <Button
                        size="sm"
                        variant="outline"
                        className="px-2 text-red-500 border-red-200 hover:bg-red-50"
                        onClick={() => handleDelete(owner.id)}
                        disabled={deletingId === owner.id}
                      >
                        {deletingId === owner.id ? (
                          <Loader2 className="h-3 w-3 animate-spin" />
                        ) : (
                          <Trash2 className="h-3 w-3" />
                        )}
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}