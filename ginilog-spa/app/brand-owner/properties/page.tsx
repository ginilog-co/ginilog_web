// app/brand-owner/properties/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import {
  Plus,
  Search,
  Eye,
  Edit,
  Trash2,
  Loader2,
  Hotel,
  MapPin,
  Star,
  DollarSign,
  Building2,
  Filter,
  ChevronDown,
  ArrowRight
} from "lucide-react";
import { getAccommodations, deleteAccommodation } from "@/lib/api";

export default function PropertiesPage() {
  const router = useRouter();
  const [properties, setProperties] = useState<any[]>([]);
  const [filteredProperties, setFilteredProperties] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchProperties();
  }, []);

  useEffect(() => {
    let filtered = properties;
    
    if (search.trim()) {
      filtered = filtered.filter((p) =>
        p.accomodationName?.toLowerCase().includes(search.toLowerCase()) ||
        p.location?.toLowerCase().includes(search.toLowerCase()) ||
        p.accomodationType?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter === "available") {
      filtered = filtered.filter((p) => p.available);
    } else if (filter === "booked") {
      filtered = filtered.filter((p) => !p.available);
    }
    
    setFilteredProperties(filtered);
  }, [search, filter, properties]);

  const fetchProperties = async () => {
    try {
      setIsLoading(true);
      const data = await getAccommodations();
      setProperties(data || []);
      setFilteredProperties(data || []);
    } catch (error) {
      console.error("Failed to fetch properties:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this property?")) return;
    setDeletingId(id);
    try {
      await deleteAccommodation(id);
      await fetchProperties();
    } catch (error) {
      console.error("Failed to delete property:", error);
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
          <h1 className="text-2xl font-bold text-gray-900">Properties</h1>
          <p className="text-sm text-gray-500">Manage your accommodations and properties</p>
        </div>
        <Button onClick={() => router.push("/brand-owner/properties/add")} className="gap-2">
          <Plus className="h-4 w-4" />
          Add Property
        </Button>
      </div>

      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            placeholder="Search properties..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-9"
          />
        </div>
        <select
          className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        >
          <option value="all">All Status</option>
          <option value="available">Available</option>
          <option value="booked">Booked</option>
        </select>
      </div>

      {filteredProperties.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Hotel className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No properties found</p>
            <Button className="mt-4" onClick={() => router.push("/brand-owner/properties/add")}>
              <Plus className="h-4 w-4 mr-2" />
              Add Your First Property
            </Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filteredProperties.map((property) => (
            <Card key={property.id} className="overflow-hidden hover:shadow-md transition-shadow">
              <div className="relative h-48 bg-gray-200">
                {property.accomodationImages?.[0] ? (
                  <img
                    src={property.accomodationImages[0]}
                    alt={property.accomodationName}
                    className="w-full h-full object-cover"
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = "/placeholder-property.jpg";
                    }}
                  />
                ) : (
                  <div className="flex items-center justify-center h-full bg-gray-100">
                    <Hotel className="h-12 w-12 text-gray-400" />
                  </div>
                )}
                <Badge
                  className={`absolute top-3 right-3 ${property.available ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}
                >
                  {property.available ? 'Available' : 'Booked'}
                </Badge>
              </div>
              <CardContent className="p-4">
                <div className="flex items-start justify-between">
                  <div>
                    <h3 className="font-semibold text-gray-900">{property.accomodationName}</h3>
                    <p className="text-sm text-gray-500">{property.accomodationType}</p>
                  </div>
                  <div className="flex items-center gap-1">
                    <Star className="h-4 w-4 text-yellow-500 fill-yellow-500" />
                    <span className="text-sm font-medium">{property.rating || '4.5'}</span>
                  </div>
                </div>
                <div className="flex items-center gap-2 text-sm text-gray-500 mt-2">
                  <MapPin className="h-3 w-3" />
                  {property.location}
                </div>
                <p className="text-lg font-bold text-primary mt-2">
                  ₦{property.bookingAmount?.toLocaleString() || 0}
                  <span className="text-sm font-normal text-gray-500">/night</span>
                </p>
                <div className="flex gap-2 mt-4">
                  <Link href={`/brand-owner/properties/${property.id}`} className="flex-1">
                    <Button size="sm" variant="outline" className="w-full">
                      <Eye className="h-3 w-3 mr-1" /> View
                    </Button>
                  </Link>
                  <Link href={`/brand-owner/properties/${property.id}/edit`} className="flex-1">
                    <Button size="sm" variant="outline" className="w-full">
                      <Edit className="h-3 w-3 mr-1" /> Edit
                    </Button>
                  </Link>
                  <Button
                    size="sm"
                    variant="outline"
                    className="px-2 text-red-500 border-red-200 hover:bg-red-50"
                    onClick={() => handleDelete(property.id)}
                    disabled={deletingId === property.id}
                  >
                    {deletingId === property.id ? (
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
    </div>
  );
}