// app/admin-dashboard/admins/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
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
  UserPlus,
  MoreVertical,
  Shield,
  UserCog
} from "lucide-react";
import { getAllAdmins, deleteAdmin } from "@/lib/api";

export default function AdminsPage() {
  const [admins, setAdmins] = useState<any[]>([]);
  const [filteredAdmins, setFilteredAdmins] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    fetchAdmins();
  }, []);

  useEffect(() => {
    if (search.trim() === "") {
      setFilteredAdmins(admins);
    } else {
      const filtered = admins.filter((admin) =>
        admin.firstName?.toLowerCase().includes(search.toLowerCase()) ||
        admin.surName?.toLowerCase().includes(search.toLowerCase()) ||
        admin.email?.toLowerCase().includes(search.toLowerCase())
      );
      setFilteredAdmins(filtered);
    }
  }, [search, admins]);

  const fetchAdmins = async () => {
    try {
      setIsLoading(true);
      const data = await getAllAdmins();
      setAdmins(data || []);
      setFilteredAdmins(data || []);
    } catch (error) {
      console.error("Failed to fetch admins:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this admin?")) return;
    setDeletingId(id);
    try {
      await deleteAdmin(id);
      await fetchAdmins();
    } catch (error) {
      console.error("Failed to delete admin:", error);
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
          <h1 className="text-2xl font-bold text-gray-900">Admins</h1>
          <p className="text-sm text-gray-500">Manage platform administrators</p>
        </div>
        <Link href="/admin-dashboard/admins/add">
          <Button className="gap-2">
            <UserPlus className="h-4 w-4" />
            Add Admin
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Admins ({filteredAdmins.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search admins..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredAdmins.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              {search ? "No admins match your search" : "No admins found"}
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-gray-50">
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Admin</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Email</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Phone</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Role</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                    <th className="text-right py-3 px-4 font-medium text-gray-600">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredAdmins.map((admin) => (
                    <tr key={admin.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-3">
                          <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center">
                            <span className="text-xs font-medium text-primary">
                              {admin.firstName?.[0]}{admin.surName?.[0]}
                            </span>
                          </div>
                          <div>
                            <p className="font-medium">{admin.firstName} {admin.surName}</p>
                            <p className="text-xs text-gray-500">{admin.adminType || "Admin"}</p>
                          </div>
                        </div>
                      </td>
                      <td className="py-3 px-4 text-gray-600">{admin.email}</td>
                      <td className="py-3 px-4">{admin.phoneNo || "—"}</td>
                      <td className="py-3 px-4">
                        <Badge className="bg-blue-100 text-blue-800">
                          {admin.roles?.[0] || "Admin"}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <Badge className="bg-green-100 text-green-800">Active</Badge>
                      </td>
                      <td className="py-3 px-4 text-right">
                        <div className="flex justify-end gap-2">
                          <Link href={`/admin-dashboard/admins/${admin.id}`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Link href={`/admin-dashboard/admins/${admin.id}/edit`}>
                            <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Button
                            size="sm"
                            variant="ghost"
                            className="h-8 w-8 p-0 text-red-500 hover:text-red-700"
                            onClick={() => handleDelete(admin.id)}
                            disabled={deletingId === admin.id}
                          >
                            {deletingId === admin.id ? (
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