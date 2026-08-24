// appbrand-owner/login-applications/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { 
  Search, Eye, Loader2, Building2, Mail, Phone, 
  CheckCircle, XCircle, Clock, Filter, MoreVertical,
  ChevronDown, User, MapPin, Briefcase, Calendar,
  X
} from "lucide-react";
import { 
  getCompanyApplications, 
  updateCompanyApplication, 
  deleteCompanyApplication 
} from "@/lib/api";

export default function CompanyApplicationsPage() {
  const [applications, setApplications] = useState<any[]>([]);
  const [filteredApps, setFilteredApps] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [processingId, setProcessingId] = useState<string | null>(null);
  const [selectedApp, setSelectedApp] = useState<any>(null);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [updateNote, setUpdateNote] = useState("");

  useEffect(() => {
    fetchApplications();
  }, []);

  useEffect(() => {
    let filtered = applications;
    
    if (search.trim()) {
      filtered = filtered.filter((app) =>
        app.companyName?.toLowerCase().includes(search.toLowerCase()) ||
        app.firstName?.toLowerCase().includes(search.toLowerCase()) ||
        app.surName?.toLowerCase().includes(search.toLowerCase()) ||
        app.email?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter !== "all") {
      filtered = filtered.filter((app) => 
        app.status?.toLowerCase() === filter.toLowerCase()
      );
    }
    
    setFilteredApps(filtered);
  }, [search, filter, applications]);

  const fetchApplications = async () => {
    try {
      setIsLoading(true);
      const data = await getCompanyApplications();
      setApplications(data || []);
      setFilteredApps(data || []);
    } catch (error) {
      console.error("Failed to fetch applications:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleStatusUpdate = async (id: string, status: string) => {
    setProcessingId(id);
    try {
      await updateCompanyApplication(id, { 
        status,
        note: updateNote || `Application ${status}`,
        reviewedAt: new Date().toISOString()
      });
      await fetchApplications();
      setUpdateNote("");
      setShowDetailsModal(false);
      setSelectedApp(null);
    } catch (error) {
      console.error("Failed to update application:", error);
      alert("Failed to update application. Please try again.");
    } finally {
      setProcessingId(null);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this application?")) return;
    try {
      await deleteCompanyApplication(id);
      await fetchApplications();
    } catch (error) {
      console.error("Failed to delete application:", error);
      alert("Failed to delete application. Please try again.");
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800 border-yellow-200", icon: Clock },
      approved: { label: "Approved", className: "bg-green-100 text-green-800 border-green-200", icon: CheckCircle },
      rejected: { label: "Rejected", className: "bg-red-100 text-red-800 border-red-200", icon: XCircle },
      reviewing: { label: "Reviewing", className: "bg-blue-100 text-blue-800 border-blue-200", icon: Eye },
    };
    const s = status?.toLowerCase() || "pending";
    return statusMap[s] || statusMap.pending;
  };

  const openReviewModal = (app: any) => {
    setSelectedApp(app);
    setShowDetailsModal(true);
    setUpdateNote("");
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
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Company Applications</h1>
        <p className="text-sm text-gray-500">Review and manage company registration applications</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total</p>
                <p className="text-2xl font-bold">{applications.length}</p>
              </div>
              <Building2 className="h-8 w-8 text-gray-400" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Pending</p>
                <p className="text-2xl font-bold text-yellow-600">
                  {applications.filter(a => a.status?.toLowerCase() === "pending" || a.status?.toLowerCase() === "reviewing").length}
                </p>
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
                <p className="text-2xl font-bold text-green-600">
                  {applications.filter(a => a.status?.toLowerCase() === "approved").length}
                </p>
              </div>
              <CheckCircle className="h-8 w-8 text-green-500" />
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Rejected</p>
                <p className="text-2xl font-bold text-red-600">
                  {applications.filter(a => a.status?.toLowerCase() === "rejected").length}
                </p>
              </div>
              <XCircle className="h-8 w-8 text-red-500" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Applications Table */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Applications ({filteredApps.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search applications..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
            <div className="flex gap-2">
              <select
                className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
              >
                <option value="all">All Status</option>
                <option value="pending">Pending</option>
                <option value="reviewing">Reviewing</option>
                <option value="approved">Approved</option>
                <option value="rejected">Rejected</option>
              </select>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredApps.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              <Building2 className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>{search || filter !== "all" ? "No applications match your filters" : "No applications found"}</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-gray-50">
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Applicant</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Company</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Contact</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Type</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                    <th className="text-center py-3 px-4 font-medium text-gray-600">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredApps.map((app) => {
                    const status = getStatusBadge(app.status);
                    const StatusIcon = status.icon;
                    const isPending = app.status?.toLowerCase() === "pending" || app.status?.toLowerCase() === "reviewing";
                    
                    return (
                      <tr key={app.id} className="border-b hover:bg-gray-50">
                        <td className="py-3 px-4">
                          <div className="flex items-center gap-3">
                            <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                              <span className="text-xs font-medium text-blue-600">
                                {app.firstName?.[0]}{app.surName?.[0]}
                              </span>
                            </div>
                            <div>
                              <p className="font-medium">{app.firstName} {app.surName}</p>
                              <p className="text-xs text-gray-500">{app.email}</p>
                            </div>
                          </div>
                        </td>
                        <td className="py-3 px-4">
                          <div className="flex items-center gap-2">
                            <Building2 className="h-4 w-4 text-gray-400" />
                            <span className="font-medium">{app.companyName}</span>
                          </div>
                        </td>
                        <td className="py-3 px-4">
                          <div className="flex items-center gap-2">
                            <Phone className="h-4 w-4 text-gray-400" />
                            {app.phoneNo || "N/A"}
                          </div>
                        </td>
                        <td className="py-3 px-4">
                          {app.companyType?.map((type: string) => (
                            <Badge key={type} variant="secondary" className="text-xs mr-1">
                              {type}
                            </Badge>
                          ))}
                        </td>
                        <td className="py-3 px-4">
                          <Badge className={`${status.className} flex items-center gap-1 w-fit`}>
                            <StatusIcon className="h-3 w-3" />
                            {status.label}
                          </Badge>
                        </td>
                        <td className="py-3 px-4 text-gray-500">
                          {app.createdAt ? new Date(app.createdAt).toLocaleDateString() : "N/A"}
                        </td>
                        <td className="py-3 px-4">
                          <div className="flex items-center justify-center gap-2">
                            {isPending ? (
                              <>
                                {/* Review Button - Opens Modal */}
                                <Button
                                  size="sm"
                                  className="h-8 px-3 bg-blue-500 hover:bg-blue-600 text-white flex items-center gap-1"
                                  onClick={() => openReviewModal(app)}
                                  disabled={processingId === app.id}
                                >
                                  <Eye className="h-3 w-3" />
                                  Review
                                </Button>

                                {/* Approve Button */}
                                <Button
                                  size="sm"
                                  className="h-8 px-3 bg-green-500 hover:bg-green-600 text-white flex items-center gap-1"
                                  onClick={() => {
                                    if (confirm(`Approve application from ${app.companyName}?`)) {
                                      handleStatusUpdate(app.id, "approved");
                                    }
                                  }}
                                  disabled={processingId === app.id}
                                >
                                  {processingId === app.id ? (
                                    <Loader2 className="h-3 w-3 animate-spin" />
                                  ) : (
                                    <CheckCircle className="h-3 w-3" />
                                  )}
                                  Approve
                                </Button>

                                {/* Reject Button */}
                                <Button
                                  size="sm"
                                  variant="outline"
                                  className="h-8 px-3 text-red-500 border-red-200 hover:bg-red-50 flex items-center gap-1"
                                  onClick={() => {
                                    if (confirm(`Reject application from ${app.companyName}?`)) {
                                      handleStatusUpdate(app.id, "rejected");
                                    }
                                  }}
                                  disabled={processingId === app.id}
                                >
                                  {processingId === app.id ? (
                                    <Loader2 className="h-3 w-3 animate-spin" />
                                  ) : (
                                    <XCircle className="h-3 w-3" />
                                  )}
                                  Reject
                                </Button>
                              </>
                            ) : (
                              <span className="text-xs text-gray-400">Already reviewed</span>
                            )}
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Review Modal */}
      {showDetailsModal && selectedApp && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-2xl p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold">Review Application</h3>
              <button 
                onClick={() => {
                  setShowDetailsModal(false);
                  setSelectedApp(null);
                  setUpdateNote("");
                }}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                <X className="h-5 w-5 text-gray-500" />
              </button>
            </div>

            <div className="space-y-4">
              {/* Applicant Details */}
              <div className="grid grid-cols-2 gap-4">
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-xs text-gray-500">Full Name</p>
                  <p className="font-medium">{selectedApp.firstName} {selectedApp.surName}</p>
                </div>
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-xs text-gray-500">Email</p>
                  <p className="font-medium">{selectedApp.email}</p>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-xs text-gray-500">Phone</p>
                  <p className="font-medium">{selectedApp.phoneNo || "N/A"}</p>
                </div>
                <div className="p-3 bg-gray-50 rounded-xl">
                  <p className="text-xs text-gray-500">Gender</p>
                  <p className="font-medium">{selectedApp.sex || "N/A"}</p>
                </div>
              </div>

              {/* Company Details */}
              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Company Name</p>
                <p className="font-medium">{selectedApp.companyName}</p>
              </div>

              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Company Username</p>
                <p className="font-medium">{selectedApp.companyUserName || "N/A"}</p>
              </div>

              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Company Type</p>
                <div className="flex flex-wrap gap-1 mt-1">
                  {selectedApp.companyType?.map((type: string) => (
                    <Badge key={type} className="bg-blue-100 text-blue-800 text-xs">
                      {type}
                    </Badge>
                  ))}
                </div>
              </div>

              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Business Address</p>
                <p className="font-medium">{selectedApp.address || selectedApp.companyAddress || "N/A"}</p>
              </div>

              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Application Date</p>
                <p className="font-medium">{new Date(selectedApp.createdAt).toLocaleString()}</p>
              </div>

              <div className="p-3 bg-gray-50 rounded-xl">
                <p className="text-xs text-gray-500">Current Status</p>
                <Badge className={getStatusBadge(selectedApp.status).className}>
                  {getStatusBadge(selectedApp.status).label}
                </Badge>
              </div>

              {/* Review Note */}
              <div>
                <Label htmlFor="updateNote">Review Note</Label>
                <Input
                  id="updateNote"
                  placeholder="Add a note about this application..."
                  value={updateNote}
                  onChange={(e) => setUpdateNote(e.target.value)}
                  className="mt-1"
                />
              </div>

              {/* Action Buttons */}
              <div className="flex gap-3 pt-4 border-t">
                <Button
                  className="flex-1 bg-green-500 hover:bg-green-600 text-white"
                  onClick={() => {
                    if (confirm(`Approve application from ${selectedApp.companyName}?`)) {
                      handleStatusUpdate(selectedApp.id, "approved");
                    }
                  }}
                  disabled={processingId === selectedApp.id}
                >
                  {processingId === selectedApp.id ? (
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  ) : (
                    <CheckCircle className="h-4 w-4 mr-2" />
                  )}
                  Approve Application
                </Button>
                <Button
                  variant="outline"
                  className="flex-1 text-red-500 border-red-200 hover:bg-red-50"
                  onClick={() => {
                    if (confirm(`Reject application from ${selectedApp.companyName}?`)) {
                      handleStatusUpdate(selectedApp.id, "rejected");
                    }
                  }}
                  disabled={processingId === selectedApp.id}
                >
                  {processingId === selectedApp.id ? (
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  ) : (
                    <XCircle className="h-4 w-4 mr-2" />
                  )}
                  Reject Application
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}