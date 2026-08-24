"use client";

import { useState, useEffect, use } from "react";
import { useRouter } from "next/navigation";
import { getAdminById, getAllAdmins } from "@/lib/api";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  ArrowLeft, Loader2, User, Mail, Phone, MapPin, Shield, 
  Building, AlertCircle, RefreshCw 
} from "lucide-react";

export default function AdminDetailsPage({ params }: { params: Promise<{ id: string }> }) {
  const router = useRouter();
  // ✅ Unwrap the params Promise using React.use()
  const { id } = use(params);
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [admin, setAdmin] = useState<any>(null);

  useEffect(() => {
    // Check authentication first
    const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
    if (!token) {
      router.push('/admin-dashboard/login');
      return;
    }
    
    if (id) {
      fetchAdmin();
    }
  }, [id]);

  const fetchAdmin = async () => {
    setLoading(true);
    setError("");
    
    try {
      console.log(`📤 Fetching admin with ID: ${id}`);
      
      let data = null;
      
      // Method 1: Try getAdminById with the correct profile endpoint
      try {
        console.log("📡 Method 1: Trying getAdminById...");
        data = await getAdminById(id);
        if (data) {
          console.log("✅ Admin found via getAdminById:", data);
        }
      } catch (err) {
        console.warn("Method 1 failed:", err);
      }
      
      // Method 2: Search in the list if method 1 fails
      if (!data) {
        try {
          console.log("📡 Method 2: Searching in admins list...");
          const admins = await getAllAdmins();
          const found = admins.find((a: any) => a.id === id || a.userId === id);
          if (found) {
            console.log("✅ Admin found in list:", found);
            data = found;
          }
        } catch (err) {
          console.warn("Method 2 failed:", err);
        }
      }
      
      // If we found data, normalize and set it
      if (data) {
        const normalizedData = {
          id: data.id || data.userId,
          userId: data.userId || data.id,
          firstName: data.firstName || data.firstname || "",
          lastName: data.lastName || data.surName || "",
          surName: data.surName || data.lastName || "",
          email: data.email || "",
          phoneNo: data.phoneNo || data.phoneNumber || "",
          phoneNumber: data.phoneNumber || data.phoneNo || "",
          sex: data.sex || "",
          staffCode: data.staffCode || "",
          state: data.state || "",
          locality: data.locality || "",
          address: data.address || "",
          branch: data.branch || "",
          adminType: data.adminType || data.userType || data.staffType || "Admin",
          userType: data.userType || data.staffType || "Admin",
          staffType: data.staffType || data.userType || "Admin",
          roles: data.roles || [],
          permissions: data.permissions || [],
          ...data,
        };
        
        setAdmin(normalizedData);
        console.log("✅ Admin data loaded successfully:", normalizedData);
      } else {
        throw new Error("Admin not found with any method");
      }
      
    } catch (err: any) {
      console.error("❌ Error fetching admin:", err);
      setError(err.message || "Failed to load admin details");
    } finally {
      setLoading(false);
    }
  };

  const getDisplayName = () => {
    if (!admin) return "N/A";
    const firstName = admin.firstName || "";
    const lastName = admin.surName || admin.lastName || "";
    if (firstName && lastName) return `${firstName} ${lastName}`;
    if (firstName) return firstName;
    if (lastName) return lastName;
    return "N/A";
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin text-primary mx-auto" />
          <p className="mt-4 text-muted-foreground">Loading admin details...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto py-8 px-4">
        <div className="bg-destructive/10 border border-destructive/20 text-destructive p-8 rounded-lg text-center max-w-lg mx-auto">
          <AlertCircle className="h-12 w-12 mx-auto mb-4" />
          <p className="text-lg font-semibold">Error loading admin</p>
          <p className="mt-2 text-sm">{error}</p>
          <div className="flex gap-4 justify-center mt-6 flex-wrap">
            <Button variant="outline" onClick={() => router.back()}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Go Back
            </Button>
            <Button onClick={fetchAdmin}>
              <RefreshCw className="mr-2 h-4 w-4" />
              Retry
            </Button>
            <Button variant="secondary" onClick={() => router.push("/admin-dashboard/admins")}>
              View All Admins
            </Button>
          </div>
        </div>
      </div>
    );
  }

  if (!admin) {
    return (
      <div className="container mx-auto py-8 px-4">
        <div className="text-center max-w-lg mx-auto">
          <User className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
          <p className="text-lg font-semibold">Admin not found</p>
          <p className="text-muted-foreground text-sm mt-2">
            The admin with ID {id} could not be found
          </p>
          <div className="flex gap-4 justify-center mt-6 flex-wrap">
            <Button variant="outline" onClick={() => router.back()}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Go Back
            </Button>
            <Button variant="secondary" onClick={() => router.push("/admin-dashboard/admins")}>
              View All Admins
            </Button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto py-8 px-4 max-w-5xl">
      <div className="mb-6 flex items-center justify-between flex-wrap gap-4">
        <Button variant="ghost" onClick={() => router.push("/admin-dashboard/admins")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to Admins
        </Button>
        <div className="flex gap-2 flex-wrap">
          <Button variant="outline" onClick={fetchAdmin}>
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push(`/admin-dashboard/admins/${id}/edit`)}>
            Edit Admin
          </Button>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-3">
        <Card className="md:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5" />
              Admin Profile
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm text-muted-foreground">Full Name</label>
                <p className="font-medium text-lg">{getDisplayName()}</p>
              </div>
              <div>
                <label className="text-sm text-muted-foreground">Staff Code</label>
                <p className="font-medium">{admin.staffCode || "N/A"}</p>
              </div>
            </div>

            <div>
              <label className="text-sm text-muted-foreground flex items-center gap-1">
                <Mail className="h-4 w-4" />
                Email
              </label>
              <p className="font-medium">{admin.email || "N/A"}</p>
            </div>

            <div>
              <label className="text-sm text-muted-foreground flex items-center gap-1">
                <Phone className="h-4 w-4" />
                Phone Number
              </label>
              <p className="font-medium">{admin.phoneNo || admin.phoneNumber || "N/A"}</p>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm text-muted-foreground">Sex</label>
                <p className="font-medium">{admin.sex || "N/A"}</p>
              </div>
              <div>
                <label className="text-sm text-muted-foreground">Admin Type</label>
                <p className="font-medium">{admin.adminType || admin.userType || admin.staffType || "N/A"}</p>
              </div>
            </div>

            <div>
              <label className="text-sm text-muted-foreground flex items-center gap-1">
                <MapPin className="h-4 w-4" />
                Address
              </label>
              <p className="font-medium">{admin.address || "N/A"}</p>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm text-muted-foreground">State</label>
                <p className="font-medium">{admin.state || "N/A"}</p>
              </div>
              <div>
                <label className="text-sm text-muted-foreground">Locality</label>
                <p className="font-medium">{admin.locality || "N/A"}</p>
              </div>
            </div>

            <div>
              <label className="text-sm text-muted-foreground flex items-center gap-1">
                <Building className="h-4 w-4" />
                Branch
              </label>
              <p className="font-medium">{admin.branch || "N/A"}</p>
            </div>
          </CardContent>
        </Card>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Shield className="h-5 w-5" />
                Role & Permissions
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <label className="text-sm text-muted-foreground">Roles</label>
                <div className="flex flex-wrap gap-2 mt-1">
                  {(admin.roles || []).map((role: string, index: number) => (
                    <span key={index} className="bg-primary/10 text-primary px-3 py-1 rounded-full text-sm">
                      {role}
                    </span>
                  ))}
                  {(!admin.roles || admin.roles.length === 0) && (
                    <p className="text-muted-foreground text-sm">No roles assigned</p>
                  )}
                </div>
              </div>
              <div>
                <label className="text-sm text-muted-foreground">Permissions</label>
                <div className="flex flex-wrap gap-1 mt-1 max-h-40 overflow-y-auto">
                  {(admin.permissions || []).map((permission: string, index: number) => (
                    <span key={index} className="bg-secondary text-secondary-foreground px-2 py-1 rounded-md text-xs">
                      {permission}
                    </span>
                  ))}
                  {(!admin.permissions || admin.permissions.length === 0) && (
                    <p className="text-muted-foreground text-sm">No permissions assigned</p>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-sm">User ID</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground break-all">{admin.id || admin.userId || "N/A"}</p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}