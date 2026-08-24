// app/brand-owner/staff/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
  Users,
  Mail,
  Phone,
  UserCheck,
  UserX,
  MoreVertical,
  ArrowRight
} from "lucide-react";
import { getStaff, deleteStaff } from "@/lib/api";

export default function StaffPage() {
  const router = useRouter();
  const [staff, setStaff] = useState<any[]>([]);
  const [filteredStaff, setFilteredStaff] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchStaff();
  }, []);

  useEffect(() => {
    if (search.trim() === "") {
      setFilteredStaff(staff);
    } else {
      const filtered = staff.filter((s) =>
        s.firstName?.toLowerCase().includes(search.toLowerCase()) ||
        s.surName?.toLowerCase().includes(search.toLowerCase()) ||
        s.email?.toLowerCase().includes(search.toLowerCase()) ||
        s.companyName?.toLowerCase().includes(search.toLowerCase())
      );
      setFilteredStaff(filtered);
    }
  }, [search, staff]);

  const fetchStaff = async () => {
    try {
      setIsLoading(true);
      const data = await getStaff();
      setStaff(data || []);
      setFilteredStaff(data || []);
    } catch (error) {
      console.error("Failed to fetch staff:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this staff member?")) return;
    setDeletingId(id);
    try {
      await deleteStaff(id);
      await fetchStaff();
    } catch (error) {
      console.error("Failed to delete staff:", error);
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
          <h1 className="text-2xl font-bold text-gray-900">Staff</h1>
          <p className="text-sm text-gray-500">Manage your team members</p>
        </div>
        <Button onClick={() => router.push("/brand-owner/staff/add")} className="gap-2">
          <Plus className="h-4 w-4" />
          Add Staff
        </Button>
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
        <Input
          placeholder="Search staff..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="pl-9"
        />
      </div>

      {filteredStaff.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <Users className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No staff found</p>
            <Button className="mt-4" onClick={() => router.push("/brand-owner/staff/add")}>
              <Plus className="h-4 w-4 mr-2" />
              Add Your First Staff
            </Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filteredStaff.map((member) => (
            <Card key={member.id} className="hover:shadow-md transition-shadow">
              <CardContent className="p-4">
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-full bg-blue-100 flex items-center justify-center">
                      <span className="text-lg font-semibold text-blue-600">
                        {member.firstName?.[0]}{member.surName?.[0]}
                      </span>
                    </div>
                    <div>
                      <h3 className="font-semibold text-gray-900">
                        {member.firstName} {member.surName}
                      </h3>
                      <p className="text-xs text-gray-500">{member.staffType || "Staff"}</p>
                    </div>
                  </div>
                  <Badge className="bg-green-100 text-green-800">Active</Badge>
                </div>

                <div className="mt-3 space-y-2 text-sm">
                  <div className="flex items-center gap-2 text-gray-600">
                    <Mail className="h-3 w-3 text-gray-400" />
                    <span>{member.email}</span>
                  </div>
                  <div className="flex items-center gap-2 text-gray-600">
                    <Phone className="h-3 w-3 text-gray-400" />
                    <span>{member.phoneNo || "N/A"}</span>
                  </div>
                </div>

                <div className="mt-4 flex gap-2">
                  <Link href={`/brand-owner/staff/${member.id}`} className="flex-1">
                    <Button size="sm" variant="outline" className="w-full">
                      <Eye className="h-3 w-3 mr-1" /> View
                    </Button>
                  </Link>
                  <Link href={`/brand-owner/staff/${member.id}/edit`} className="flex-1">
                    <Button size="sm" variant="outline" className="w-full">
                      <Edit className="h-3 w-3 mr-1" /> Edit
                    </Button>
                  </Link>
                  <Button
                    size="sm"
                    variant="outline"
                    className="px-2 text-red-500 border-red-200 hover:bg-red-50"
                    onClick={() => handleDelete(member.id)}
                    disabled={deletingId === member.id}
                  >
                    {deletingId === member.id ? (
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