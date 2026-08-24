"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Plus, Search, Eye, Edit, Trash2, Loader2, Megaphone, Calendar, DollarSign } from "lucide-react";
import { getAllAdverts, deleteAdvert } from "@/lib/api";  // Changed from getAdverts to getAllAdverts

export default function AdvertsPage() {
  const [adverts, setAdverts] = useState<any[]>([]);
  const [filteredAdverts, setFilteredAdverts] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchAdverts();
  }, []);

  useEffect(() => {
    if (search.trim() === "") {
      setFilteredAdverts(adverts);
    } else {
      const filtered = adverts.filter((advert) =>
        advert.advertName?.toLowerCase().includes(search.toLowerCase()) ||
        advert.advertType?.toLowerCase().includes(search.toLowerCase()) ||
        advert.advertItemDescription?.toLowerCase().includes(search.toLowerCase())
      );
      setFilteredAdverts(filtered);
    }
  }, [search, adverts]);

  const fetchAdverts = async () => {
    try {
      setIsLoading(true);
      const data = await getAllAdverts();  // Changed from getAdverts to getAllAdverts
      setAdverts(data || []);
      setFilteredAdverts(data || []);
    } catch (error) {
      console.error("Failed to fetch adverts:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this advert?")) return;
    setDeletingId(id);
    try {
      await deleteAdvert(id);
      await fetchAdverts();
    } catch (error) {
      console.error("Failed to delete advert:", error);
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
          <h1 className="text-2xl font-bold text-gray-900">Adverts</h1>
          <p className="text-sm text-gray-500">Manage promotional content</p>
        </div>
        <Link href="/admin-dashboard/adverts/add">
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Create Advert
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Adverts ({filteredAdverts.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search adverts..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredAdverts.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              {search ? "No adverts match your search" : "No adverts created"}
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {filteredAdverts.map((advert) => (
                <Card key={advert.id} className="overflow-hidden hover:shadow-md transition-shadow">
                  <div className="relative h-48 bg-gray-200">
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
                    <Badge className="absolute top-3 right-3 bg-white/90 text-gray-700">
                      {advert.advertType}
                    </Badge>
                  </div>
                  <CardContent className="p-4">
                    <h3 className="font-semibold text-gray-900">{advert.advertName}</h3>
                    <p className="text-sm text-gray-500 line-clamp-2 mt-1">
                      {advert.advertItemDescription}
                    </p>
                    <div className="flex items-center justify-between mt-3">
                      <div className="flex items-center gap-1 text-sm font-medium text-primary">
                        <DollarSign className="h-4 w-4" />
                        ₦{advert.advertItemCost?.toLocaleString() || 0}
                      </div>
                      <div className="flex items-center gap-1 text-sm text-gray-500">
                        <Calendar className="h-4 w-4" />
                        {advert.advertDays4} days
                      </div>
                    </div>
                    <div className="flex gap-2 mt-4">
                      <Link href={`/admin-dashboard/adverts/${advert.id}`} className="flex-1">
                        <Button size="sm" variant="outline" className="w-full">
                          <Eye className="h-3 w-3 mr-1" /> View
                        </Button>
                      </Link>
                      <Link href={`/admin-dashboard/adverts/${advert.id}/edit`} className="flex-1">
                        <Button size="sm" variant="outline" className="w-full">
                          <Edit className="h-3 w-3 mr-1" /> Edit
                        </Button>
                      </Link>
                      <Button
                        size="sm"
                        variant="outline"
                        className="px-2 text-red-500 border-red-200 hover:bg-red-50"
                        onClick={() => handleDelete(advert.id)}
                        disabled={deletingId === advert.id}
                      >
                        {deletingId === advert.id ? (
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