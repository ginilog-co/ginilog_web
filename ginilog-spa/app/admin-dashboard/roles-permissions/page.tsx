// app/admin-dashboard/roles-permissions/page.tsx
"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { 
  Search, Loader2, Shield, UserCog, Plus, 
  Edit, Trash2, Check, X, Key, Users,
  Eye, EyeOff, Lock, Unlock, Save
} from "lucide-react";
import { getRoles, createRole, updateRole, deleteRole } from "@/lib/api";

export default function RolesPermissionsPage() {
  const [roles, setRoles] = useState<any[]>([]);
  const [filteredRoles, setFilteredRoles] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [showAddForm, setShowAddForm] = useState(false);
  const [editingRole, setEditingRole] = useState<any>(null);
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    permissions: [] as string[],
  });

  const availablePermissions = [
    // Admin Permissions
    { id: "CanViewAdmin", label: "View Admins" },
    { id: "CanCreateAdmin", label: "Create Admins" },
    { id: "CanUpdateAdmin", label: "Update Admins" },
    { id: "CanDeleteAdmin", label: "Delete Admins" },
    // User Permissions
    { id: "CanViewUsers", label: "View Users" },
    { id: "CanManageUsers", label: "Manage Users" },
    { id: "CanDeleteUsers", label: "Delete Users" },
    // Staff Permissions
    { id: "CanViewStaff", label: "View Staff" },
    { id: "CanCreateStaff", label: "Create Staff" },
    { id: "CanManageStaff", label: "Manage Staff" },
    { id: "CanDeleteStaff", label: "Delete Staff" },
    // Brand Permissions
    { id: "CanViewBrands", label: "View Brands" },
    { id: "CanManageBrands", label: "Manage Brands" },
    { id: "CanDeleteBrands", label: "Delete Brands" },
    // Order Permissions
    { id: "CanViewOrders", label: "View Orders" },
    { id: "CanManageOrders", label: "Manage Orders" },
    { id: "CanDeleteOrders", label: "Delete Orders" },
    // Booking Permissions
    { id: "CanViewBookings", label: "View Bookings" },
    { id: "CanManageBookings", label: "Manage Bookings" },
    { id: "CanDeleteBookings", label: "Delete Bookings" },
    // Wallet Permissions
    { id: "CanViewWallet", label: "View Wallet" },
    { id: "CanManageWallet", label: "Manage Wallet" },
  ];

  useEffect(() => {
    fetchRoles();
  }, []);

  useEffect(() => {
    if (search.trim() === "") {
      setFilteredRoles(roles);
    } else {
      const filtered = roles.filter((role) =>
        role.name?.toLowerCase().includes(search.toLowerCase()) ||
        role.description?.toLowerCase().includes(search.toLowerCase())
      );
      setFilteredRoles(filtered);
    }
  }, [search, roles]);

  const fetchRoles = async () => {
    try {
      setIsLoading(true);
      const data = await getRoles();
      setRoles(data || []);
      setFilteredRoles(data || []);
    } catch (error) {
      console.error("Failed to fetch roles:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingRole) {
        await updateRole(editingRole.id, formData);
      } else {
        await createRole(formData);
      }
      setShowAddForm(false);
      setEditingRole(null);
      setFormData({ name: "", description: "", permissions: [] });
      await fetchRoles();
    } catch (error) {
      console.error("Failed to save role:", error);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this role?")) return;
    try {
      await deleteRole(id);
      await fetchRoles();
    } catch (error) {
      console.error("Failed to delete role:", error);
    }
  };

  const togglePermission = (permissionId: string) => {
    setFormData((prev) => ({
      ...prev,
      permissions: prev.permissions.includes(permissionId)
        ? prev.permissions.filter((p) => p !== permissionId)
        : [...prev.permissions, permissionId],
    }));
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
          <h1 className="text-2xl font-bold text-gray-900">Roles & Permissions</h1>
          <p className="text-sm text-gray-500">Manage user roles and their permissions</p>
        </div>
        <Button className="gap-2" onClick={() => {
          setShowAddForm(true);
          setEditingRole(null);
          setFormData({ name: "", description: "", permissions: [] });
        }}>
          <Plus className="h-4 w-4" />
          Create Role
        </Button>
      </div>

      {/* Add/Edit Form */}
      {(showAddForm || editingRole) && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Shield className="h-5 w-5 text-primary" />
              {editingRole ? "Edit Role" : "Create New Role"}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="text-sm font-medium text-gray-700">Role Name *</label>
                  <Input
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    placeholder="e.g., Manager"
                    required
                  />
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-700">Description</label>
                  <Input
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    placeholder="Role description"
                  />
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-gray-700 mb-2 block">Permissions</label>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                  {availablePermissions.map((perm) => (
                    <label
                      key={perm.id}
                      className="flex items-center gap-2 p-2 rounded-lg border hover:bg-gray-50 cursor-pointer"
                    >
                      <input
                        type="checkbox"
                        checked={formData.permissions.includes(perm.id)}
                        onChange={() => togglePermission(perm.id)}
                        className="rounded border-gray-300 text-primary focus:ring-primary"
                      />
                      <span className="text-sm">{perm.label}</span>
                    </label>
                  ))}
                </div>
              </div>

              <div className="flex gap-3">
                <Button type="submit">
                  <Save className="h-4 w-4 mr-2" />
                  {editingRole ? "Update Role" : "Create Role"}
                </Button>
                <Button 
                  type="button" 
                  variant="outline" 
                  onClick={() => {
                    setShowAddForm(false);
                    setEditingRole(null);
                  }}
                >
                  Cancel
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {/* Roles List */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Roles ({filteredRoles.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search roles..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredRoles.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              <Shield className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>{search ? "No roles match your search" : "No roles created yet"}</p>
              <Button className="mt-4" onClick={() => {
                setShowAddForm(true);
                setEditingRole(null);
                setFormData({ name: "", description: "", permissions: [] });
              }}>
                <Plus className="h-4 w-4 mr-2" />
                Create First Role
              </Button>
            </div>
          ) : (
            <div className="space-y-4">
              {filteredRoles.map((role) => (
                <div key={role.id} className="p-4 border rounded-xl hover:shadow-sm transition-shadow">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3">
                        <Shield className="h-5 w-5 text-primary" />
                        <div>
                          <h3 className="font-semibold text-gray-900">{role.name}</h3>
                          {role.description && (
                            <p className="text-sm text-gray-500">{role.description}</p>
                          )}
                        </div>
                      </div>
                      <div className="mt-2 flex flex-wrap gap-1">
                        {role.permissions?.slice(0, 6).map((perm: string) => (
                          <Badge key={perm} variant="secondary" className="text-xs">
                            {perm}
                          </Badge>
                        ))}
                        {(role.permissions?.length || 0) > 6 && (
                          <Badge variant="secondary" className="text-xs">
                            +{role.permissions.length - 6} more
                          </Badge>
                        )}
                      </div>
                    </div>
                    <div className="flex gap-2">
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => {
                          setEditingRole(role);
                          setShowAddForm(true);
                          setFormData({
                            name: role.name,
                            description: role.description || "",
                            permissions: role.permissions || [],
                          });
                        }}
                      >
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-red-500 hover:text-red-700"
                        onClick={() => handleDelete(role.id)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                  <div className="mt-2 flex items-center gap-4 text-xs text-gray-400">
                    <span>Created: {new Date(role.createdAt).toLocaleDateString()}</span>
                    <span>•</span>
                    <span>{role.permissions?.length || 0} permissions</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}