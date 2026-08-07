// app/admin-dashboard/manager-approvals/page.tsx

"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Users,
  CheckCircle,
  XCircle,
  Loader2,
  Search,
  Eye,
  Mail,
  Phone,
  Calendar,
  Building2,
  MapPin,
  UserCheck,
  UserX,
  Clock,
  AlertCircle,
  FileText,
  Shield,
  ChevronDown,
  MoreVertical,
  RefreshCw,
} from "lucide-react";
import { getAllStaff, updateStaffStatus } from "@/lib/api";

interface ManagerApplication {
  id: string;
  firstName: string;
  surName: string;
  email: string;
  phoneNo: string;
  adminType: string;
  staffCode: string;
  companyName: string;
  branch: string;
  state: string;
  locality: string;
  address: string;
  sex: string;
  userStatus: boolean;
  createdAt: string;
  updatedAt: string;
  profilePicture?: string;
  rejectionReason?: string;
}

export default function ManagerApprovals() {
  const [applications, setApplications] = useState<ManagerApplication[]>([]);
  const [filteredApplications, setFilteredApplications] = useState<ManagerApplication[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedStatus, setSelectedStatus] = useState<string>("all");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [selectedApplication, setSelectedApplication] = useState<ManagerApplication | null>(null);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [stats, setStats] = useState({
    total: 0,
    pending: 0,
    approved: 0,
    rejected: 0,
  });

  useEffect(() => {
    fetchApplications();
  }, []);

  const fetchApplications = async () => {
    try {
      setIsLoading(true);
      setError(null);

      const allStaff = await getAllStaff();
      
      const managers = allStaff.filter(
        (staff: any) => staff.adminType === "Manager" || staff.adminType === "manager"
      );

      setApplications(managers);
      filterApplications(managers, searchTerm, selectedStatus);

      setStats({
        total: managers.length,
        pending: managers.filter((m: any) => m.userStatus === false).length,
        approved: managers.filter((m: any) => m.userStatus === true).length,
        rejected: managers.filter((m: any) => m.userStatus === false && m.rejectionReason).length,
      });
    } catch (err) {
      console.error("Failed to fetch manager applications:", err);
      setError("Failed to load manager applications. Please refresh.");
    } finally {
      setIsLoading(false);
    }
  };

  const filterApplications = (
    data: ManagerApplication[],
    search: string,
    status: string
  ) => {
    let filtered = data;

    if (status !== "all") {
      if (status === "pending") {
        filtered = filtered.filter((m) => m.userStatus === false);
      } else if (status === "approved") {
        filtered = filtered.filter((m) => m.userStatus === true);
      } else if (status === "rejected") {
        filtered = filtered.filter(
          (m) => m.userStatus === false && m.rejectionReason
        );
      }
    }

    if (search) {
      const lower = search.toLowerCase();
      filtered = filtered.filter(
        (m) =>
          m.firstName.toLowerCase().includes(lower) ||
          m.surName.toLowerCase().includes(lower) ||
          m.email.toLowerCase().includes(lower) ||
          (m.companyName || "").toLowerCase().includes(lower) ||
          (m.staffCode || "").toLowerCase().includes(lower)
      );
    }

    setFilteredApplications(filtered);
  };

  const handleSearch = (term: string) => {
    setSearchTerm(term);
    filterApplications(applications, term, selectedStatus);
  };

  const handleStatusFilter = (status: string) => {
    setSelectedStatus(status);
    filterApplications(applications, searchTerm, status);
  };

  const handleApprove = async (id: string) => {
    setUpdatingId(id);
    try {
      await updateStaffStatus(id, { UserStatus: true });
      const updated = applications.map((app) =>
        app.id === id ? { ...app, userStatus: true } : app
      );
      setApplications(updated);
      filterApplications(updated, searchTerm, selectedStatus);
      
      setStats((prev) => ({
        ...prev,
        pending: prev.pending - 1,
        approved: prev.approved + 1,
      }));
    } catch (err) {
      console.error("Failed to approve manager:", err);
      setError("Failed to approve manager. Please try again.");
    } finally {
      setUpdatingId(null);
    }
  };

  const handleReject = async (id: string) => {
    setUpdatingId(id);
    try {
      await updateStaffStatus(id, { UserStatus: false, RejectionReason: "Application rejected" });
      const updated = applications.map((app) =>
        app.id === id ? { ...app, userStatus: false, rejectionReason: "Application rejected" } : app
      );
      setApplications(updated);
      filterApplications(updated, searchTerm, selectedStatus);
      
      setStats((prev) => ({
        ...prev,
        pending: prev.pending - 1,
        rejected: prev.rejected + 1,
      }));
    } catch (err) {
      console.error("Failed to reject manager:", err);
      setError("Failed to reject manager. Please try again.");
    } finally {
      setUpdatingId(null);
    }
  };

  const viewDetails = (application: ManagerApplication) => {
    setSelectedApplication(application);
    setShowDetailsModal(true);
  };

  const getStatusBadge = (status: boolean) => {
    if (status === true) {
      return <Badge className="bg-green-100 text-green-800">Approved</Badge>;
    }
    return <Badge className="bg-yellow-100 text-yellow-800">Pending</Badge>;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Manager Approvals</h2>
          <p className="text-gray-500 text-sm">Review and approve manager applications</p>
        </div>
        <Button onClick={fetchApplications} variant="outline" className="gap-2">
          <RefreshCw className="h-4 w-4" />
          Refresh
        </Button>
      </div>

      {/* Error */}
      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
          <AlertCircle className="h-4 w-4" />
          <span>{error}</span>
          <button onClick={() => setError(null)} className="ml-auto text-red-400 hover:text-red-600">
            <XCircle className="h-4 w-4" />
          </button>
        </div>
      )}

      {/* Stats Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Applications</p>
                <p className="text-2xl font-bold">{stats.total}</p>
              </div>
              <Users className="h-8 w-8 text-blue-500" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Pending</p>
                <p className="text-2xl font-bold text-yellow-600">{stats.pending}</p>
              </div>
              <Clock className="h-8 w-8 text-yellow-500" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Approved</p>
                <p className="text-2xl font-bold text-green-600">{stats.approved}</p>
              </div>
              <UserCheck className="h-8 w-8 text-green-500" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Rejected</p>
                <p className="text-2xl font-bold text-red-600">{stats.rejected}</p>
              </div>
              <UserX className="h-8 w-8 text-red-500" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Filters */}
      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <input
            type="text"
            placeholder="Search by name, email, company, or staff code..."
            value={searchTerm}
            onChange={(e) => handleSearch(e.target.value)}
            className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
          />
        </div>
        <div className="flex gap-2">
          {["all", "pending", "approved", "rejected"].map((status) => (
            <button
              key={status}
              onClick={() => handleStatusFilter(status)}
              className={`px-4 py-2 rounded-lg text-sm font-medium capitalize transition-colors ${
                selectedStatus === status
                  ? "bg-primary text-white"
                  : "bg-gray-100 text-gray-600 hover:bg-gray-200"
              }`}
            >
              {status}
            </button>
          ))}
        </div>
      </div>

      {/* Applications List */}
      <Card>
        <CardContent className="p-0">
          {filteredApplications.length === 0 ? (
            <div className="text-center py-12">
              <Users className="h-12 w-12 text-gray-300 mx-auto mb-4" />
              <p className="text-gray-500">No manager applications found</p>
            </div>
          ) : (
            <div className="divide-y">
              {filteredApplications.map((app) => (
                <div key={app.id} className="p-4 hover:bg-gray-50 transition-colors">
                  <div className="flex flex-col md:flex-row md:items-center gap-4">
                    {/* Avatar */}
                    <div className="h-12 w-12 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                      {app.profilePicture ? (
                        <img
                          src={app.profilePicture}
                          alt={app.firstName}
                          className="h-full w-full rounded-full object-cover"
                        />
                      ) : (
                        <span className="text-primary font-bold text-lg">
                          {app.firstName?.[0] || ""}{app.surName?.[0] || ""}
                        </span>
                      )}
                    </div>

                    {/* Info */}
                    <div className="flex-1 min-w-0">
                      <div className="flex flex-col sm:flex-row sm:items-center gap-2">
                        <h3 className="font-medium text-gray-900">
                          {app.firstName} {app.surName}
                        </h3>
                        {getStatusBadge(app.userStatus)}
                      </div>
                      <div className="flex flex-wrap gap-x-4 gap-y-1 text-sm text-gray-500 mt-1">
                        <span className="flex items-center gap-1">
                          <Mail className="h-3.5 w-3.5" />
                          {app.email}
                        </span>
                        <span className="flex items-center gap-1">
                          <Phone className="h-3.5 w-3.5" />
                          {app.phoneNo}
                        </span>
                        <span className="flex items-center gap-1">
                          <Building2 className="h-3.5 w-3.5" />
                          {app.companyName || "N/A"}
                        </span>
                        <span className="flex items-center gap-1">
                          <Shield className="h-3.5 w-3.5" />
                          Staff Code: {app.staffCode}
                        </span>
                      </div>
                    </div>

                    {/* Actions */}
                    <div className="flex items-center gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => viewDetails(app)}
                        className="gap-1"
                      >
                        <Eye className="h-4 w-4" />
                        View
                      </Button>
                      {app.userStatus === false && (
                        <>
                          <Button
                            size="sm"
                            className="bg-green-600 hover:bg-green-700 text-white gap-1"
                            onClick={() => handleApprove(app.id)}
                            disabled={updatingId === app.id}
                          >
                            {updatingId === app.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <CheckCircle className="h-4 w-4" />
                            )}
                            Approve
                          </Button>
                          <Button
                            size="sm"
                            variant="destructive"
                            className="gap-1"
                            onClick={() => handleReject(app.id)}
                            disabled={updatingId === app.id}
                          >
                            {updatingId === app.id ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <XCircle className="h-4 w-4" />
                            )}
                            Reject
                          </Button>
                        </>
                      )}
                      {app.userStatus === true && (
                        <Badge className="bg-green-100 text-green-800">Active</Badge>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Details Modal */}
      {showDetailsModal && selectedApplication && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold">Application Details</h3>
              <button
                onClick={() => setShowDetailsModal(false)}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <XCircle className="h-5 w-5 text-gray-500" />
              </button>
            </div>

            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-500">First Name</p>
                  <p className="font-medium">{selectedApplication.firstName}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Surname</p>
                  <p className="font-medium">{selectedApplication.surName}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Email</p>
                  <p className="font-medium">{selectedApplication.email}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Phone</p>
                  <p className="font-medium">{selectedApplication.phoneNo}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Gender</p>
                  <p className="font-medium">{selectedApplication.sex || "N/A"}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-500">Staff Code</p>
                  <p className="font-medium font-mono">{selectedApplication.staffCode}</p>
                </div>
              </div>

              <div className="border-t pt-4">
                <h4 className="font-medium mb-2">Company Information</h4>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-gray-500">Company Name</p>
                    <p className="font-medium">{selectedApplication.companyName || "N/A"}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Branch</p>
                    <p className="font-medium">{selectedApplication.branch || "N/A"}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">State</p>
                    <p className="font-medium">{selectedApplication.state || "N/A"}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Locality</p>
                    <p className="font-medium">{selectedApplication.locality || "N/A"}</p>
                  </div>
                </div>
                <div className="mt-2">
                  <p className="text-sm text-gray-500">Address</p>
                  <p className="font-medium">{selectedApplication.address || "N/A"}</p>
                </div>
              </div>

              <div className="border-t pt-4 flex flex-wrap gap-4">
                <div>
                  <p className="text-sm text-gray-500">Status</p>
                  {getStatusBadge(selectedApplication.userStatus)}
                </div>
                <div>
                  <p className="text-sm text-gray-500">Applied On</p>
                  <p className="font-medium">
                    {new Date(selectedApplication.createdAt).toLocaleString()}
                  </p>
                </div>
                {selectedApplication.updatedAt && (
                  <div>
                    <p className="text-sm text-gray-500">Last Updated</p>
                    <p className="font-medium">
                      {new Date(selectedApplication.updatedAt).toLocaleString()}
                    </p>
                  </div>
                )}
              </div>

              <div className="border-t pt-4 flex gap-3">
                {selectedApplication.userStatus === false && (
                  <>
                    <Button
                      className="bg-green-600 hover:bg-green-700 text-white flex-1 gap-2"
                      onClick={() => {
                        handleApprove(selectedApplication.id);
                        setShowDetailsModal(false);
                      }}
                      disabled={updatingId === selectedApplication.id}
                    >
                      {updatingId === selectedApplication.id ? (
                        <Loader2 className="h-4 w-4 animate-spin" />
                      ) : (
                        <CheckCircle className="h-4 w-4" />
                      )}
                      Approve
                    </Button>
                    <Button
                      variant="destructive"
                      className="flex-1 gap-2"
                      onClick={() => {
                        handleReject(selectedApplication.id);
                        setShowDetailsModal(false);
                      }}
                      disabled={updatingId === selectedApplication.id}
                    >
                      {updatingId === selectedApplication.id ? (
                        <Loader2 className="h-4 w-4 animate-spin" />
                      ) : (
                        <XCircle className="h-4 w-4" />
                      )}
                      Reject
                    </Button>
                  </>
                )}
                <Button
                  variant="outline"
                  onClick={() => setShowDetailsModal(false)}
                  className="flex-1"
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}