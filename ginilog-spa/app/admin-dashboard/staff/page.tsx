// app/admin-dashboard/staff/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Plus, Search, Eye, Edit, Trash2, Loader2, UserPlus, Users } from "lucide-react";
import { getStaff, deleteStaff } from "@/lib/api";

export default function StaffPage() {
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
          <h1 className="text-2xl font-bold text-gray-900">Staff Management</h1>
          <p className="text-sm text-gray-500">Manage all staff members across brands</p>
        </div>
        <Link href="/admin-dashboard/staff/add">
          <Button className="gap-2">
            <UserPlus className="h-4 w-4" />
            Add Staff
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Staff ({filteredStaff.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search staff..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredStaff.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              {search ? "No staff match your search" : "No staff found"}
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-gray-50">
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Staff</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Company</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Email</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Role</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                    <th className="text-right py-3 px-4 font-medium text-gray-600">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredStaff.map((s) => (
                    <tr key={s.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-3">
                          <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                            <span className="text-xs font-medium text-blue-600">
                              {s.firstName?.[0]}{s.surName?.[0]}
                            </span>
                          </div>
                          <div>
                            <p className="font-medium">{s.firstName} {s.surName}</p>
                            <p className="text-xs text-gray-500">{s.staffType || "Staff"}</p>
                          </div>
                        </div>
                      </td>
                      <td className="py-3 px-4">{s.companyName}</td>
                      <td className="py-3 px-4 text-gray-600">{s.email}</td>
                      <td className="py-3 px-4">
                        <Badge className="bg-purple-100 text-purple-800">
                          {s.roles?.[0] || "Staff"}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <Badge className="bg-green-100 text-green-800">Active</Badge>
                      </td>
                      <td className="py-3 px-4 text-right">
                        <div className="flex justify-end gap-2">
                          <Link href={`/admin-dashboard/staff/${s.id}`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Link href={`/admin-dashboard/staff/${s.id}/edit`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Button
                            size="sm"
                            variant="ghost"
                            className="h-8 w-8 p-0 text-red-500 hover:text-red-700"
                            onClick={() => handleDelete(s.id)}
                            disabled={deletingId === s.id}
                          >
                            {deletingId === s.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <Trash2 className="h-4 w-4" />
                            )}
                          </Button>
                        </div>
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